namespace BalanceService.Models
{
    public class Balance
    {
        public int id { get; set; }
        public long balance { get; set; }
    }
    public class TransferBalance
    {
        public int from { get; set; } 
        public int to { get; set; } 
        public long moneyAmount { get; set; }
    }
}
