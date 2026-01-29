using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.Infrastructure.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Security.Claims;
using Xunit;
using Explorer.API.Controllers.Tourist;
using Explorer.Stakeholders.API.Public.Emergency;


using Explorer.Stakeholders.API.Dtos.Emergency;

namespace Explorer.Stakeholders.Tests.Integration
{
    [Collection("Sequential")]
    public class EmergencyOverviewQueryTests : BaseStakeholdersIntegrationTest
    {
        public EmergencyOverviewQueryTests(StakeholdersTestFactory factory) : base(factory) { }

        [Fact]
        public void GetOverview_RS_returns_data()
        {
            using var scope = Factory.Services.CreateScope();

            var controller = CreateControllerWithRole(scope, "-21", "tourist");
            var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

           
            dbContext.EmergencyDirectories.Count().ShouldBeGreaterThan(0);
            dbContext.EmergencyPlaces.Count().ShouldBeGreaterThan(0);

            var result = controller.GetOverview("RS").ShouldBeOfType<OkObjectResult>();
            var dto = result.Value.ShouldBeOfType<EmergencyOverviewResponseDto>();

            dto.CountryCode.ShouldBe("RS");
            dto.Hospitals.Count.ShouldBeGreaterThan(0);
            dto.PoliceStations.Count.ShouldBeGreaterThan(0);

            dto.Embassies.ShouldNotBeNull();
            dto.Embassies.Count.ShouldBeGreaterThan(0);

            dto.PhraseCategories.ShouldNotBeNull();
            dto.PhraseCategories.Count.ShouldBe(2);

            // Medicina category exists and has data
            dto.PhraseCategories.Any(c => c.Category == "Medicina").ShouldBeTrue();
            dto.PhraseCategories.First(c => c.Category == "Medicina").Phrases.ShouldNotBeNull();
            dto.PhraseCategories.First(c => c.Category == "Medicina").Phrases.Count.ShouldBeGreaterThan(0);

            // Police category exists and has data
            dto.PhraseCategories.Any(c => c.Category == "Policija").ShouldBeTrue();
            dto.PhraseCategories.First(c => c.Category == "Policija").Phrases.ShouldNotBeNull();
            dto.PhraseCategories.First(c => c.Category == "Policija").Phrases.Count.ShouldBeGreaterThan(0);

        }

        [Fact]
        public void GetOverview_DE_returns_empty_but_has_texts()
        {
            using var scope = Factory.Services.CreateScope();

            var controller = CreateControllerWithRole(scope, "-21", "tourist");

            // Act
            var result = controller.GetOverview("DE").ShouldBeOfType<OkObjectResult>();
            var dto = result.Value.ShouldBeOfType<EmergencyOverviewResponseDto>();

            // Assert
            dto.CountryCode.ShouldBe("DE");
            dto.Hospitals.Count.ShouldBe(0);
            dto.PoliceStations.Count.ShouldBe(0);
            dto.Instructions.ShouldNotBeNullOrWhiteSpace();
            dto.Disclaimer.ShouldNotBeNullOrWhiteSpace();

            dto.Embassies.ShouldNotBeNull();
            dto.Embassies.Count.ShouldBe(0);

            dto.PhraseCategories.ShouldNotBeNull();
            dto.PhraseCategories.Count.ShouldBe(2);

            dto.PhraseCategories.Any(c => c.Category == "Medicina").ShouldBeTrue();
            dto.PhraseCategories.First(c => c.Category == "Medicina").Phrases.Count.ShouldBe(0);

            dto.PhraseCategories.Any(c => c.Category == "Policija").ShouldBeTrue();
            dto.PhraseCategories.First(c => c.Category == "Policija").Phrases.Count.ShouldBe(0);

        }

        private static EmergencyOverviewController CreateControllerWithRole(IServiceScope scope, string userId, string role)
        {
            return new EmergencyOverviewController(
                scope.ServiceProvider.GetRequiredService<IEmergencyOverviewService>(),
                scope.ServiceProvider.GetRequiredService<IEmergencyTranslationService>()
            )
            {
                ControllerContext = BuildContextWithRole(userId, role)
            };
        }


        private static ControllerContext BuildContextWithRole(string id, string role)
        {
            var claims = new List<Claim>
            {
                new Claim("id", id),
                new Claim(ClaimTypes.Role, role)
            };

            var identity = new ClaimsIdentity(claims, "test");
            var user = new ClaimsPrincipal(identity);

            return new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }
    }
}
