using System.Collections.Generic;
using System.Security.Claims;
using Explorer.API.Controllers.Tourist;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Explorer.Stakeholders.Tests.Integration
{
    [Collection("Sequential")]
    public class TourPreferencesQueryTests : BaseStakeholdersIntegrationTest
    {
        public TourPreferencesQueryTests(StakeholdersTestFactory factory) : base(factory) { }

        [Fact]
        public void Retrieves_by_tourist_id()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "-21");
            var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

            dbContext.TourPreferences.RemoveRange(dbContext.TourPreferences);
            dbContext.SaveChanges();

            var createDto = new TourPreferencesDto
            {
                // TouristId ce svakako biti prepisan iz tokena (-21)
                TouristId = -21,
                PreferredDifficulty = 0,
                WalkingScore = 3,
                BicycleScore = 2,
                CarScore = 1,
                BoatScore = 0,
                Tags = new List<string> { "nature", "history" }
            };

            var createResult = controller.Create(createDto);
            var createOk = createResult.Result as OkObjectResult;
            var created = createOk?.Value as TourPreferencesDto;
            created.ShouldNotBeNull();

            // novi kontroler: Get() bez parametra, koristi ulogovanog korisnika
            var actionResult = controller.Get();
            var okResult = actionResult.Result as OkObjectResult;
            var result = okResult?.Value as TourPreferencesDto;

            result.ShouldNotBeNull();
            result!.TouristId.ShouldBe(-21);
            result.WalkingScore.ShouldBe(createDto.WalkingScore);
            result.PreferredDifficulty.ShouldBe(createDto.PreferredDifficulty);
        }

        private static TourPreferencesController CreateController(IServiceScope scope, string touristId)
        {
            var controller = new TourPreferencesController(
                scope.ServiceProvider.GetRequiredService<ITourPreferencesService>());

            var user = new ClaimsPrincipal(
                new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, touristId),
                    new Claim("id", touristId),
                    new Claim(ClaimTypes.Role, "tourist")
                }, "TestAuth"));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = user
                }
            };

            return controller;
        }
    }
}
