using Explorer.Stakeholders.API.Dtos.Planner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Public
{
    public interface IPlannerService
    {
        DayEntryDto CreateScheduleEntry(long touristId, CreateScheduleDto newSchedule);
        DayEntryDto UpdateScheduleEntry(long touristId, UpdateScheduleDto newSchedule);
        IEnumerable<DayEntryDto> GetMonthlySchedule(long touristId, int month, int year);

        void RemoveScheduleEntry(long id);
    }
}
