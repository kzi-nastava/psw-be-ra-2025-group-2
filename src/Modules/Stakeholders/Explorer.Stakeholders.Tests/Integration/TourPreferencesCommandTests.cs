using System.Collections.Generic;
using System.Linq;
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
    public class TourPreferencesCommandTests : BaseStakeholdersIntegrationTest
    {
        public TourPreferencesCommandTests(StakeholdersTestFactory factory) : base(factory) { }

        [Fact]
        public void Creates()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "1");
            var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

            dbContext.TourPreferences.RemoveRange(dbContext.TourPreferences);
            dbContext.SaveChanges();

            var newEntity = new TourPreferencesDto
            {
                TouristId = 1, // prepisan iz tokena
                PreferredDifficulty = 1,
                WalkingScore = 3,
                BicycleScore = 2,
                CarScore = 1,
                BoatScore = 0,
                Tags = new List<string> { "nature", "history" }
            };

            var actionResult = controller.Create(newEntity);
            var okResult = actionResult.Result as OkObjectResult;
            var result = okResult?.Value as TourPreferencesDto;

            result.ShouldNotBeNull();
            result!.Id.ShouldNotBe(0);
            result.TouristId.ShouldBe(1);
            result.PreferredDifficulty.ShouldBe(newEntity.PreferredDifficulty);
            result.WalkingScore.ShouldBe(newEntity.WalkingScore);
            result.Tags.Count.ShouldBe(2);

            var stored = dbContext.TourPreferences.SingleOrDefault(tp => tp.Id == result.Id);
            stored.ShouldNotBeNull();
            stored!.TouristId.ShouldBe(1);
            ((int)stored.PreferredDifficulty).ShouldBe(newEntity.PreferredDifficulty);
            stored.WalkingScore.ShouldBe(newEntity.WalkingScore);
        }

        [Fact]
        public void Create_fails_invalid_scores()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "1");

            var invalid = new TourPreferencesDto
            {
                TouristId = 1,
                PreferredDifficulty = 0,
                WalkingScore = 5,   // 0–3 ispravno, ovo je neispravno
                BicycleScore = 0,
                CarScore = 0,
                BoatScore = 0,
                Tags = new List<string>()
            };

            var actionResult = controller.Create(invalid);
            var badRequest = actionResult.Result as BadRequestObjectResult;

            badRequest.ShouldNotBeNull();
            badRequest!.StatusCode.ShouldBe(400);
            badRequest.Value.ShouldNotBeNull();
        }

        [Fact]
        public void Updates()
        {
            long createdId;

            // prvo preferences za usera 1
            using (var scope = Factory.Services.CreateScope())
            {
                var controller = CreateController(scope, "1");
                var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

                dbContext.TourPreferences.RemoveRange(dbContext.TourPreferences);
                dbContext.SaveChanges();

                var createDto = new TourPreferencesDto
                {
                    TouristId = 1,
                    PreferredDifficulty = 0,
                    WalkingScore = 1,
                    BicycleScore = 1,
                    CarScore = 1,
                    BoatScore = 1,
                    Tags = new List<string> { "old" }
                };

                var createResult = controller.Create(createDto);
                var createOk = createResult.Result as OkObjectResult;
                var created = createOk?.Value as TourPreferencesDto;
                created.ShouldNotBeNull();

                createdId = created!.Id;
            }

            using (var scope = Factory.Services.CreateScope())
            {
                var controller = CreateController(scope, "1");
                var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

                var updated = new TourPreferencesDto
                {
                    Id = createdId, // kontroler svakako setuje ID iz entiteta
                    TouristId = 1,
                    PreferredDifficulty = 2,
                    WalkingScore = 2,
                    BicycleScore = 2,
                    CarScore = 2,
                    BoatScore = 2,
                    Tags = new List<string> { "adventure", "mountains" }
                };

                var actionResult = controller.Update(updated);
                var okResult = actionResult.Result as OkObjectResult;
                var result = okResult?.Value as TourPreferencesDto;

                result.ShouldNotBeNull();
                result!.Id.ShouldBe(createdId);
                result.PreferredDifficulty.ShouldBe(updated.PreferredDifficulty);
                result.WalkingScore.ShouldBe(updated.WalkingScore);
                result.Tags.Count.ShouldBe(2);

                var stored = dbContext.TourPreferences.SingleOrDefault(tp => tp.Id == createdId);
                stored.ShouldNotBeNull();
                ((int)stored!.PreferredDifficulty).ShouldBe(updated.PreferredDifficulty);
                stored.WalkingScore.ShouldBe(updated.WalkingScore);
                stored.BoatScore.ShouldBe(updated.BoatScore);
            }
        }

        [Fact]
        public void Update_fails_invalid_id()
        {
            // nova logika: user nema preferences -> NotFound
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "1");
            var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

            dbContext.TourPreferences.RemoveRange(dbContext.TourPreferences);
            dbContext.SaveChanges();

            var invalid = new TourPreferencesDto
            {
                Id = -1000,
                TouristId = 1,
                PreferredDifficulty = 1,
                WalkingScore = 1,
                BicycleScore = 1,
                CarScore = 1,
                BoatScore = 1,
                Tags = new List<string>()
            };

            var actionResult = controller.Update(invalid);
            var notFound = actionResult.Result as NotFoundObjectResult;

            notFound.ShouldNotBeNull();
            notFound!.StatusCode.ShouldBe(404);
        }

        [Fact]
        public void Deletes()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "1");
            var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

            dbContext.TourPreferences.RemoveRange(dbContext.TourPreferences);
            dbContext.SaveChanges();

            var createDto = new TourPreferencesDto
            {
                TouristId = 1,
                PreferredDifficulty = 0,
                WalkingScore = 1,
                BicycleScore = 1,
                CarScore = 1,
                BoatScore = 1,
                Tags = new List<string> { "to-delete" }
            };

            var createResult = controller.Create(createDto);
            var createOk = createResult.Result as OkObjectResult;
            var created = createOk?.Value as TourPreferencesDto;
            created.ShouldNotBeNull();

            var actionResult = controller.Delete();
            var noContent = actionResult as NoContentResult;

            noContent.ShouldNotBeNull();
            noContent!.StatusCode.ShouldBe(204);

            var stored = dbContext.TourPreferences.SingleOrDefault(tp => tp.Id == created!.Id);
            stored.ShouldBeNull();
        }

        [Fact]
        public void Delete_when_no_preferences_still_returns_no_content()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "1");
            var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

            dbContext.TourPreferences.RemoveRange(dbContext.TourPreferences);
            dbContext.SaveChanges();

            var actionResult = controller.Delete();
            var noContent = actionResult as NoContentResult;

            noContent.ShouldNotBeNull();
            noContent!.StatusCode.ShouldBe(204);
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
