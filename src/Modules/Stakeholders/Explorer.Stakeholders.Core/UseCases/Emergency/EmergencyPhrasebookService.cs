using Explorer.Stakeholders.API.Dtos.Emergency;
using Explorer.Stakeholders.API.Public.Emergency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Core.UseCases.Emergency
{
    public sealed class EmergencyPhrasebookService : IEmergencyPhrasebookService
    {
        private readonly IEmergencyPhrasebookProviderTransl _provider;

        public EmergencyPhrasebookService(IEmergencyPhrasebookProviderTransl provider)
        {
            _provider = provider;
        }

        public IReadOnlyList<EmergencyPhrasebookLanguageDto> GetLanguages()
        {
            return _provider.GetLanguages()
                .Select(l => new EmergencyPhrasebookLanguageDto { Code = l.Code, Name = l.Name })
                .ToList();
        }

        public IReadOnlyList<EmergencyPhrasebookSentenceDto> GetSentences(string lang)
        {
            lang = NormalizeLangOrThrow(lang);

            var all = _provider.GetAll();

            // vraćamo listu rečenica prikazanu u jeziku 'lang'
            // fallback: en, pa prva dostupna
            var result = all.Select(kvp =>
            {
                var key = kvp.Key;
                var map = kvp.Value;

                var text =
                    TryGet(map, lang, out var t1) ? t1 :
                    TryGet(map, "en", out var t2) ? t2 :
                    map.Values.FirstOrDefault() ?? "";

                return new EmergencyPhrasebookSentenceDto { Key = key, Text = text };
            })
            .OrderBy(x => x.Key)
            .ToList();

            return result;
        }

        public EmergencyPhrasebookTranslateResponseDto Translate(EmergencyPhrasebookTranslateRequestDto request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (string.IsNullOrWhiteSpace(request.Key)) throw new ArgumentException("Key is required.");

            var sourceLang = NormalizeLangOrThrow(request.SourceLang);
            var targetLang = NormalizeLangOrThrow(request.TargetLang);

            var map = _provider.TryGetPhrase(request.Key.Trim());
            if (map == null)
                throw new KeyNotFoundException($"Unknown phrase key: {request.Key}");

            var sourceText =
                TryGet(map, sourceLang, out var s1) ? s1 :
                TryGet(map, "en", out var s2) ? s2 :
                map.Values.FirstOrDefault() ?? "";

            var translatedText =
                TryGet(map, targetLang, out var tr) ? tr : sourceText;

            return new EmergencyPhrasebookTranslateResponseDto
            {
                Key = request.Key.Trim(),
                SourceLang = sourceLang,
                TargetLang = targetLang,
                SourceText = sourceText,
                TranslatedText = translatedText
            };
        }

        private string NormalizeLangOrThrow(string lang)
        {
            lang = (lang ?? "").Trim().ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(lang)) lang = "en";

            var supported = _provider.GetLanguages().Select(x => x.Code).ToHashSet(StringComparer.OrdinalIgnoreCase);
            if (!supported.Contains(lang))
                throw new ArgumentException($"Unsupported language: {lang}");

            return lang;
        }

        private static bool TryGet(IReadOnlyDictionary<string, string> map, string lang, out string value)
        {
            foreach (var kv in map)
            {
                if (string.Equals(kv.Key, lang, StringComparison.OrdinalIgnoreCase))
                {
                    value = kv.Value;
                    return true;
                }
            }
            value = "";
            return false;
        }
    }
}