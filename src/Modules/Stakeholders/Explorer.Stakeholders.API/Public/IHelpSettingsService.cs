using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Stakeholders.API.Dtos.Help;

namespace Explorer.Stakeholders.API.Public
{
    public interface IHelpSettingsService
    {
        HelpSettingsDto GetOrCreate(long personId);
        HelpSettingsDto Update(long personId, bool showTooltips);
    }
}
