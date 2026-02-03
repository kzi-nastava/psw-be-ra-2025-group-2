using Explorer.Stakeholders.API.Dtos.Emergency;
using Explorer.Stakeholders.API.Public.Emergency;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist
{
    [ApiController]
    [Route("api/emergency/phrasebook")]
    public class EmergencyPhrasebookController : ControllerBase
    {
        private readonly IEmergencyPhrasebookService _service;

        public EmergencyPhrasebookController(IEmergencyPhrasebookService service)
        {
            _service = service;
        }

        [HttpGet("languages")]
        public IActionResult GetLanguages()
        {
            return Ok(_service.GetLanguages());
        }

        [HttpGet("sentences")]
        public IActionResult GetSentences([FromQuery] string lang = "en")
        {
            return Ok(_service.GetSentences(lang));
        }

        [HttpPost("translate")]
        public IActionResult Translate([FromBody] EmergencyPhrasebookTranslateRequestDto request)
        {
            try
            {
                return Ok(_service.Translate(request));
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }
    }
}
