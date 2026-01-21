using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.API.Controllers.Stakeholders;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Shouldly;

namespace Explorer.Stakeholders.Tests.Integration
{
    [Collection("Sequential")]
    public class OnboardingTests : BaseStakeholdersIntegrationTest
    {
        public OnboardingTests(StakeholdersTestFactory factory) : base(factory) { }

        [Theory]
        [InlineData(0, 2)]
        [InlineData(1, 1)]
        public void Gets_slides_by_role(int role, int expectedCount)
        {
            using (var scope = Factory.Services.CreateScope())
            {
                var controller = CreateController(scope);

                var actionResult = controller.GetSlides(role);
                var result = actionResult.Result.ShouldBeOfType<OkObjectResult>();

                var slides = result.Value.ShouldBeOfType<List<OnboardingSlideDto>>();

                slides.Count.ShouldBe(expectedCount);

                foreach (var s in slides)
                {
                    ((int)s.Role).ShouldBe(role);
                }
            }
        }

        [Fact]
        public void Gets_empty_list_for_non_existing_role()
        {
            using (var scope = Factory.Services.CreateScope())
            {
                // Arrange
                var controller = CreateController(scope);

                // Act 
                var result = controller.GetSlides(99).Result.ShouldBeOfType<OkObjectResult>();
                var slides = result.Value.ShouldBeOfType<List<OnboardingSlideDto>>();

                // Assert
                slides.ShouldBeEmpty();
            }
        }

        private static OnboardingController CreateController(IServiceScope scope)
        {
            return new OnboardingController(scope.ServiceProvider.GetRequiredService<IOnboardingService>())
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };
        }
    }
}
