using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Stakeholders.API.Dtos.Help;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Stakeholders.Core.UseCases
{
    public class HelpSettingsService : IHelpSettingsService
    {
        private readonly IHelpSettingsRepository _repository;

        public HelpSettingsService(IHelpSettingsRepository repository)
        {
            _repository = repository;
        }

        public HelpSettingsDto GetOrCreate(long personId)
        {
            var settings = _repository.GetByPersonId(personId);
            if (settings == null)
            {
                settings = _repository.Create(new Domain.Help.HelpSettings(personId));
            }

            return new HelpSettingsDto { ShowTooltips = settings.ShowTooltips };
        }

        public HelpSettingsDto Update(long personId, bool showTooltips) 
        {
            var settings = _repository.GetByPersonId(personId);
            settings.ChangeTooltipsVisibility(showTooltips);
            _repository.Update(settings);

            return new HelpSettingsDto { ShowTooltips = settings.ShowTooltips };
        }
    }
}
