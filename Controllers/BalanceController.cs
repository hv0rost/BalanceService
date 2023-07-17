using BalanceService.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BalanceService.Controllers
{
    [ApiController]
    public class BalanceController : ControllerBase
    {
        private readonly DbController _db;
        public BalanceController(BalanceContext balanceContext)
        {
            _db = new DbController(balanceContext);
        }
        // GET: api/<BalanceController>
        [HttpGet]
        [Route("balance")]

        public IActionResult Get()
        {
            responseType type = responseType.Succes;
            try
            {
                IEnumerable<Balance> data = _db.GetBalances();
               
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

        // GET api/<BalanceController>/5
        [HttpGet]
        [Route("balance/{id}")]
        public IActionResult Get(int id)
        {
            responseType type = responseType.Succes;
            try
            {
                Balance data = _db.GetBalance(id);

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

        // POST api/<BalanceController>
        [HttpPost]
        [Route("balance/create")]

        public IActionResult Post([FromBody] Balance value)
        {
            responseType type = responseType.Succes;
            try
            {
                _db.CreateBalance(value);
                return Ok(ResponseHandler.GetAppResponse(type, value));
            }
            catch (Exception ex)
            {
                type = responseType.Failure;
                return BadRequest(ResponseHandler.GetExceptionResponse(ex));
            }
        }

        // PUT api/<BalanceController>/5
        [HttpPut]
        [Route("balance/depositeMoney")]
        public IActionResult Put([FromBody] Balance value)
        {
            responseType type = responseType.Succes;
            try
            {
                _db.MutateBalance(value);
                return Ok(ResponseHandler.GetAppResponse(type, value));
            }
            catch (Exception ex)
            {
                type = responseType.Failure;
                return BadRequest(ResponseHandler.GetExceptionResponse(ex));
            }
        }

        // DELETE api/<BalanceController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
