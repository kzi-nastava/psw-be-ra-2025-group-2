using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.API.Public.Execution;
using Explorer.Tours.Core.UseCases.Administration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist;

[Authorize(Policy = "touristPolicy")]
[Route("api/tourist/tour-review")]
[ApiController]
public class TourReviewController : ControllerBase
{
    private readonly ITourReviewService _reviewService;
    private readonly ITourExecutionService _executionService;
    public TourReviewController(ITourReviewService reviewService, ITourExecutionService executionService)
    {
        _reviewService = reviewService;
        _executionService = executionService;
    }

    [HttpGet("{tourId:int}")]
    public ActionResult<PagedResult<TourReviewDto>> GetByTourId(int tourId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        return Ok(_reviewService.GetByTourId(page, pageSize, tourId));
    }

    [HttpPost]
    public ActionResult<TourReviewDto> Create([FromBody] TourReviewDto review)
    {
        var identityId = int.Parse(User.FindFirst("id")?.Value ?? "0");

        bool canReview = _executionService.IsFinishedEnough(identityId, review.TourId);
        if (!canReview)
        {
            return BadRequest("Ne možete oceniti turu. Uslovi: min. 35% pređenog puta i aktivnost u poslednjih 7 dana.");
        }

        try
        {
            var result = _reviewService.Create(review, identityId);
            return Ok(result);
        }
        catch (ArgumentException e)
        {
            return BadRequest(e.Message);
        }
        catch (InvalidOperationException e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPut("{id:int}")]
    public ActionResult<TourReviewDto> Update([FromBody] TourReviewDto review)
    {
        var identityId = int.Parse(User.FindFirst("id")?.Value ?? "0");
        try
        {
            var result = _reviewService.Update(review, identityId);
            return Ok(result);
        }
        catch (ArgumentException e)
        {
            return BadRequest(e.Message);
        }
        catch (InvalidOperationException e)
        {
            return BadRequest(e.Message);
        }
        catch (UnauthorizedAccessException e)
        {
            return StatusCode(403, e.Message);
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
    [HttpDelete("{id:long}")] 
    public ActionResult Delete(long id)
    {
        try
        {
            var touristId = int.Parse(HttpContext.User.Claims.First(c => c.Type == "id").Value);
            _reviewService.Delete(id, touristId);
            return Ok();
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, ex.Message);
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("not found"))
            {
                return NotFound(ex.Message);
            }

            return BadRequest(ex.Message);
        }
    }
}
