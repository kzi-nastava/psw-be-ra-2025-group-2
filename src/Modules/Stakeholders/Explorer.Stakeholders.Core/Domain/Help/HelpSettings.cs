using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain.Help
{
    public class HelpSettings : AggregateRoot
    {
        public long PersonId { get; private set; }
        public bool ShowTooltips { get; private set; }

        public HelpSettings(long personId)
        {
            PersonId = personId;
            ShowTooltips = true;
        }

        public void ChangeTooltipsVisibility(bool value)
        {
            ShowTooltips = value;
        }
    }
}
