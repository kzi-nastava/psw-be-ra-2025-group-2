using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist
{
    [Route("api/tourist/position")]
    [ApiController]
    public class TouristPositionController : ControllerBase
    {
        private readonly ITouristPositionService _positionService;

        public TouristPositionController(ITouristPositionService positionService)
        {
            _positionService = positionService;
        }

        [HttpGet("{id:long}")]
        public ActionResult<TouristPositionDto> GetByTouristId(long id)
        {
            try
            {
                var result = _positionService.GetByTouristId(id);
                return Ok(result);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("{id:long}")]
        public ActionResult<TouristPositionDto> Update(long id, TouristPositionDto dto)
        {
            var updated = _positionService.Update(id, dto);

            return Ok(updated);
        }
    }
}
