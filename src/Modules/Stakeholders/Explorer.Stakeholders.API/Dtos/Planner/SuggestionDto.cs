using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Dtos.Planner
{
    public class SuggestionDto
    {
        public DateOnly Date { get; set; }
        public string Kind { get; set; }
        public string Message { get; set; }
    }
}
