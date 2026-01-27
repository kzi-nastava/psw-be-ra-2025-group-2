using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Explorer.Stakeholders.API.Dtos.Emergency;

namespace Explorer.Stakeholders.API.Public.Emergency
{
    public interface IEmergencyTranslationService
    {
        EmergencyTranslationResponseDto Translate(EmergencyTranslationRequestDto request);
    }
}
