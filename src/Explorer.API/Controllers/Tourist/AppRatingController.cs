using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


namespace Explorer.API.Controllers.Tourist
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

        [HttpPost]
        public ActionResult<AppRatingDto> Create([FromBody] AppRatingDto appRating)
        {
            appRating.UserId = User.PersonId();

            var result = _appRatingService.Create(appRating);
            return Ok(result);
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
            catch (KeyNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (UnauthorizedAccessException e)
            {
                return Forbid(e.Message); 
            }
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
            catch (KeyNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (UnauthorizedAccessException e)
            {
                return Forbid(e.Message); 
            }
        }


        [HttpGet]
        public ActionResult<PagedResult<AppRatingDto>> GetRatingByRole(int page = 1, int pageSize = 10)
        {
            long userId = User.PersonId();
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (role == "tourist" || role == "author")
            {
                var result = _appRatingService.GetPagedByUserId(userId, page, pageSize);
                return Ok(result);
            }
            else if (role == "administrator")
            {
                var result = _appRatingService.GetPaged(page, pageSize);
                return Ok(result);
            }

            return Ok();
        }



        [HttpGet("my-rating")]
        public ActionResult<IEnumerable<AppRatingDto>> GetMyRating()
        {
            var userId = User.PersonId();
            var result = _appRatingService.GetByUserId(userId);
            return Ok(result);
        }
    }
}