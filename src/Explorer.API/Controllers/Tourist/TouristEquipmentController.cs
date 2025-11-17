using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.Core.UseCases.Administration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist
{
    [Authorize(Policy = "touristPolicy")]
    [Route("api/tourist/equipment")]
    [ApiController]
    public class TouristEquipmentController : ControllerBase
    {
        private readonly ITouristEquipmentService _service;

        public TouristEquipmentController(ITouristEquipmentService service)
        {
            _service = service;
        }

        [HttpPost]
        public ActionResult<TouristEquipmentDto> Create([FromBody] TouristEquipmentDto equipment)
        {
            return Ok(_service.Create(equipment));
        }

        [HttpPut]
        public ActionResult<TouristEquipmentDto> Update([FromBody] TouristEquipmentDto equipment)
        {
            return Ok(_service.Update(equipment));
        }

        [HttpGet("{id:long}")]
        public ActionResult<TouristEquipmentDto> Get(int id)
        {
            return Ok(_service.GetTouristEquipment(id));
        }
    }
}
