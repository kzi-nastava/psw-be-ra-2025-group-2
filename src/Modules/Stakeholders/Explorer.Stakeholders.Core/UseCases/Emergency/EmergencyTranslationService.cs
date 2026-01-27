using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Explorer.Stakeholders.API.Dtos.Emergency;
using Explorer.Stakeholders.API.Public.Emergency;
using Explorer.Stakeholders.Core.Domain.Emergency;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Stakeholders.Core.UseCases.Emergency
{
    public class EmergencyTranslationService : IEmergencyTranslationService
    {
        private readonly IEmergencyDirectoryRepository _repo;

        public EmergencyTranslationService(IEmergencyDirectoryRepository repo)
        {
            _repo = repo;
        }

        public EmergencyTranslationResponseDto Translate(EmergencyTranslationRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Text))
            {
                return new EmergencyTranslationResponseDto
                {
                    TranslatedText = "No text provided for translation.",
                    IsFallback = true
                };
            }

            EmergencyDirectory? directory = null;
            try
            {
                var code = new CountryCode(request.CountryCode);
                directory = _repo.GetByCountry(code);
            }
            catch
            {
                return new EmergencyTranslationResponseDto
                {
                    TranslatedText = "Invalid country code.",
                    IsFallback = true
                };
            }

            if (directory == null)
            {
                return new EmergencyTranslationResponseDto
                {
                    TranslatedText = "Translation not available for selected country.",
                    IsFallback = true
                };
            }

            // MVP: bez pravog prevodioca -> vrati isti tekst, ali jasno označi fallback
            // (kasnije ovde ubaci pravi prevod ili mapiranje po jeziku)
            return new EmergencyTranslationResponseDto
            {
                TranslatedText = request.Text.Trim(),
                IsFallback = true
            };
        }
    }
}
