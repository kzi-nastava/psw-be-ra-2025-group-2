using Explorer.Stakeholders.Infrastructure.Authentication;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Execution;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist
{
    [Authorize(Policy = "touristPolicy")]
    [Route("api/tourist/tour-execution")]
    [ApiController]
    public class TourExecutionController : ControllerBase
    {
        private readonly ITourExecutionService _executionService;

        public TourExecutionController(ITourExecutionService executionService)
        {
            _executionService = executionService;
        }

        [HttpGet("user/{tourId:long}")]
        public ActionResult<TourExecutionDto> GetExecution(long tourId)
        {
            try
            {
                var result = _executionService.GetExecution(User.UserId(), tourId);

                if (result == null) return NotFound();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("start/{id:long}")]
        public ActionResult<TourExecutionDto> Proceed(long id)
        {
            try
            {
                var data = _executionService.Proceed(User.UserId(), id);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("abandon/{id:long}")]
        public IActionResult Abandon(long id)
        {
            try
            {
                _executionService.Abandon(User.UserId(), id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("complete/{id:long}")]
        public IActionResult Complete(long id)
        {
            try
            {
                _executionService.Complete(User.UserId(), id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("data/{executionId:long}")]
        public ActionResult<TourExecutionDto> GetExecutionData(long executionId)
        {
            try
            {
                var data = _executionService.GetExecutionData(User.UserId(), executionId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("query-key-point-visit/{id:long}")]
        public ActionResult<KeyPointVisitResponseDto> QueryKeyPointVisit(long id, [FromBody] PositionDto position)
        {
            try
            {
                var response = _executionService.QueryKeyPointVisit(User.UserId(), id, position);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
