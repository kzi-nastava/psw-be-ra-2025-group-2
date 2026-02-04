using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Infrastructure.Translation.Emergency
{
    /// Reads Emergency phrasebook translations from local resources (JSON).
    /// key -> lang -> text
    public interface IEmergencyPhrasebookProvider
    {
        IReadOnlyList<(string Code, string Name)> GetLanguages();

        /// Returns all phrases in normalized structure: key -> lang -> text.
      
        IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> GetAll();

        /// Returns a single phrase map: lang -> text, or null if key doesn't exist.
        IReadOnlyDictionary<string, string>? TryGetPhrase(string key);
    }
}
