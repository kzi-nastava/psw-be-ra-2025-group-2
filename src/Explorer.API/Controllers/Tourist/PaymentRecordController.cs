using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public;
using Explorer.Tours.API.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist
{
    [Authorize(Roles = "tourist")]
    [Route("api/payment-record")]
    [ApiController]
    public class PaymentRecordController : ControllerBase
    {
        private readonly IPaymentRecordService _recordService;

        public PaymentRecordController(IPaymentRecordService recordService)
        {
            _recordService = recordService;
        }

        [HttpPost]
        public ActionResult<PaymentRecordDto> Create([FromBody] PaymentRecordDto record)
        {
            return Ok(_recordService.Create(record));
        }
    }
}
