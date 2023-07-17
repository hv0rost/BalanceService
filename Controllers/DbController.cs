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
            Balance response = new Balance();
            var dataList = _context.balance.Where(data => data.id.Equals(id)).FirstOrDefault();

            return new Balance()
            {
                id = dataList.id,
                balance = dataList.balance
            };
        }

        public void MutateBalance(Balance balance)
        {
            Balance dbTable = new Balance();
            if (balance.id > 0)
            {
                //Put
                dbTable = _context.balance.Where(data => data.id.Equals(balance.id)).FirstOrDefault();
                if (dbTable != null)
                {
                    Console.WriteLine(213);
                    Console.WriteLine(dbTable.balance);
                    dbTable.balance += balance.balance;
                    _context.balance.Update(dbTable);
                }
                _context.SaveChanges();
            }
        }

        public void CreateBalance(Balance balance)
        {
            Balance dbTable = new Balance();

            dbTable.id = _context.balance.Max(d => d.id) + 1;
            dbTable.balance = balance.balance;

            _context.balance.Add(dbTable);
            _context.SaveChanges();
        }

        public void DeleteBalance(int id)
        {
            var balance = _context.balance.Where(d => d.id.Equals(id)).FirstOrDefault();

            if (balance != null)
            {
                _context.balance.Remove(balance);
                _context.SaveChanges();
            }
        }
        
    }
}
