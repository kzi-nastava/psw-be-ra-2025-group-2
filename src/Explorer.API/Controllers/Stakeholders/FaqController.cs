using Explorer.Stakeholders.API.Dtos.Help;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Stakeholders
{
    [Authorize]
    [Route("api/help/faq")]
    [ApiController]
    public class FaqController : ControllerBase
    {
        private readonly IFaqService _service;

        public FaqController(IFaqService service) 
        {
            _service = service;
        }

        [HttpGet]
        public ActionResult<List<FaqItemDto>> Get() => Ok(_service.GetActive());
    }
}
