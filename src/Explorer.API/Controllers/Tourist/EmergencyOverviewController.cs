using Explorer.Stakeholders.API.Public.Emergency;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist
{
    [ApiController]
    [Route("api/emergency")]
    public class EmergencyOverviewController : ControllerBase
    {
        private readonly IEmergencyOverviewService _service;

        public EmergencyOverviewController(IEmergencyOverviewService service)
        {
            _service = service;
        }

        [HttpGet("overview")]
        public ActionResult GetOverview([FromQuery] string country)
        {
            return Ok(_service.GetOverview(country));
        }
    }
}
