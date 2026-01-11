using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public;
using Explorer.Tours.API.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist
{

    [Authorize(Roles = "tourist")]
    [Route("api/payment-records")]
    [ApiController]
    public class PaymentRecordController : ControllerBase
    {
        private readonly IPaymentRecordService _recordService;

        public PaymentRecordController(IPaymentRecordService recordService)
        {
            _recordService = recordService;
        }

        // GET: api/payment-records/mine
        [HttpGet("mine")]
        public IActionResult GetMine()
        {
            var touristIdClaim = User.FindFirst("id");
            if (touristIdClaim == null) return Unauthorized();

            var touristId = long.Parse(touristIdClaim.Value);
            var result = _recordService.GetMine(touristId);

            return Ok(result);
        }

        // GET: api/payment-records/{id}
        [HttpGet("{id:long}")]
        public IActionResult GetMineById(long id)
        {
            var touristIdClaim = User.FindFirst("id");
            if (touristIdClaim == null) return Unauthorized();

            var touristId = long.Parse(touristIdClaim.Value);

            try
            {
                var result = _recordService.GetMineById(touristId, id);
                return Ok(result);
            }
            catch (KeyNotFoundException e)
            {
                return NotFound(e.Message);
            }
        }
    }
}
