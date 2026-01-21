using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Stakeholders.Core.Domain.Help;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Stakeholders.Infrastructure.Database.Repositories
{
    public class HelpSettingsRepository : IHelpSettingsRepository
    {
        private readonly StakeholdersContext _context;

        public HelpSettingsRepository(StakeholdersContext context) 
        {
            _context = context;
        }

        public HelpSettings GetByPersonId(long personId) => _context.HelpSettings.FirstOrDefault(x =>  x.PersonId == personId);

        public HelpSettings Create(HelpSettings helpSettings) 
        {
            _context.HelpSettings.Add(helpSettings);
            _context.SaveChanges();
            return helpSettings;
        }

        public HelpSettings Update(HelpSettings helpSettings)
        {
            _context.HelpSettings.Update(helpSettings);
            _context.SaveChanges();
            return helpSettings;
        }

    }
}
