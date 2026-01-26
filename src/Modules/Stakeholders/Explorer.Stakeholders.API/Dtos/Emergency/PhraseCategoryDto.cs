using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Dtos.Emergency
{
    public class PhraseCategoryDto
    {
        public string Category { get; set; }
        public List<PhraseDto> Phrases { get; set; } = new();
    }
}
