using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Explorer.Stakeholders.API.Public.Emergency;


namespace Explorer.Stakeholders.Infrastructure.Translation.Emergency
{
    public sealed class EmergencyPhrasebookProviderTransl : IEmergencyPhrasebookProviderTransl
    {
        private readonly IHostEnvironment _env;
        private readonly ILogger<EmergencyPhrasebookProviderTransl> _logger;

        // Lazy-loaded cache
        private bool _loaded;
        private readonly object _lock = new();

        private List<(string Code, string Name)> _languages = new();
        private Dictionary<string, Dictionary<string, string>> _phrases =
            new(StringComparer.OrdinalIgnoreCase);

        public EmergencyPhrasebookProviderTransl(IHostEnvironment env, ILogger<EmergencyPhrasebookProviderTransl> logger)
        {
            _env = env;
            _logger = logger;
        }

        public IReadOnlyList<(string Code, string Name)> GetLanguages()
        {
            EnsureLoaded();
            return _languages;
        }

        public IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> GetAll()
        {
            EnsureLoaded();

            // expose as read-only views
            return _phrases.ToDictionary(
                kvp => kvp.Key,
                kvp => (IReadOnlyDictionary<string, string>)kvp.Value,
                StringComparer.OrdinalIgnoreCase
            );
        }

        public IReadOnlyDictionary<string, string>? TryGetPhrase(string key)
        {
            EnsureLoaded();

            if (string.IsNullOrWhiteSpace(key)) return null;

            return _phrases.TryGetValue(key.Trim(), out var map)
                ? (IReadOnlyDictionary<string, string>)map
                : null;
        }

        // ----------------- internals -----------------

        private void EnsureLoaded()
        {
            if (_loaded) return;

            lock (_lock)
            {
                if (_loaded) return;

                var basePath = _env.ContentRootPath;

                // Resources folder inside this project:
                // Explorer.Stakeholders.Infrastructure/Translation/Emergency/Resources
                var resourcesPath = Path.Combine(
                    basePath,
                    "Translation",
                    "Emergency",
                    "Resources"
                );

                var languagesPath = Path.Combine(resourcesPath, "languages.json");
                var phrasebookPath = Path.Combine(resourcesPath, "phrasebook.json");

                _languages = LoadLanguages(languagesPath);
                _phrases = LoadPhrasebook(phrasebookPath);

                ValidateLoadedData();

                _logger.LogInformation(
                    "Emergency phrasebook loaded: {Keys} keys, {Langs} languages, from {ResourcesPath}",
                    _phrases.Count,
                    _languages.Count,
                    resourcesPath
                );

                _loaded = true;
            }
        }

        private void ValidateLoadedData()
        {
            if (_languages.Count == 0)
                throw new InvalidOperationException("Emergency phrasebook: languages.json is empty or invalid.");

            if (_phrases.Count == 0)
                throw new InvalidOperationException("Emergency phrasebook: phrasebook.json is empty or invalid.");

            // Ensure no empty codes
            if (_languages.Any(x => string.IsNullOrWhiteSpace(x.Code)))
            {
                throw new InvalidOperationException(
                    "Emergency phrasebook: languages.json contains an empty language code."
                );
            }


            // Normalize: keep only known languages (optional but helpful)
            var known = _languages.Select(l => l.Code).ToHashSet(StringComparer.OrdinalIgnoreCase);

            foreach (var key in _phrases.Keys.ToList())
            {
                var map = _phrases[key];

                // remove empty lang keys
                var emptyLangKeys = map.Keys.Where(k => string.IsNullOrWhiteSpace(k)).ToList();
                foreach (var k in emptyLangKeys) map.Remove(k);

                // optional: remove unknown languages
                var unknown = map.Keys.Where(k => !known.Contains(k)).ToList();
                foreach (var k in unknown) map.Remove(k);

                // remove empty texts
                var emptyTexts = map.Where(p => string.IsNullOrWhiteSpace(p.Value)).Select(p => p.Key).ToList();
                foreach (var k in emptyTexts) map.Remove(k);

                if (map.Count == 0)
                    _logger.LogWarning("Emergency phrasebook key {Key} has no valid translations after normalization.", key);
            }
        }

        private static List<(string Code, string Name)> LoadLanguages(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException($"Emergency phrasebook: languages.json not found at '{path}'.");

            var json = File.ReadAllText(path);

            var items = JsonSerializer.Deserialize<List<LanguageJson>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            ) ?? new List<LanguageJson>();

            return items
                .Where(x => !string.IsNullOrWhiteSpace(x.Code))
                .Select(x => (x.Code.Trim().ToLowerInvariant(), (x.Name ?? "").Trim()))
                .ToList();
        }

        private static Dictionary<string, Dictionary<string, string>> LoadPhrasebook(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException($"Emergency phrasebook: phrasebook.json not found at '{path}'.");

            var json = File.ReadAllText(path);

            var data = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            return data ?? new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);
        }

        private sealed class LanguageJson
        {
            public string? Code { get; set; }
            public string? Name { get; set; }
        }
    }
}
