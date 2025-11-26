using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Explorer.API.Controllers
{
    [Authorize(Roles = "author, tourist, administrator")]
    [Route("api/app-ratings")]
    [ApiController]
    public class AppRatingController : ControllerBase
    {
        private readonly IAppRatingService _appRatingService;

        public AppRatingController(IAppRatingService appRatingService)
        {
            _appRatingService = appRatingService;
        }

        [HttpGet]
        [Authorize(Roles = "administrator")]
        public ActionResult<PagedResult<AppRatingDto>> GetAll([FromQuery] int page, [FromQuery] int pageSize)
        {
            var result = _appRatingService.GetPaged(page, pageSize);
            return Ok(result);
        }

        [HttpGet("my-rating")]
        [Authorize(Roles = "author, tourist")] // Eksplicitno samo za autore i turiste
        public ActionResult<IEnumerable<AppRatingDto>> GetMyRating()
        {
            var userId = User.PersonId();
            var result = _appRatingService.GetByUserId(userId);
            return Ok(result);
        }


        [HttpPost]
        [Authorize(Roles = "author, tourist")]
        public ActionResult<AppRatingDto> Create([FromBody] AppRatingDto appRating)
        {
            try
            {
                appRating.UserId = User.PersonId();
                var result = _appRatingService.Create(appRating);
                return Ok(result);
            }
            catch (InvalidOperationException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("{id:long}")]
        public ActionResult<AppRatingDto> Update(long id, [FromBody] AppRatingDto appRating)
        {
            appRating.Id = id;
            long userId = User.PersonId();
            string userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            try
            {
                var result = _appRatingService.Update(appRating, userId, userRole);
                return Ok(result);
            }
            catch (KeyNotFoundException e) { return NotFound(e.Message); }
            catch (UnauthorizedAccessException e) { return Forbid(e.Message); }
        }

        [HttpDelete("{id:long}")]
        public ActionResult Delete(long id)
        {
            long userId = User.PersonId();
            string userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            try
            {
                _appRatingService.Delete(id, userId, userRole);
                return Ok();
            }
            catch (KeyNotFoundException e) { return NotFound(e.Message); }
            catch (UnauthorizedAccessException e) { return Forbid(e.Message); }
        }
    }
}