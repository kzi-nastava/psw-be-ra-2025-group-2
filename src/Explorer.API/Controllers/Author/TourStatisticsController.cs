using Explorer.Stakeholders.Infrastructure.Authentication;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Statistics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Author
{
    [Authorize(Policy = "authorPolicy")]
    [Route("api/author/tour-statistics")]
    [ApiController]
    public class TourStatisticsController : ControllerBase
    {
        private readonly ITourStatisticsService _statisticsService;

        public TourStatisticsController(ITourStatisticsService statisticsService)
        {
            _statisticsService = statisticsService;
        }

        [HttpGet]
        public ActionResult<AuthorTourStatisticsDto> GetAuthorStatistics()
        {
            try
            {
                var authorId = User.UserId();
                var result = _statisticsService.GetAuthorTourStatistics(authorId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{tourId:long}")]
        public ActionResult<TourCompletionStatisticsDto> GetTourStatistics(long tourId)
        {
            try
            {
                var result = _statisticsService.GetTourCompletionStatistics(tourId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
