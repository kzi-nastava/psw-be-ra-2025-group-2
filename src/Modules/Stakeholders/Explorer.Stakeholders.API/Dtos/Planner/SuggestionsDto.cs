using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Dtos.Planner
{
    public class SuggestionsDto
    {
        public int Month { get; set; }
        public int? Day { get; set; }
        List<string> Suggestions { get; set; } = new();
    }
}
