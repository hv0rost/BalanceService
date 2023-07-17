using Microsoft.EntityFrameworkCore;

namespace BalanceService.Models
{
    public class BalanceContext : DbContext
    {
        public BalanceContext(DbContextOptions<BalanceContext> options) : base(options) { }

        public DbSet<Balance> balance { get; set; }    
    }
}
