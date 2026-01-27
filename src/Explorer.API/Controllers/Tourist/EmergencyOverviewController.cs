using Explorer.Stakeholders.API.Public.Emergency;
using Microsoft.AspNetCore.Mvc;
using Explorer.Stakeholders.API.Dtos.Emergency;

namespace Explorer.API.Controllers.Tourist
{
    [ApiController]
    [Route("api/emergency")]
    public class EmergencyOverviewController : ControllerBase
    {
        private readonly IEmergencyOverviewService _service;
        private readonly IEmergencyTranslationService _translation;

        public EmergencyOverviewController(IEmergencyOverviewService service, IEmergencyTranslationService translation)
        {
            _service = service;
            _translation = translation;
        }

        [HttpGet("overview")]
        public ActionResult GetOverview([FromQuery] string country)
        {
            return Ok(_service.GetOverview(country));
        }

        [HttpPost("translate")]
        public ActionResult Translate([FromBody] EmergencyTranslationRequestDto request)
        {
            return Ok(_translation.Translate(request));
        }
    }
}
