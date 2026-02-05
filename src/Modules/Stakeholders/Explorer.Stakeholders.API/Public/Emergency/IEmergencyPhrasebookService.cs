using Explorer.Stakeholders.API.Dtos.Emergency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Public.Emergency
{
    public interface IEmergencyPhrasebookService
    {
        IReadOnlyList<EmergencyPhrasebookLanguageDto> GetLanguages();

        IReadOnlyList<EmergencyPhrasebookSentenceDto> GetSentences(string lang);

        EmergencyPhrasebookTranslateResponseDto Translate(EmergencyPhrasebookTranslateRequestDto request);
    }
}
