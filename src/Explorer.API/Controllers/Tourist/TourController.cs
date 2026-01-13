using Explorer.Payments.API.Public;
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
        private readonly IPaymentRecordService _paymentRecordService;

        public TourController(ITourService tourService, IPaymentRecordService paymentRecordService, ITourExecutionService tourExecutionService)
        {
            _tourService = tourService;
            _paymentRecordService = paymentRecordService;
            _tourExecutionService = tourExecutionService;
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
        public ActionResult<PagedResultDto<PublishedTourPreviewDto>> GetPublished(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 6,
            [FromQuery] int? environmentType = null,
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null,
            [FromQuery] String? suitableFor = null,
            [FromQuery] String? foodTypes = null,
            [FromQuery] String? adventureLevel = null,
            [FromQuery] String? activityTypes = null)
        {
            var filter = new TourFilterDto
            {
                Page = page,                    
                PageSize = pageSize,          
                EnvironmentType = environmentType,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                SuitableFor = suitableFor,
                FoodTypes = foodTypes,
                AdventureLevel = adventureLevel,
                ActivityTypes = activityTypes
            };

            return Ok(_tourService.GetFilteredTours(filter));
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
            catch (KeyNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
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

        // GET api/tourist/tours/mine
        [HttpGet("mine")]
        public ActionResult<IEnumerable<TourDto>> GetMyPurchasedTours()
        {
            var touristId = User.UserId();

            var payments = _paymentRecordService.GetMine(touristId);

            var tourIds = payments.Select(p => p.TourId).Distinct().ToList();

            if (!tourIds.Any())
                return Ok(new List<TourDto>());

            var tours = tourIds.Select(id => _tourService.Get(id)).ToList();

            foreach (var tour in tours)
            {
                var execution = _tourExecutionService.GetExecution(touristId, tour.Id);

                tour.IsActive = execution != null;
                tour.CanBeStarted = execution == null || execution.CompletedPercentage < 100;

                tour.KeyPoints = new List<KeyPointDto>();
            }

            return Ok(tours);
        }


    }
}
