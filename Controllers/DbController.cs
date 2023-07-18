using Newtonsoft.Json;
using System.Data;
using System.Net;

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
        }

        public responseType MutateBalance(Balance balance, bool deposite)
        {
            if (balance.id > 0)
            {
                var dbTable = _context.balance.Where(data => data.id.Equals(balance.id)).FirstOrDefault();
                if (dbTable != null && deposite)
                {
                    dbTable.balance += balance.balance;
                    _context.balance.Update(dbTable);
                }
                else if (dbTable != null)
                {
                    dbTable.balance -= balance.balance;
                    if (dbTable.balance < 0)
                    {
                        return responseType.NotEnoghMoney;
                    }
                    _context.balance.Update(dbTable);

                }
                _context.SaveChanges();
                return responseType.Succes;
            }
            return responseType.BadData;
        }

        public responseType TransferBetweenBankAccount(TransferBalance value)
        {
            if (value.from > 0 && value.to > 0 && value.moneyAmount > 0)
            {
                var dbTable = _context.balance.Where(data => data.id.Equals(value.from))
                    .Union(_context.balance.Where(data => data.id.Equals(value.to))).ToList();

                Console.WriteLine(dbTable[0].id);
                Console.WriteLine(dbTable[1].id);

                if (dbTable[0] != null && dbTable[1] != null)
                {
                    dbTable[0].balance += value.moneyAmount;
                    dbTable[1].balance -= value.moneyAmount;
                    if (dbTable[1].balance < 0)
                    {
                        return responseType.NotEnoghMoney;
                    }
                }
                _context.balance.Update(dbTable[0]);
                _context.balance.Update(dbTable[1]);
                _context.SaveChanges();
                return responseType.Succes;
            }
            return responseType.BadData;
        }

        public void CreateBalance(Balance balance)
        {
            Balance dbTable = new Balance();

            dbTable.id = _context.balance.Max(d => d.id) + 1;
            dbTable.balance = balance.balance;

            _context.balance.Add(dbTable);
            _context.SaveChanges();
        }

        /*        public void DeleteBalance(int id)
                {
                    var balance = _context.balance.Where(d => d.id.Equals(id)).FirstOrDefault();

                    if (balance != null)
                    {
                        _context.balance.Remove(balance);
                        _context.SaveChanges();
                    }
                }*/

    }
}
