using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.Infrastructure.Authentication;

namespace Explorer.API.Controllers.Tourist
{
    [Authorize(Policy = "touristPolicy")]
    [Route("api/tourist/diary")]
    [ApiController]
    public class DiaryController : ControllerBase
    {
        private readonly IDiaryService _diaryService;

        public DiaryController(IDiaryService diaryService)
        {
            _diaryService = diaryService;
        }

        [HttpGet("user")]
        public ActionResult<List<DiaryDto>> GetByUser()
        {
            var result = _diaryService.GetByUserId(User.PersonId());
            return Ok(result);
        }

        [HttpGet("{id:long}")]
        public ActionResult<DiaryDto> Get(long id)
        {
            var result = _diaryService.Get(id);
            return Ok(result);
        }

        [HttpPost]
        public ActionResult<DiaryDto> Create([FromBody] DiaryDto diary)
        {
            var result = _diaryService.Create(diary);
            return Ok(result);
        }

        [HttpPut("{id:long}")]
        public ActionResult<DiaryDto> Update([FromBody] DiaryDto diary)
        {
            var result = _diaryService.Update(diary);
            return Ok(result);
        }

        [HttpDelete("{id:long}")]
        public ActionResult Delete(long id)
        {
            _diaryService.Delete(id);
            return Ok();
        }
    }
}
