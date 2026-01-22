using Explorer.API.Controllers.Administrator.Administration;
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
    public class FaqCommandTests : BaseStakeholdersIntegrationTest
    {
        public FaqCommandTests(StakeholdersTestFactory factory) : base(factory) { }

        [Fact]
        public void Admin_can_create_update_delete_faq()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "-1", "admin");
            var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

            var createDto = new CreateFaqItemDto { Category = "Payments", Question = "Test?", Answer = "Yes." };
            var createResult = controller.Create(createDto).Result.ShouldBeOfType<OkObjectResult>();
            var createdFaq = createResult.Value.ShouldBeOfType<FaqItemDto>();
            createdFaq.Id.ShouldNotBe(0);

            var updateDto = new UpdateFaqItemDto { Question = "Updated?", Answer = "No." };
            var updateResult = controller.Update(createdFaq.Id, updateDto).Result.ShouldBeOfType<OkObjectResult>();
            var updatedFaq = updateResult.Value.ShouldBeOfType<FaqItemDto>();
            updatedFaq.Question.ShouldBe("Updated?");
            updatedFaq.Answer.ShouldBe("No.");

            controller.Deactivate(createdFaq.Id).ShouldBeOfType<NoContentResult>();
            dbContext.FaqItems.Find(createdFaq.Id)!.IsActive.ShouldBeFalse();
        }

        private static AdminFaqController CreateController(IServiceScope scope, string id, string role)
        {
            return new AdminFaqController(scope.ServiceProvider.GetRequiredService<IFaqService>())
            {
                ControllerContext = BuildContext(id, role)
            };
        }

        private static ControllerContext BuildContext(string id, string role)
        {
            var claims = new[] { new Claim("id", id), new Claim(ClaimTypes.Role, role) };
            return new ControllerContext { HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity(claims)) } };
        }
    }
}
