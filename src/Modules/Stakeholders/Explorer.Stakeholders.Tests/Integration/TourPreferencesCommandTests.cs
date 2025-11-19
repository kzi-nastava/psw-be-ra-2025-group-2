using System;
using System.Collections.Generic;
using System.Linq;
using Explorer.API.Controllers.Tourist;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Database;
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
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

            dbContext.TourPreferences.RemoveRange(dbContext.TourPreferences);
            dbContext.SaveChanges();

            var newEntity = new TourPreferencesDto
            {
                TouristId = 1,
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
            result.TouristId.ShouldBe(newEntity.TouristId);
            result.PreferredDifficulty.ShouldBe(newEntity.PreferredDifficulty);
            result.WalkingScore.ShouldBe(newEntity.WalkingScore);
            result.Tags.Count.ShouldBe(2);

            var stored = dbContext.TourPreferences.SingleOrDefault(tp => tp.Id == result.Id);
            stored.ShouldNotBeNull();
            stored!.TouristId.ShouldBe(newEntity.TouristId);
            ((int)stored.PreferredDifficulty).ShouldBe(newEntity.PreferredDifficulty);
            stored.WalkingScore.ShouldBe(newEntity.WalkingScore);
        }

        [Fact]
        public void Create_fails_invalid_scores()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            var invalid = new TourPreferencesDto
            {
                TouristId = 1,          // validan turistId
                PreferredDifficulty = 0,
                WalkingScore = 5,       // neispravno mora 0–3
                BicycleScore = 0,
                CarScore = 0,
                BoatScore = 0,
                Tags = new List<string>()
            };

            Should.Throw<ArgumentOutOfRangeException>(() => controller.Create(invalid));
        }

        [Fact]
        public void Updates()
        {
            long createdId;

            using (var scope = Factory.Services.CreateScope())
            {
                var controller = CreateController(scope);
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
                var controller = CreateController(scope);
                var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

                var updated = new TourPreferencesDto
                {
                    Id = createdId,
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
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            var invalid = new TourPreferencesDto
            {
                Id = -1000,          // ne postoji u bazi
                TouristId = 1,
                PreferredDifficulty = 1,
                WalkingScore = 1,
                BicycleScore = 1,
                CarScore = 1,
                BoatScore = 1,
                Tags = new List<string>()
            };

            Should.Throw<NotFoundException>(() => controller.Update(invalid));
        }

        [Fact]
        public void Deletes()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
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

            var actionResult = controller.Delete(created!.Id);   
            var okResult = actionResult as OkResult;

            okResult.ShouldNotBeNull();
            okResult!.StatusCode.ShouldBe(200);

            var stored = dbContext.TourPreferences.SingleOrDefault(tp => tp.Id == created.Id);
            stored.ShouldBeNull();
        }

        [Fact]
        public void Delete_fails_invalid_id()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            Should.Throw<NotFoundException>(() => controller.Delete(-1000));
        }

        private static TourPreferencesController CreateController(IServiceScope scope)
        {
            return new TourPreferencesController(scope.ServiceProvider.GetRequiredService<ITourPreferencesService>())
            {
                ControllerContext = BuildContext("1")
            };
        }
    }
}
