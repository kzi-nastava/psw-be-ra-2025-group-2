using Explorer.Stakeholders.Infrastructure.Authentication;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.API.Public.Execution;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist
{
    [Authorize(Policy = "touristPolicy")]
    [Route("api/tourist/tours")]
    [ApiController]
    public class TourController : ControllerBase
    {
        private readonly ITourService _tourService;
        private readonly ITourExecutionService _tourExecutionService;

        public TourController(ITourService tourService)
        {
            _tourService = tourService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<TourDto>> GetAll()
        {
            return Ok(_tourService.GetAvailableForTourist(User.UserId()));
        }

        [HttpGet("{id}")]
        public ActionResult<TourDto> GetById(long id)
        {

            var result = _tourService.Get(id);
            return Ok(result);
        }

        [HttpGet("published")]
        public ActionResult<List<PublishedTourPreviewDto>> GetPublished()
        {
            return Ok(_tourService.GetPublishedForTourist());
        }

        [HttpPost("rate")]
        public ActionResult<TourReviewDto> RateTour([FromBody] TourReviewDto reviewDto)
        {
            try
            {
                var result = _tourService.AddReview(
                    reviewDto.TourId,
                    User.UserId(),
                    reviewDto.Rating,
                    reviewDto.Comment,
                    reviewDto.Images
                );
                return Ok(result);
            }
            catch (InvalidOperationException e)
            {
                return BadRequest(e.Message);
            }
            catch (KeyNotFoundException e)
            {
                return NotFound(e.Message);
            }
        }

        [HttpPut("rate")]
        public ActionResult<TourReviewDto> UpdateReview([FromBody] TourReviewDto review)
        {
            var userId = long.Parse(HttpContext.User.Claims.First(i => i.Type.Equals("id", StringComparison.OrdinalIgnoreCase)).Value);

            if (review.TouristId != userId)
                return Forbid(); // Ili BadRequest("You can only edit your own reviews.");

            try
            {
                var result = _tourService.UpdateReview(review);
                return Ok(result);
            }
            catch (KeyNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("reviews/{tourId}")]
        public ActionResult DeleteReview(long tourId)
        {
            var userId = long.Parse(HttpContext.User.Claims.First(i => i.Type.Equals("id", StringComparison.OrdinalIgnoreCase)).Value);

            try
            {
                _tourService.DeleteReview(userId, tourId);
                return Ok();
            }
            catch (KeyNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
