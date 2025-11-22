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
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "-100");
            var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

            var updatedProfile = new UpdatePersonProfileDto
            {
                Biography = "Nova biografija iz testa",
                Motto = "Test motto",
                ProfileImageUrl = "https://test.com/image.jpg"
            };

            // Act
            var result = ((ObjectResult)controller.UpdateProfile(updatedProfile).Result)?.Value as PersonProfileDto;

            // Assert - Response
            result.ShouldNotBeNull();
            result.Biography.ShouldBe(updatedProfile.Biography);
            result.Motto.ShouldBe(updatedProfile.Motto);
            result.ProfileImageUrl.ShouldBe(updatedProfile.ProfileImageUrl);
            result.Name.ShouldBe("Test");
            result.Surname.ShouldBe("User");
            result.Email.ShouldBe("testuser1@test.com");

            // Assert - Database
            var storedEntity = dbContext.People.FirstOrDefault(p => p.UserId == -100);
            storedEntity.ShouldNotBeNull();
            storedEntity.Biography.ShouldBe(updatedProfile.Biography);
        }

        [Fact]
        public void Updates_profile_with_null_values()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "-100");
            var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

            var updatedProfile = new UpdatePersonProfileDto
            {
                Biography = null,
                Motto = null,
                ProfileImageUrl = null
            };

            // Act
            var result = ((ObjectResult)controller.UpdateProfile(updatedProfile).Result)?.Value as PersonProfileDto;

            // Assert - Response
            result.ShouldNotBeNull();
            result.Biography.ShouldBeNull();
            result.Motto.ShouldBeNull();
            result.ProfileImageUrl.ShouldBeNull();

            // Assert - Database
            var storedEntity = dbContext.People.FirstOrDefault(p => p.UserId == -100);
            storedEntity.ShouldNotBeNull();
            storedEntity.Biography.ShouldBeNull();
            storedEntity.Motto.ShouldBeNull();
            storedEntity.ProfileImageUrl.ShouldBeNull();
        }

        [Fact]
        public void Updates_only_biography()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "-101");
            var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

            var updatedProfile = new UpdatePersonProfileDto
            {
                Biography = "Nova biografija",
                Motto = null,
                ProfileImageUrl = null
            };

            // Act
            var result = ((ObjectResult)controller.UpdateProfile(updatedProfile).Result)?.Value as PersonProfileDto;

            // Assert
            result.ShouldNotBeNull();
            result.Biography.ShouldBe("Nova biografija");
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