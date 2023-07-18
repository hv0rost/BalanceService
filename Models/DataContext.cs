using Microsoft.EntityFrameworkCore;

namespace BalanceService.Models
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<Balance> balance { get; set; }
        public DbSet<TransferHistory> history { get; set; }    
    }
}
