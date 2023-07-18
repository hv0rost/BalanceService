using System.Data;

namespace BalanceService.Models
{
    public class DbController
    {
        private BalanceContext _context;
        public DbController(BalanceContext context)
        {
            _context = context;
        }

        public List<Balance> GetBalances()
        {
            List<Balance> response = new List<Balance>();
            var dataList = _context.balance.ToList();
            dataList.ForEach(row => response.Add(new Balance()
            {
                id = row.id,
                balance = row.balance
            }));

            return response;
        }

        public Balance GetBalance(int id)
        {
            var dataList = _context.balance.Where(data => data.id.Equals(id)).FirstOrDefault();

            return new Balance()
            {
                id = dataList.id,
                balance = dataList.balance
            };
        }

        public responseType MutateBalance(Balance balance, bool deposite)
        {
            //Balance dbTable = new Balance();
            if (balance.id > 0)
            {
                var dbTable = _context.balance.Where(data => data.id.Equals(balance.id)).FirstOrDefault();
                if (dbTable != null && deposite)
                {
                    dbTable.balance += balance.balance;
                    _context.balance.Update(dbTable);
                }
                else if (dbTable != null) {
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
            return responseType.NotFound;
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
            return responseType.NotFound;
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
