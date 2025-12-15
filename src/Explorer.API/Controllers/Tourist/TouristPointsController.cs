using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist
{
    [Authorize(Policy = "touristPolicy")]
    [Route("api/tourist/points")]
    [ApiController]
    public class TouristPointsController : ControllerBase
    {
        private readonly IMonumentService _monumentService;
        private readonly ITouristObjectService _touristObjectService;

        public TouristPointsController(
            IMonumentService monumentService,
            ITouristObjectService touristObjectService)
        {
            _monumentService = monumentService;
            _touristObjectService = touristObjectService;
        }

        [HttpGet("map")]
        public ActionResult<TouristMapPointsDto> GetAllMapPoints()
        {
            // Uzmi sve spomenike i objekte bez paginacije
            var monuments = _monumentService.GetPaged(0, int.MaxValue);
            var touristObjects = _touristObjectService.GetPaged(0, int.MaxValue);

            var mapPoints = new TouristMapPointsDto
            {
                Monuments = monuments.Results,
                TouristObjects = touristObjects.Results
            };

            return Ok(mapPoints);
        }
    }
}