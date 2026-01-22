using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Stakeholders.Core.Domain.Help;
using Shouldly;

namespace Explorer.Stakeholders.Tests.Unit
{
    public class HelpSettingsUnitTests
    {
        [Fact]
        public void Creates_with_tooltips_enabled()
        {
            var settings = new HelpSettings(1);

            settings.PersonId.ShouldBe(1);
            settings.ShowTooltips.ShouldBeTrue();
        }

        [Fact]
        public void Changes_tooltip_visibility()
        {
            var settings = new HelpSettings(1);

            settings.ChangeTooltipsVisibility(false);

            settings.ShowTooltips.ShouldBeFalse();
        }
    }
}
