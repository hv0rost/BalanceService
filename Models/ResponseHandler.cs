namespace BalanceService.Models
{
    public class ResponseHandler
    {
        public static ApiResponse GetExceptionResponse(Exception ex)
        {
            ApiResponse response = new ApiResponse();
            response.Code = 1;
            response.ResponseData = ex.Message;
            return response;
        }

        public static ApiResponse GetAppResponse(responseType type, object? responseData)
        {
            ApiResponse response;
            response = new ApiResponse { ResponseData = responseData };

            switch (type)
            {
                case responseType.Succes:
                    response.Code = 0;
                    response.Message = "Success";
                    break;
                case responseType.NotFound:
                    response.Code = 2;
                    response.Message = "No record available";
                    break;
            }
            return response;
        }
    }
}
