using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Dtos.Planner
{
    public class DayEntryDto
    {
        public long Id { get; set; }
        public long TouristId { get; set; }
        public DateOnly Date { get; set; }
        public string? Notes { get; set; }
        public IEnumerable<ScheduleEntryDto> Entries { get; set; }
    }
}
