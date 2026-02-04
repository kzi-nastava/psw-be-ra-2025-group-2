using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Dtos.Emergency
{
    public class EmergencyPhrasebookTranslateRequestDto
    {
        public string Key { get; set; } = "";
        public string SourceLang { get; set; } = ""; 
        public string TargetLang { get; set; } = "";
    }
}
