using Explorer.API.Controllers.Stakeholders;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Stakeholders.Tests.Integration.People
{
    [Collection("Sequential")]
    public class PersonQueryTests : BaseStakeholdersIntegrationTest
    {
        public PersonQueryTests(StakeholdersTestFactory factory) : base(factory) { }

        [Fact]
        public void Retrieves_profile_for_testuser1()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "-100");

            var result = ((ObjectResult)controller.GetProfile().Result)?.Value as PersonProfileDto;

            result.ShouldNotBeNull();
            result.UserId.ShouldBe(-100);
            result.Name.ShouldBe("Test");
            result.Surname.ShouldBe("User");
            result.Email.ShouldBe("testuser1@test.com");
        }

        [Fact]
        public void Retrieves_profile_for_testuser2()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "-101");

            var result = ((ObjectResult)controller.GetProfile().Result)?.Value as PersonProfileDto;

            result.ShouldNotBeNull();
            result.UserId.ShouldBe(-101);
            result.Name.ShouldBe("Test");
            result.Surname.ShouldBe("User2");
            result.Email.ShouldBe("testuser2@test.com");
            result.Biography.ShouldBe("Existing bio");
            result.Motto.ShouldBe("Old motto");
            result.ProfileImageUrl.ShouldBe("http://old.jpg");
        }

        [Fact]
        public void Retrieves_profile_for_testauthor()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "-102");

            var result = ((ObjectResult)controller.GetProfile().Result)?.Value as PersonProfileDto;

            result.ShouldNotBeNull();
            result.UserId.ShouldBe(-102);
            result.Name.ShouldBe("Test");
            result.Surname.ShouldBe("Author");
            result.Email.ShouldBe("testauthor@test.com");
        }

        private static PersonController CreateController(IServiceScope scope, string userId)
        {
            return new PersonController(scope.ServiceProvider.GetRequiredService<IPersonService>())
            {
                ControllerContext = BuildContext(userId)
            };
        }
    }
}
