namespace BalanceService.Models
{
    public class TransferHistory
    {
        public int id { get; set; }
        public double moneyAmount { get; set; }
        public DateTime? date { get; set; }
        public string? description { get; set; }
        public int balanceId { get; set; }
    }
}
