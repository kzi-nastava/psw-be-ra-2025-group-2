using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Dtos.Help
{
    public class CreateFaqItemDto
    {
        public string Category { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
    }
}
