using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Explorer.Stakeholders.API.Dtos.Emergency;
using Explorer.Stakeholders.API.Public.Emergency;
using Explorer.Stakeholders.Core.Domain.Emergency;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

using System.Net.Http.Json;
using System.Text.Json;

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

            try
            {
                using var http = new HttpClient
                {
                    BaseAddress = new Uri("http://127.0.0.1:5000")
                };

                var body = new
                {
                    q = request.Text.Trim(),
                    source = request.SourceLanguage?.Trim().ToLowerInvariant(),
                    target = request.TargetLanguage?.Trim().ToLowerInvariant(),
                    format = "text"
                };

                var resp = http.PostAsJsonAsync("/translate", body).GetAwaiter().GetResult();
                resp.EnsureSuccessStatusCode();

                var json = resp.Content.ReadFromJsonAsync<JsonElement>().GetAwaiter().GetResult();

                // LibreTranslate response: { "translatedText": "...", "alternatives": [...] }
                var translated = json.TryGetProperty("translatedText", out var tt) ? tt.GetString() : null;

                if (!string.IsNullOrWhiteSpace(translated))
                {
                    return new EmergencyTranslationResponseDto
                    {
                        TranslatedText = translated!,
                        IsFallback = false
                    };
                }
            }
            catch
            {
                // ignore -> fallback below
            }

            return new EmergencyTranslationResponseDto
            {
                TranslatedText = request.Text.Trim(),
                IsFallback = true
            };

        }
    }
}
