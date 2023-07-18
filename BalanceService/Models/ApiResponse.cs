namespace BalanceService.Models
{
    public class ApiResponse
    {
        public string? Message { get; set; }
        public object? ResponseData { get; set; }
    }

    public enum responseType
    {
        Succes,
        NotFound,
        Failure,
        NotEnoghMoney,
        BadData,
        Created
    }
}
