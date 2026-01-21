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
    public class FaqQueryTests : BaseStakeholdersIntegrationTest
    {
        public FaqQueryTests(StakeholdersTestFactory factory) : base(factory) { }

        [Fact]
        public void User_can_get_all_active_faq()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "-21", "tourist");

            var result = controller.Get().Result.ShouldBeOfType<OkObjectResult>();
            var faqs = result.Value.ShouldBeOfType<List<FaqItemDto>>();

            faqs.Count.ShouldBeGreaterThan(0);
            faqs.All(f => f.IsActive).ShouldBeTrue();
        }

        private static FaqController CreateController(IServiceScope scope, string id, string role)
        {
            return new FaqController(scope.ServiceProvider.GetRequiredService<IFaqService>())
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                        {
                            new Claim("id", id),
                            new Claim(ClaimTypes.Role, role)
                        }))
                    }
                }
            };
        }
    }
}
