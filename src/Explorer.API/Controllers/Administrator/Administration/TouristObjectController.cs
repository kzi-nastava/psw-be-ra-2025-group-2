using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Administrator.Administration;

[Authorize(Policy = "administratorPolicy")]
[Route("api/administration/touristobject")]
[ApiController]
public class TouristObjectController : ControllerBase
{
    private readonly ITouristObjectService _touristObjectService;

    public TouristObjectController(ITouristObjectService touristObjectService)
    {
        _touristObjectService = touristObjectService;
    }

    [HttpGet]
    public ActionResult<PagedResult<TouristObjectDto>> GetAll([FromQuery] int page, [FromQuery] int pageSize)
    {
        return Ok(_touristObjectService.GetPaged(page, pageSize));
    }

    [HttpPost]
    public ActionResult<TouristObjectDto> Create([FromBody] TouristObjectDto touristObject)
    {
        return Ok(_touristObjectService.Create(touristObject));
    }

    [HttpPut("{id:long}")]
    public ActionResult<TouristObjectDto> Update([FromBody] TouristObjectDto touristObject)
    {
        return Ok(_touristObjectService.Update(touristObject));
    }

    [HttpDelete("{id:long}")]
    public ActionResult Delete(long id)
    {
        _touristObjectService.Delete(id);
        return Ok();
    }
}