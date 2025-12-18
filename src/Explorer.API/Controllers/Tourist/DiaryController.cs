using System.Collections.Generic;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;

namespace Explorer.API.Controllers.Tourist
{
    [Authorize(Policy = "touristPolicy")]
    [Route("api/tourist/diaries")]
    [ApiController]
    public class DiaryController : ControllerBase
    {
        private readonly IDiaryService _diaryService;

        public DiaryController(IDiaryService diaryService)
        {
            _diaryService = diaryService;
        }

        private long GetCurrentUserId()
        {
            var idClaim = User.Claims.FirstOrDefault(c =>
                c.Type == "id" || c.Type == ClaimTypes.NameIdentifier);

            if (idClaim == null)
            {
                throw new Exception("User id claim not found in token.");
            }

            return long.Parse(idClaim.Value);
        }

        // GET: api/tourist/diaries
        [HttpGet]
        public ActionResult<List<DiaryDto>> GetAll()
        {
            var result = _diaryService.GetAll();
            return Ok(result);
        }

        // POST: api/tourist/diaries
        [HttpPost]
        public ActionResult<DiaryDto> Create([FromBody] DiaryDto dto)
        {
            var currentUserId = GetCurrentUserId();
            dto.UserId = currentUserId;
            var created = _diaryService.Create(dto);
            return Ok(created);
        }

        // PUT: api/tourist/diaries/{id}
        [HttpPut("{id:long}")]
        public ActionResult<DiaryDto> Update(long id, [FromBody] DiaryDto dto)
        {
            var currentUserId = GetCurrentUserId();
            var existing = _diaryService.Get(id);

            if (existing.UserId != currentUserId)
                return Forbid();

            dto.Id = id;
            dto.UserId = currentUserId;
            var updated = _diaryService.Update(dto);
            return Ok(updated);
        }

        // DELETE: api/tourist/diaries/{id}
        [HttpDelete("{id:long}")]
        public ActionResult Delete(long id)
        {
            var currentUserId = GetCurrentUserId();
            var existing = _diaryService.Get(id);

            if (existing.UserId != currentUserId)
                return Forbid();

            _diaryService.Delete(id);
            return NoContent();
        }

        [HttpGet("{id}")]
        public ActionResult<DiaryDto> GetDiaryById(long id)
        {
            var diary = _diaryService.Get(id);
            if (diary == null) return NotFound();
            return Ok(diary);
        }
    }
}
