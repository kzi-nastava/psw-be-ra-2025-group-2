using Explorer.Stakeholders.API.Dtos.Planner;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist
{
    [Route("api/planner")]
    [ApiController]
    [Authorize(Policy = "touristPolicy")]
    public class PlannerController : ControllerBase
    {
        private readonly IPlannerService _plannerService;

        public PlannerController(IPlannerService plannerService)
        {
            _plannerService = plannerService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<DayEntryDto>> GetMonthlySchedule([FromQuery] int year, [FromQuery] int month)
        {
            var touristId = User.UserId();
            var result = _plannerService.GetMonthlySchedule(touristId, month, year);
            return Ok(result);
        }

        [HttpPost]
        public ActionResult<DayEntryDto> CreateScheduleEntry([FromBody] CreateScheduleDto createDto)
        {
            var touristId = User.UserId();
            var result = _plannerService.CreateScheduleEntry(touristId, createDto);
            return Ok(result);
        }

        [HttpPut]
        public ActionResult<DayEntryDto> UpdateScheduleEntry([FromBody] UpdateScheduleDto updateDto)
        {
            var touristId = User.UserId();
            var result = _plannerService.UpdateScheduleEntry(touristId, updateDto);
            return Ok(result);
        }

        [HttpPut("day-notes")]
        public ActionResult<DayEntryDto> UpdateDayNotes([FromBody] UpdateDayNotesDto notesDto)
        {
            var touristId = User.UserId();
            var result = _plannerService.UpdateDayNotes(touristId, notesDto);
            return Ok(result);
        }

        [HttpDelete("{id:long}")]
        public ActionResult RemoveScheduleEntry(long id)
        {
            _plannerService.RemoveScheduleEntry(id);
            return Ok();
        }
    }
}
