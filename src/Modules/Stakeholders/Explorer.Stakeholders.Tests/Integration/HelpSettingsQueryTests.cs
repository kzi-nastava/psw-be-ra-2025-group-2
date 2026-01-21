using Explorer.API.Controllers.Stakeholders;
using Explorer.Stakeholders.API.Dtos.Help;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Security.Claims;

namespace Explorer.Stakeholders.Tests.Integration.Help
{
    [Collection("Sequential")]
    public class HelpSettingsQueryTests : BaseStakeholdersIntegrationTest
    {
        public HelpSettingsQueryTests(StakeholdersTestFactory factory) : base(factory) { }

        [Fact]
        public void User_can_get_own_helpsettings()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "-21", "tourist");

            var result = controller.Get().Result.ShouldBeOfType<OkObjectResult>();
            var settings = result.Value.ShouldBeOfType<HelpSettingsDto>();

            settings.ShowTooltips.ShouldBeTrue();
        }

        private static HelpSettingsController CreateController(IServiceScope scope, string id, string role)
        {
            return new HelpSettingsController(scope.ServiceProvider.GetRequiredService<IHelpSettingsService>())
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                        {
                            new Claim("personId", id),
                            new Claim(ClaimTypes.Role, role)
                        }))
                    }
                }
            };
        }
    }
}
