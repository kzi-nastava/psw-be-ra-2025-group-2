using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Public.Emergency
{
    /// Reads Emergency phrasebook translations from local resources (JSON).
    /// key -> lang -> text
    public interface IEmergencyPhrasebookProviderTransl
    {
        IReadOnlyList<(string Code, string Name)> GetLanguages();
      
        IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> GetAll();

        IReadOnlyDictionary<string, string>? TryGetPhrase(string key);
    }
}
