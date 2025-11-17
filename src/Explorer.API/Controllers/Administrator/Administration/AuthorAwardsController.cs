using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Administrator.Administration
{
    [Authorize(Policy = "administratorPolicy")]
    [Route("api/administration/authorawards")]
    [ApiController]
    public class AuthorAwardsController : ControllerBase
    {
        private readonly IAuthorAwardsService _awardsService;

        public AuthorAwardsController(IAuthorAwardsService awardsService)
        {
            _awardsService = awardsService;
        }

        [HttpGet]
        public ActionResult<PagedResult<AuthorAwardsDto>> GetAll([FromQuery] int page, [FromQuery] int pageSize)
        {
            return Ok(_awardsService.GetPaged(page, pageSize));
        }

        [HttpPost]
        public ActionResult<AuthorAwardsDto> Create([FromBody] AuthorAwardsDto awards)
        {
            return Ok(_awardsService.Create(awards));
        }

        [HttpPut("{id:long}")]
        public ActionResult<AuthorAwardsDto> Update([FromBody] AuthorAwardsDto awards)
        {
            return Ok(_awardsService.Update(awards));
        }

        [HttpDelete("{id:long}")]
        public ActionResult Delete(long id)
        {
            _awardsService.Delete(id);
            return Ok();
        }
    }
}
