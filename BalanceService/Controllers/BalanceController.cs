using BalanceService.Models;
using Microsoft.AspNetCore.Mvc;

namespace BalanceService.Controllers
{
    [ApiController]
    public class BalanceController : ControllerBase
    {
        private readonly DbController _db;
        public BalanceController(DataContext dataContext)
        {
            _db = new DbController(dataContext);
        }

        [HttpGet]
        [Route("balance")]

        public IActionResult GetBalance([FromQuery(Name = "currency")] string? currency)
        {
            responseType type = responseType.Success;
            try
            {
                IEnumerable<Balance> data = _db.GetBalances(currency);

                if (!data.Any())
                {
                    type = responseType.NotFound;
                    return NotFound(ResponseHandler.GetAppResponse(type, data));
                }
                return Ok(ResponseHandler.GetAppResponse(type, data));
            }
            catch (Exception ex)
            {
                type = responseType.Failure;
                return BadRequest(ResponseHandler.GetExceptionResponse(ex));
            }

        }

        [HttpGet]
        [Route("balance/{id}")]
        public IActionResult GetBalanceById(int id, [FromQuery(Name = "currency")] string? currency)
        {
            responseType type = responseType.Success;
            try
            {
                Balance data = _db.GetBalance(id, currency);

                if (data == null)
                {
                    type = responseType.NotFound;
                    return NotFound(ResponseHandler.GetAppResponse(type, data));
                }
                return Ok(ResponseHandler.GetAppResponse(type, data));
            }
            catch (Exception ex)
            {
                type = responseType.Failure;
                return BadRequest(ResponseHandler.GetExceptionResponse(ex));
            }
        }

        [HttpPut]
        [Route("balance/depositeMoney")]
        public IActionResult DepositeMoney([FromBody] Balance value)
        {
            responseType type = responseType.Success;
            try
            {
                _db.MutateBalance(value, true);
                return Ok(ResponseHandler.GetAppResponse(type, value));
            }
            catch (Exception ex)
            {
                type = responseType.Failure;
                return BadRequest(ResponseHandler.GetExceptionResponse(ex));
            }
        }

        [HttpPut]
        [Route("balance/chargeMoney")]
        public IActionResult ChargeMoney([FromBody] Balance value)
        {
            responseType type = responseType.Success;
            try
            {
                type = _db.MutateBalance(value, false);
                return Ok(ResponseHandler.GetAppResponse(type, value));
            }
            catch (Exception ex)
            {
                type = responseType.Failure;
                return BadRequest(ResponseHandler.GetExceptionResponse(ex));
            }
        }

        [HttpPut]
        [Route("balance/transferMoney")]
        public IActionResult TransferMoney([FromBody] TransferBalance transferData)
        {
            responseType type = responseType.Success;
            try
            {
                type = _db.TransferBetweenBankAccount(transferData);
                return Ok(ResponseHandler.GetAppResponse(type, transferData));
            }
            catch (Exception ex)
            {
                type = responseType.Failure;
                return BadRequest(ResponseHandler.GetExceptionResponse(ex));
            }
        }

        [HttpGet]
        [Route("transferHistory/{id}")]
        public IActionResult GetTransferHistory(int id, [FromQuery(Name = "sortBy")] string? sortBy, [FromQuery(Name = "page")] string? page)
        {
            responseType type = responseType.Success;
            try
            {
                List<TransferHistory> data = _db.GetTransferHistory(id, sortBy, page);

                if (data == null)
                {
                    type = responseType.NotFound;
                    return NotFound(ResponseHandler.GetAppResponse(type, data));
                }
                return Ok(ResponseHandler.GetAppResponse(type, data));
            }
            catch (Exception ex)
            {
                type = responseType.Failure;
                return BadRequest(ResponseHandler.GetExceptionResponse(ex));
            }
        }
    }
}
