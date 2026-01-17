using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Dtos.Planner
{
    public class UpdateScheduleDto
    {
        public long Id { get; set; }
        public string? Notes { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
}
