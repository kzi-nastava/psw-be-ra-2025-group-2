using Explorer.Stakeholders.API.Dtos.Planner;
using Explorer.Stakeholders.Core.Domain.Planner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces
{
    public interface IPlannerRepository
    {
        List<ScheduleEntry> GetMonthlyScheduleEntries(long touristId, int month, int year);
        List<ScheduleEntry> GetDailyScheduleEntries(long touristId, int day, int month, int year);

        IEnumerable<DayEntry> GetByMonth(long touristId, int month, int year);
        DayEntry? GetById(long id);
        DayEntry? GetByScheduleEntryId(long scheduleEntryId);
        DayEntry? GetByDate(long touristId, DateOnly date);
        DayEntry Create(DayEntry entry);
        DayEntry Update(DayEntry entry);
        void Delete(long id);
    }
}
