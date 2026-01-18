using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Dtos.Planner
{
    public class UpdateDayNotesDto
    {
        public long Id { get; set; }
        public string? Notes { get; set; }
    }
}
