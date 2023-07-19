using Humanizer;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using System.Data;
using System.Net;
using System.Text;

namespace BalanceService.Models
{
    public class CurencyExchange
    {
        public Dictionary<string, double>? rates { get; set; }
    }

    public class DbController
    {
        private DataContext _context;
        public DbController(DataContext context)
        {
            _context = context;
        }

        public double getCurrencyExchange(string? currency)
        {
            HttpWebRequest WebReq = (HttpWebRequest)WebRequest.Create(string.Format("https://www.cbr-xml-daily.ru/latest.js"));

            WebReq.Method = "GET";

            HttpWebResponse WebResp = (HttpWebResponse)WebReq.GetResponse();

            string jsonString;
            using (Stream stream = WebResp.GetResponseStream())   
            {
                StreamReader reader = new StreamReader(stream, System.Text.Encoding.UTF8);
                jsonString = reader.ReadToEnd();
            }
            CurencyExchange rate = JsonConvert.DeserializeObject<CurencyExchange>(jsonString);
            try
            {
                return rate.rates[currency];
            }
            catch (Exception)
            {
                return 1;
            }
        }

        public List<Balance> GetBalances(string? currency)
        {
            double rate = getCurrencyExchange(currency);

            List<Balance> response = new List<Balance>();
            var dataList = _context.balance.ToList();
            dataList.ForEach(row => response.Add(new Balance()
            {
                id = row.id,
                balance = row.balance * rate
            }));
            return response;
        }

        public Balance GetBalance(int id, string? currency)
        {
            var dataList = _context.balance.Where(data => data.id.Equals(id)).FirstOrDefault();
            double rate = getCurrencyExchange(currency);

            return new Balance()
            {
                id = dataList.id,
                balance = dataList.balance * rate
            };
            SendNewMessage(id, "Просмотр баланса");
        }

        public responseType MutateBalance(Balance balance, bool deposite)
        {
            if (balance.id > 0)
            {
                var dbTable = _context.balance.Where(data => data.id.Equals(balance.id)).FirstOrDefault();
                if (dbTable != null && deposite)
                {
                    dbTable.balance += balance.balance;
                    SetNewTransferHistory(dbTable.id, "Пополнение счета", balance.balance);
                    SendNewMessage(dbTable.id, "Пополнение счета");
                    _context.balance.Update(dbTable);
                }
                else if (dbTable != null)
                {
                    dbTable.balance -= balance.balance;
                    if (dbTable.balance < 0)
                    {
                        return responseType.NotEnoghMoney;
                    }
                    SetNewTransferHistory(dbTable.id, "Списание с счета", balance.balance);
                    SendNewMessage(dbTable.id, "Списание с счета");
                    _context.balance.Update(dbTable);

                }
                else if (dbTable == null && deposite)
                {
                    CreateBalance(balance);
                    SendNewMessage(balance.id, "Пополнение счета");
                    return responseType.Created;
                }
                else if (dbTable == null && !deposite)
                    return responseType.BadData;

                _context.SaveChanges();
                return responseType.Success;
            }
            return responseType.BadData;
        }

        public responseType TransferBetweenBankAccount(TransferBalance value)
        {
            if (value.from > 0 && value.to > 0 && value.moneyAmount > 0)
            {
                var dbTable = _context.balance.Where(data => data.id.Equals(value.from))
                    .Union(_context.balance.Where(data => data.id.Equals(value.to))).ToList();
                int to, from;

                if (dbTable.ElementAtOrDefault(0) != null && dbTable.ElementAtOrDefault(1) != null)
                {
                    to = dbTable.FindIndex(d => d.id == value.to);
                    from = dbTable.FindIndex(d => d.id == value.from);

                    dbTable[to].balance += value.moneyAmount;
                    dbTable[from].balance -= value.moneyAmount;
    
                    if (dbTable[from].balance < 0)
                    {
                        return responseType.NotEnoghMoney;
                    }

                    _context.balance.Update(dbTable[to]);
                    _context.balance.Update(dbTable[from]);
                    _context.SaveChanges();

                    SetNewTransferHistory(dbTable[from].id, $"Перевод клиенту с id - {dbTable[to].id}", value.moneyAmount);
                    SetNewTransferHistory(dbTable[to].id, $"Перевод от клиента с id - {dbTable[from].id}", value.moneyAmount);

                    return responseType.Success;
                }
                return responseType.BadData;
            }
            return responseType.BadData;
        }

        public void CreateBalance(Balance balance)
        {
            Balance dbTable = new Balance();

            dbTable.id = balance.id;
            dbTable.balance = balance.balance;

            _context.balance.Add(dbTable);
            _context.SaveChanges();
        }

        public List<TransferHistory> GetTransferHistory(int id, string sortBy, string page)
        {
            List<TransferHistory> response = new List<TransferHistory>();
            var dataList = _context.history.Where(data => data.balanceId.Equals(id)).ToList();

            if (sortBy == "moneyAmount") 
                dataList = dataList.OrderBy(data => data.moneyAmount).ToList();
            else if (sortBy == "date")
                dataList = dataList.OrderBy(data => data.date).ToList();

            dataList.ForEach(row => response.Add(new TransferHistory()
            {
                id = row.id,
                moneyAmount = row.moneyAmount,
                date = row.date,
                description = row.description,
                balanceId = row.balanceId,
            }));
            SendNewMessage(id, "Просмотр транзакций");
            try
            {
                int pagination = Int32.Parse(page);
                if (pagination == 1)
                    response = (List<TransferHistory>)response.Take(5).ToList();
                else
                    response = (List<TransferHistory>)response.Skip((pagination - 1) * 5).Take(5).ToList();
                }
            catch (Exception)
            {
                return response;
            }
            return response;
        }

        public void SetNewTransferHistory(int id, string description, double moneyAmount)
        {
            TransferHistory newTransfer = new TransferHistory();

            newTransfer.id = (_context.history.Max(d => (int?)d.id) ?? 0) + 1;
            newTransfer.moneyAmount = moneyAmount;
            newTransfer.description = description;
            newTransfer.date = DateTime.UtcNow ;
            newTransfer.balanceId = id;

            _context.history.Add(newTransfer);
            SendNewMessage(id, description);
            _context.SaveChanges();
        }

        public void SendNewMessage(int id, string message)
        {
            try
            {
                var factory = new ConnectionFactory() { HostName = "localhost" };
                using (var connection = factory.CreateConnection())
                {
                    using (var channel = connection.CreateModel())
                    {
                        channel.QueueDeclare(
                            queue: $"{id}",
                            exclusive: false,
                            durable: true,
                            autoDelete: false,
                            arguments: null
                            );

                        var body = Encoding.UTF8.GetBytes(message);

                        channel.BasicPublish(
                            exchange: "",
                            routingKey: $"{id}",
                            basicProperties: null,
                            body: body
                            );
                    }
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Rabbit dosen't congigured well");
            }

        }
    }
}
