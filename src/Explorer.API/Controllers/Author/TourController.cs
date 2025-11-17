using System.Security.Claims;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Author
{
    [ApiController]
    [Route("api/author/tours")]
    [Authorize(Policy = "authorPolicy")]
    public class TourController : ControllerBase
    {
        private readonly ITourService _tourService;

        public TourController(ITourService tourService)
        {
            _tourService = tourService;
        }

        // POST: api/author/tours
        [HttpPost]
        public ActionResult<TourDto> Create([FromBody] CreateTourDto dto)
        {
            var authorIdClaim = User.FindFirst("id");
            if (authorIdClaim == null) return Unauthorized();

            dto.AuthorId = long.Parse(authorIdClaim.Value);
            var created = _tourService.Create(dto);
            return Ok(created);
        }

        // GET: api/author/tours?authorId=5
        [HttpGet]
        public ActionResult<IEnumerable<TourDto>> GetByAuthor([FromQuery] long authorId)
        {
            var tours = _tourService.GetByAuthor(authorId);
            return Ok(tours);
        }

        // PUT: api/author/tours/{id}
        [HttpPut("{id}")]
        public ActionResult<TourDto> Update(long id, [FromBody] UpdateTourDto dto)
        {
            var updated = _tourService.Update(id, dto);
            return Ok(updated);
        }

        // DELETE: api/author/tours/{id}
        [HttpDelete("{id}")]
        public IActionResult Delete(long id)
        {
            _tourService.Delete(id);
            return NoContent();
        }
    }
}
