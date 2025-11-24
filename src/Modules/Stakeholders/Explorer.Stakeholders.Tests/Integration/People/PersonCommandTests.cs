using Explorer.API.Controllers.Stakeholders;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Stakeholders.Tests.Integration.People
{
    [Collection("Sequential")]
    public class PersonCommandTests : BaseStakeholdersIntegrationTest
    {
        public PersonCommandTests(StakeholdersTestFactory factory) : base(factory) { }

        [Fact]
        public void Updates_own_profile()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "-100");
            var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

            var updatedProfile = new PersonProfileDto
            {
                Name = "Test",
                Surname = "User",
                Biography = "Nova biografija iz testa",
                Motto = "Test motto",
                ProfileImageUrl = "https://test.com/image.jpg"
            };

            var result = ((ObjectResult)controller.UpdateProfile(updatedProfile).Result)?.Value as PersonProfileDto;

            result.ShouldNotBeNull();
            result.Name.ShouldBe("Test");
            result.Surname.ShouldBe("User");
            result.Biography.ShouldBe("Nova biografija iz testa");
            result.Motto.ShouldBe("Test motto");
            result.ProfileImageUrl.ShouldBe("https://test.com/image.jpg");

            var storedEntity = dbContext.People.FirstOrDefault(p => p.UserId == -100);
            storedEntity.ShouldNotBeNull();
            storedEntity.Biography.ShouldBe("Nova biografija iz testa");
        }

        [Fact]
        public void Updates_profile_with_null_values()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "-100");
            var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

            var updatedProfile = new PersonProfileDto
            {
                Name = "Test",
                Surname = "User",
                Biography = null,
                Motto = null,
                ProfileImageUrl = null
            };

            var result = ((ObjectResult)controller.UpdateProfile(updatedProfile).Result)?.Value as PersonProfileDto;

            result.ShouldNotBeNull();
            result.Name.ShouldBe("Test");
            result.Surname.ShouldBe("User");
            result.Biography.ShouldBeNull();
            result.Motto.ShouldBeNull();
            result.ProfileImageUrl.ShouldBeNull();

            var storedEntity = dbContext.People.FirstOrDefault(p => p.UserId == -100);
            storedEntity.ShouldNotBeNull();
            storedEntity.Biography.ShouldBeNull();
        }

        [Fact]
        public void Updates_only_biography()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "-101");
            var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

            var updatedProfile = new PersonProfileDto
            {
                Name = "Test",
                Surname = "User2",
                Biography = "Nova biografija"
            };

            var result = ((ObjectResult)controller.UpdateProfile(updatedProfile).Result)?.Value as PersonProfileDto;

            result.ShouldNotBeNull();
            result.Biography.ShouldBe("Nova biografija");

            var storedEntity = dbContext.People.FirstOrDefault(p => p.UserId == -101);
            storedEntity.ShouldNotBeNull();
            storedEntity.Biography.ShouldBe("Nova biografija");
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
