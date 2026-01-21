using Explorer.Stakeholders.API.Dtos.Help;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Administrator.Administration
{
    [Authorize(Policy = "administratorPolicy")]
    [Route("api/administration/faq")]
    [ApiController]
    public class AdminFaqController : ControllerBase
    {
        private readonly IFaqService _service;

        public AdminFaqController(IFaqService service)
        {
            _service = service;
        }

        [HttpPost]
        public ActionResult<FaqItemDto> Create(CreateFaqItemDto dto) => Ok(_service.Create(dto));

        [HttpPut("{id:long}")]
        public ActionResult<FaqItemDto> Update(long id, UpdateFaqItemDto dto) => Ok(_service.Update(id, dto));

        [HttpDelete("{id:long}")]
        public IActionResult Deactivate(long id)
        {
            _service.Deactivate(id);
            return NoContent();
        }
    }
}
