using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist;

//[Authorize(Policy = "touristPolicy")]
[Route("api/tours/search")]
[ApiController]
public class TourSearchController : ControllerBase
{
    private readonly ITourService _tourService;

    public TourSearchController(ITourService tourService)
    {
        _tourService = tourService;
    }

    [HttpPost("filter")]
    public ActionResult<PagedResultDto<PublishedTourPreviewDto>> FilterTours([FromBody] TourFilterDto filter)
    {
        try
        {
            var result = _tourService.GetFilteredTours(filter);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }
}