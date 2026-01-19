using System.Security.Claims;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Encounters.API.Dtos.Encounter;
using Explorer.Encounters.API.Public;

namespace Explorer.API.Controllers.Author
{
    [Authorize(Policy = "authorPolicy")]
    [Route("api/author/tours")]
    [ApiController]
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

            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentNullException(nameof(dto.Name), "Name is required.");

            if (dto.Difficulty < 1 || dto.Difficulty > 5)
                throw new ArgumentOutOfRangeException(nameof(dto.Difficulty), "Difficulty must be between 1 and 5.");

            var created = _tourService.Create(dto);
            return Ok(created);
        }

        // GET: api/author/tours?authorId=-11
        [HttpGet]
        public ActionResult<IEnumerable<TourDto>> GetByAuthor([FromQuery] long authorId)
        {
            try
            {
                var tours = _tourService.GetByAuthor(authorId);
                return Ok(tours);
            }
            catch (Exception ex)
            {
                // Ovo će ti pokazati tačnu grešku
                var innerMsg = ex.InnerException?.Message ?? "No inner exception";
                var stackTrace = ex.StackTrace ?? "No stack trace";

                return StatusCode(500, new
                {
                    error = ex.Message,
                    innerError = innerMsg,
                    type = ex.GetType().Name,
                    stack = stackTrace
                });
            }
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<TourDto>> GetById(long id)
        {
            var authorIdClaim = User.FindFirst("id");
            if (authorIdClaim == null) return Unauthorized();

            long authorId = long.Parse(authorIdClaim.Value);

            var tour = await _tourService.GetByIdAsync(id, authorId);
            if (tour == null) return NotFound();

            return Ok(tour);
        }

        // PUT: api/author/tours/{id}
        [HttpPut("{id}")]
        public ActionResult<TourDto> Update(long id, [FromBody] UpdateTourDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentNullException(nameof(dto.Name), "Name is required.");

            if (dto.Difficulty < 1 || dto.Difficulty > 5)
                throw new ArgumentOutOfRangeException(nameof(dto.Difficulty), "Difficulty must be between 1 and 5.");

            try
            {
                var updated = _tourService.Update(id, dto);
                return Ok(updated);
            }
            catch (InvalidOperationException ex) // npr. tura je arhivirana
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Došlo je do neočekivane greške prilikom ažuriranja ture." });
            }
        }

        // DELETE: api/author/tours/{id}
        [HttpDelete("{id}")]
        public IActionResult Delete(long id)
        {
            _tourService.Delete(id);
            return NoContent();
        }

        // POST: api/author/tours/{tourId}/keypoints
        [HttpPost("{tourId}/keypoints")]
        public async Task<ActionResult<KeyPointDto>> AddKeyPoint(long tourId, [FromBody] KeyPointDto dto)
        {
            try
            {
                var authorIdClaim = User.FindFirst("id");
                if (authorIdClaim == null) return Unauthorized();

                dto.AuthorId = long.Parse(authorIdClaim.Value);

                var result = await _tourService.AddKeyPoint(tourId, dto);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // PUT: api/author/tours/{tourId}/keypoints/{ordinalNo}
        [HttpPut("{tourId}/keypoints/{ordinalNo}")]
        public async Task<ActionResult<KeyPointDto>> UpdateKeyPoint(long tourId, int ordinalNo, [FromBody] KeyPointDto dto)
        {
            try
            {
                var authorIdClaim = User.FindFirst("id");
                if (authorIdClaim == null) return Unauthorized();

                dto.AuthorId = long.Parse(authorIdClaim.Value);

                var result = await _tourService.UpdateKeyPoint(tourId, ordinalNo, dto);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An unexpected error occurred. Please try again." });
            }
        }

        // DELETE: api/author/tours/{tourId}/keypoints/{ordinalNo}
        [HttpDelete("{tourId}/keypoints/{ordinalNo}")]
        public ActionResult RemoveKeyPoint(long tourId, int ordinalNo)
        {
            try
            {
                _tourService.RemoveKeyPoint(tourId, ordinalNo);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "An error occurred while deleting the keypoint." });
            }
        }

        [HttpPut("{id}/publish")]
        public IActionResult Publish(long id)
        {
            var authorIdClaim = User.FindFirst("id");
            if (authorIdClaim == null) return Unauthorized();

            long authorId = long.Parse(authorIdClaim.Value);

            try
            {
                _tourService.Publish(id, authorId);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Unexpected error while publishing tour." });
            }
        }

        // POST: api/author/tours/{id}/archive
        [HttpPost("{id}/archive")]
        public IActionResult Archive(long id)
        {
            try
            {
                _tourService.Archive(id);
                return Ok(new { message = "Tura je uspešno arhivirana." });
            }
            catch (AggregateException ex) when (ex.InnerException is InvalidOperationException || ex.InnerException is DomainException)
            {
                // async .Result je zamotao našu domensku grešku u AggregateException
                return BadRequest(new { message = ex.InnerException!.Message });
            }
            catch (InvalidOperationException ex)
            {
                // direktan slučaj, ako se ikad desi
                return BadRequest(new { message = ex.Message });
            }
            catch (DomainException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                // ostale neočekivane greške
                return StatusCode(500, new { message = "Došlo je do greške prilikom arhiviranja ture." });
            }
        }

        // POST: api/author/tours/{id}/reactivate
        [HttpPost("{id}/reactivate")]
        public IActionResult Reactivate(long id)
        {
            try
            {
                _tourService.Reactivate(id);
                return Ok(new { message = "Tura je uspešno ponovo aktivirana." });
            }
            catch (InvalidOperationException ex)
            {
                // npr. tura nije arhivirana
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Došlo je do greške prilikom reaktivacije ture." });
            }
        }

        // GetByRange: 
        [HttpGet("search/{lat:double}/{lon:double}/{range:int}")]
        [AllowAnonymous]
        public ActionResult<PagedResult<TourDto>> GetByRange (double lat, double lon, int range, [FromQuery]int page, [FromQuery] int pageSize)
        {
            var result = _tourService.GetByRange(lat, lon, range, page, pageSize);
            return Ok(result);
        }
        
        //GET api/author/{tourId}/equipment
        [HttpGet("{tourId}/equipment")]
        public ActionResult<List<TourEquipmentItemDto>> GetEquipmentForTour(long tourId)
        {
            var authorIdClaim = User.FindFirst("id");
            if (authorIdClaim == null) return Unauthorized();

            long authorId = long.Parse(authorIdClaim.Value);

            try
            {
                var result = _tourService.GetEquipmentForTour(tourId, authorId);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        
        //PUT api/author/{tourId}/equipment
        [HttpPut("{tourId}/equipment")]
        public IActionResult UpdateEquipmentForTour(long tourId, [FromBody] UpdateTourEquipmentDto dto)
        {
            var authorIdClaim = User.FindFirst("id");
            if (authorIdClaim == null) return Unauthorized();

            long authorId = long.Parse(authorIdClaim.Value);

            try
            {
                _tourService.UpdateEquipmentForTour(tourId, authorId, dto.EquipmentIds);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        
        // GET api/author/tours/equipment
        [HttpGet("equipment")]
        public ActionResult<List<TourEquipmentItemDto>> GetAllEquipmentForAuthor()
        {
            var authorId = long.Parse(User.FindFirst("id")!.Value);
            var result = _tourService.GetAllEquipmentForAuthor(authorId);
            return Ok(result);
        }

        [HttpPost("{tourId}/keypoints/{keyPointOrdinalNo}/encounter")]
        public async Task<ActionResult<EncounterDto>> CreateEncounterFromKeyPoint(
            long tourId,
            int keyPointOrdinalNo,
            [FromBody] CreateEncounterDto encounterDto)
        {
            try
            {
                var authorIdClaim = User.FindFirst("id");
                if (authorIdClaim == null) return Unauthorized();

                long authorId = long.Parse(authorIdClaim.Value);

                var tour = await _tourService.GetByIdAsync(tourId, authorId);
                if (tour == null)
                    return NotFound(new { error = "Tour not found" });

                var keyPoint = tour.KeyPoints?.FirstOrDefault(kp => kp.OrdinalNo == keyPointOrdinalNo);

                if (keyPoint != null)
                {
                    encounterDto.Latitude = keyPoint.Latitude;
                    encounterDto.Longitude = keyPoint.Longitude;
                }
                else
                {
                    if (encounterDto.Latitude == 0 || encounterDto.Longitude == 0)
                    {
                        return BadRequest(new { error = "Latitude and Longitude are required when creating encounter for new KeyPoint" });
                    }
                }

                var encounterService = HttpContext.RequestServices.GetRequiredService<IEncounterService>();
                var createdEncounter = encounterService.Create(encounterDto);
                encounterService.MakeActive(createdEncounter.Id);

                return Ok(createdEncounter);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("keypoints/upload-image")]
        [RequestSizeLimit(10_000_000)] // 10MB
        public async Task<ActionResult<string>> UploadKeyPointImage([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0) return BadRequest("File is required.");

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            if (!allowed.Contains(ext)) return BadRequest("Unsupported file type.");

            var fileName = $"{Guid.NewGuid()}{ext}";
            var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "keypoints-images");
            Directory.CreateDirectory(folder);

            var fullPath = Path.Combine(folder, fileName);
            await using var stream = System.IO.File.Create(fullPath);
            await file.CopyToAsync(stream);

            return Ok($"/keypoints-images/{fileName}");
        }

        [HttpPost("{tourId}/keypoints/{ordinalNo}/images")]
        [RequestSizeLimit(30_000_000)]
        public async Task<ActionResult<KeyPointDto>> UploadKeyPointImages(long tourId, int ordinalNo, [FromForm] List<IFormFile> files)
        {
            var authorIdClaim = User.FindFirst("id");
            if (authorIdClaim == null) return Unauthorized();

            if (files == null || files.Count == 0) return BadRequest("Files are required.");

            var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var urls = new List<string>();

            foreach (var file in files)
            {
                if (file == null || file.Length == 0) return BadRequest("File is required.");

                var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!allowed.Contains(ext)) return BadRequest("Unsupported file type.");

                var fileName = $"{Guid.NewGuid()}{ext}";
                var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "keypoints-images");
                Directory.CreateDirectory(folder);

                var fullPath = Path.Combine(folder, fileName);
                await using var stream = System.IO.File.Create(fullPath);
                await file.CopyToAsync(stream);

                urls.Add($"/keypoints-images/{fileName}");
            }

            var authorId = long.Parse(authorIdClaim.Value);
            var result = await _tourService.AddKeyPointImages(tourId, ordinalNo, authorId, urls);
            return Ok(result);
        }

        [HttpPut("{tourId}/cover-image")]
        public async Task<IActionResult> SetTourCoverImage(long tourId, [FromBody] SetCoverImageDto dto)
        {
            var authorIdClaim = User.FindFirst("id");
            if (authorIdClaim == null) return Unauthorized();

            var authorId = long.Parse(authorIdClaim.Value);
            await _tourService.SetCoverImage(tourId, authorId, dto.Url);
            return NoContent();
        }

        [HttpDelete("{tourId}/keypoints/{ordinalNo}/images/{imageId:long}")]
        public async Task<IActionResult> DeleteKeyPointImage(long tourId, int ordinalNo, long imageId)
        {
            var authorIdClaim = User.FindFirst("id");
            if (authorIdClaim == null) return Unauthorized();

            var authorId = long.Parse(authorIdClaim.Value);

            try
            {
                await _tourService.RemoveKeyPointImage(tourId, ordinalNo, authorId, imageId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while deleting the image." });
            }
        }
    }

}
