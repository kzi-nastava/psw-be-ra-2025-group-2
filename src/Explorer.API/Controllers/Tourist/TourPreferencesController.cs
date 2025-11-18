using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist;

[Authorize(Policy = "touristPolicy")]
[Route("api/tourist/preferences")]
[ApiController]
public class TourPreferencesController : ControllerBase
{
    private readonly ITourPreferencesService _tourPreferencesService;

    public TourPreferencesController(ITourPreferencesService tourPreferencesService)
    {
        _tourPreferencesService = tourPreferencesService;
    }

    [HttpGet("{touristId:long}")]
    public ActionResult<TourPreferencesDto> Get(long touristId)
    {
        var result = _tourPreferencesService.GetByTourist(touristId);
        return Ok(result);
    }

    [HttpPost]
    public ActionResult<TourPreferencesDto> Create([FromBody] TourPreferencesDto dto)
    {
        var created = _tourPreferencesService.Create(dto);
        return Ok(created);
    }

    [HttpPut("{id:long}")]
    public ActionResult<TourPreferencesDto> Update([FromBody] TourPreferencesDto dto)
    {
        var updated = _tourPreferencesService.Update(dto);
        return Ok(updated);
    }

    [HttpDelete("{id:long}")]
    public ActionResult Delete(long id)
    {
        _tourPreferencesService.Delete(id);
        return Ok();
    }
}
