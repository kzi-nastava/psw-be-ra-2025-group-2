using Explorer.Stakeholders.API.Public.Emergency;

namespace Explorer.Stakeholders.Tests.Integration;

public sealed class StubEmergencyPhrasebookProvider : IEmergencyPhrasebookProviderTransl
{
    private readonly List<(string Code, string Name)> _langs;
    private readonly Dictionary<string, Dictionary<string, string>> _phrases;

    public StubEmergencyPhrasebookProvider()
    {
        _langs = new()
        {
            ("en", "English"),
            ("sr", "Srpski"),
            ("de", "Deutsch")
        };

        _phrases = new(StringComparer.OrdinalIgnoreCase)
        {
            ["POL_HELP"] = new(StringComparer.OrdinalIgnoreCase)
            {
                ["en"] = "Help!",
                ["sr"] = "Upomoć!"
            },
            ["MED_CALL_AMBULANCE"] = new(StringComparer.OrdinalIgnoreCase)
            {
                ["en"] = "Call an ambulance!",
                ["sr"] = "Pozovite hitnu pomoć!"
            }
        };
    }

    public IReadOnlyList<(string Code, string Name)> GetLanguages() => _langs;

    public IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> GetAll()
        => _phrases.ToDictionary(
            x => x.Key,
            x => (IReadOnlyDictionary<string, string>)x.Value,
            StringComparer.OrdinalIgnoreCase);

    public IReadOnlyDictionary<string, string>? TryGetPhrase(string key)
        => _phrases.TryGetValue(key.Trim(), out var map) ? map : null;
}
