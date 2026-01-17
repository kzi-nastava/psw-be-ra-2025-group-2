using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Dtos.Planner
{
    public class ScheduleEntryDto
    {
        public long Id { get; set; }
        public long TourId { get; set; }
        public long TourName { get; set; }
        public string? Notes { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
}
