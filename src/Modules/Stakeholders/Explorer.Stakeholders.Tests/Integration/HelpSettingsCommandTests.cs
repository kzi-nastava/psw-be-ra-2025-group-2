using Explorer.API.Controllers.Stakeholders;
using Explorer.Stakeholders.API.Dtos.Help;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Security.Claims;

namespace Explorer.Stakeholders.Tests.Integration.Help
{
    [Collection("Sequential")]
    public class HelpSettingsCommandTests : BaseStakeholdersIntegrationTest
    {
        public HelpSettingsCommandTests(StakeholdersTestFactory factory) : base(factory) { }

        [Fact]
        public void User_can_toggle_tooltips()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "-21", "tourist");
            var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

            var getResult = controller.Get().Result.ShouldBeOfType<OkObjectResult>();
            var settings = getResult.Value.ShouldBeOfType<HelpSettingsDto>();
            settings.ShowTooltips.ShouldBeTrue();

            var updateDto = new UpdateHelpSettingsDto { ShowTooltips = false };
            var updateResult = controller.Update(updateDto).Result.ShouldBeOfType<OkObjectResult>();
            var updatedSettings = updateResult.Value.ShouldBeOfType<HelpSettingsDto>();
            updatedSettings.ShowTooltips.ShouldBeFalse();

            var dbSettings = dbContext.HelpSettings.Single(h => h.PersonId == -21);
            dbSettings.ShowTooltips.ShouldBeFalse();
        }

        private static HelpSettingsController CreateController(IServiceScope scope, string id, string role)
        {
            return new HelpSettingsController(scope.ServiceProvider.GetRequiredService<IHelpSettingsService>())
            {
                ControllerContext = BuildContext(id, role)
            };
        }

        private static ControllerContext BuildContext(string id, string role)
        {
            var claims = new[] { new Claim("personId", id), new Claim(ClaimTypes.Role, role) };
            return new ControllerContext { HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity(claims)) } };
        }
    }
}
