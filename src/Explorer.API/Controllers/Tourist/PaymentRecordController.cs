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

        // POST: api/payment-records/checkout
        [HttpPost("checkout")]
        public IActionResult Checkout()
        {
            var touristIdClaim = User.FindFirst("id");
            if (touristIdClaim == null) return Unauthorized();

            var touristId = long.Parse(touristIdClaim.Value);

            _recordService.Checkout(touristId);

            return NoContent();
        }
    }
}
