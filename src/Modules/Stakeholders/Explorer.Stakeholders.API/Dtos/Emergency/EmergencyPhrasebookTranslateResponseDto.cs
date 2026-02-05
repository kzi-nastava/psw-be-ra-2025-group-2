using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Dtos.Emergency
{
    public class EmergencyPhrasebookTranslateResponseDto
    {
        public string Key { get; set; } = "";
        public string SourceLang { get; set; } = "";
        public string TargetLang { get; set; } = ""; 
        public string SourceText { get; set; } = "";
        public string TranslatedText { get; set; } = "";
    }
}
