using Explorer.Stakeholders.API.Dtos.Help;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Stakeholders
{
    [Authorize]
    [Route("api/help/settings")]
    [ApiController]
    public class HelpSettingsController : ControllerBase
    {
        private readonly IHelpSettingsService _service;

        public HelpSettingsController(IHelpSettingsService service)
        {
            _service = service;
        }

        [HttpGet]
        public ActionResult<HelpSettingsDto> Get() => Ok(_service.GetOrCreate(User.PersonId()));

        [HttpPut]
        public ActionResult<HelpSettingsDto> Update(UpdateHelpSettingsDto dto) => Ok(_service.Update(User.PersonId(), dto.ShowTooltips));
    }
}
