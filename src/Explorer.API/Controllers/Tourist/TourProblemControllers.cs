using System.Collections.Generic;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist
{
    [Authorize(Policy = "touristPolicy")]
    [Route("api/tourist/tour-problems")]
    [ApiController]
    public class TourProblemController : ControllerBase
    {
        private readonly ITourProblemService _tourProblemService;

        public TourProblemController(ITourProblemService tourProblemService)
        {
            _tourProblemService = tourProblemService;
        }

        [HttpGet]
        public ActionResult<List<TourProblemDto>> GetForCreator()
        {
            return Ok(_tourProblemService.GetForCreator());
        }

        [HttpPost]
        public ActionResult<TourProblemDto> Create([FromBody] CreateTourProblemDto problem)
        {
            return Ok(_tourProblemService.Create(problem));
        }

        [HttpPut("{id:long}")]
        public ActionResult<TourProblemDto> Update([FromBody] TourProblemDto problem)
        {
            return Ok(_tourProblemService.Update(problem));
        }

        [HttpDelete("{id:long}")]
        public ActionResult Delete(long id)
        {
            _tourProblemService.Delete(id);
            return Ok();
        }
    }
}