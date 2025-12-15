using Explorer.Stakeholders.Infrastructure.Authentication;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
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
    }
}
