using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Explorer.API.Controllers.Tourist
{
    [Authorize(Roles = "author, tourist")]

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

        [HttpPut]
        public ActionResult<AppRatingDto> Update([FromBody] AppRatingDto appRating)
        {
            appRating.UserId = User.PersonId();
            var result = _appRatingService.Update(appRating);
            return Ok(result);
        }

        [HttpDelete("{id:long}")]
        public ActionResult Delete(long id)
        {
            _appRatingService.Delete(id);
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