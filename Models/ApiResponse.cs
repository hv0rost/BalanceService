namespace BalanceService.Models
{
    public class ApiResponse
    {
        public int Code {get; set;}    
        public string? Message {get; set;}
        public object? ResponseData {get; set;}   
    }

    public enum responseType
    {
        Succes,
        NotFound,
        Failure
    }
}
