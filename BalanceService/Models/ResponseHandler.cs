namespace BalanceService.Models
{
    public class ResponseHandler
    {
        public static ApiResponse GetExceptionResponse(Exception ex)
        {
            ApiResponse response = new ApiResponse();
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
                    response.Message = "Успешно";
                    break;
                case responseType.NotEnoghMoney:
                    response.Message = "Недостаточно средств для списания";
                    response.ResponseData = null;
                    break;
                case responseType.NotFound:
                    response.Message = "Запись не найдена";
                    response.ResponseData = null;
                    break;
                case responseType.BadData:
                    response.Message = "Введены не корректные данные";
                    response.ResponseData = null;
                    break;
            }
            return response;
        }
    }
}
