using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Stakeholders.Core.Domain.Help;

namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces
{
    public interface IHelpSettingsRepository
    {
        HelpSettings GetByPersonId(long personId);
        HelpSettings Create(HelpSettings settings);
        HelpSettings Update(HelpSettings settings);
    }
}
