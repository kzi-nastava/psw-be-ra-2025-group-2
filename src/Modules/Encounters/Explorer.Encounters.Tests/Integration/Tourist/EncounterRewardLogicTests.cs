using Explorer.API.Controllers.Administrator.Administration;
using Explorer.Encounters.API.Dtos.Encounter;
using Explorer.Encounters.API.Public;
using Explorer.Encounters.Core.Domain;
using Explorer.Encounters.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Explorer.Encounters.Tests.Integration.Tourist
{
    [Collection("Sequential")]
    public class EncounterRewardLogicTests : BaseEncountersIntegrationTest
    {
        public EncounterRewardLogicTests(EncountersTestFactory factory) : base(factory) { }

        [Fact]
        public void CreateByTourist_Level15_Removes_All_Rewards()
        {
            // Level 15 (< 20) -> Ne sme da ima ni Turu ni Blog
            using var scope = Factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<EncountersContext>();
            var touristId = -100L; // Unique ID for test

            // 1. Setujemo korisnika na Level 15 (1600 XP)
            SetupTouristLevel(db, touristId, 1400, 15);

            var controller = CreateControllerWithRole(scope, touristId.ToString(), "tourist");

            var dto = CreateDtoWithRewards(10, 20); // Pokušava da "prošvercuje" turu(10) i blog(20)

            // Act
            var result = controller.CreateByTourist(dto).Result.ShouldBeOfType<OkObjectResult>();
            var created = result.Value.ShouldBeOfType<EncounterDto>();

            // Assert
            created.FavoriteTourId.ShouldBeNull(); // Mora biti obrisano
            created.FavoriteBlogId.ShouldBeNull(); // Mora biti obrisano
        }

        [Fact]
        public void CreateByTourist_Level25_Keeps_Tour_Removes_Blog()
        {
            // Level 25 (>= 20 ali < 30) -> Sme Turu, ne sme Blog
            using var scope = Factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<EncountersContext>();
            var touristId = -101L;

            // 1. Setujemo korisnika na Level 25
            SetupTouristLevel(db, touristId, 2400, 25);

            var controller = CreateControllerWithRole(scope, touristId.ToString(), "tourist");

            var dto = CreateDtoWithRewards(10, 20);

            // Act
            var result = controller.CreateByTourist(dto).Result.ShouldBeOfType<OkObjectResult>();
            var created = result.Value.ShouldBeOfType<EncounterDto>();

            // Assert
            created.FavoriteTourId.ShouldBe(10); // Tura ostaje
            created.FavoriteBlogId.ShouldBeNull(); // Blog se briše
        }

        [Fact]
        public void CreateByTourist_Level35_Keeps_All_Rewards()
        {
            // Level 35 (>= 30) -> Sme sve
            using var scope = Factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<EncountersContext>();
            var touristId = -102L;

            // 1. Setujemo korisnika na Level 35
            SetupTouristLevel(db, touristId, 3400, 35);

            var controller = CreateControllerWithRole(scope, touristId.ToString(), "tourist");

            var dto = CreateDtoWithRewards(10, 20);

            // Act
            var result = controller.CreateByTourist(dto).Result.ShouldBeOfType<OkObjectResult>();
            var created = result.Value.ShouldBeOfType<EncounterDto>();

            // Assert
            created.FavoriteTourId.ShouldBe(10);
            created.FavoriteBlogId.ShouldBe(20);
        }


        private void SetupTouristLevel(EncountersContext db, long userId, int xp, int level)
        {
            var existing = db.TouristProgresses.FirstOrDefault(p => p.UserId == userId);
            if (existing != null) db.TouristProgresses.Remove(existing);

            var progress = new TouristProgress(userId);

            if (xp > 0)
            {
                progress.AddXp(xp);
            }

            db.TouristProgresses.Add(progress);
            db.SaveChanges();
        }

        private CreateEncounterDto CreateDtoWithRewards(long? tourId, long? blogId)
        {
            return new CreateEncounterDto
            {
                Name = "Reward Logic Test",
                Description = "Testing levels",
                Latitude = 45.0,
                Longitude = 19.0,
                XP = 10,
                Type = "Miscellaneous",
                FavoriteTourId = tourId,
                FavoriteBlogId = blogId
            };
        }

        private static EncounterController CreateControllerWithRole(IServiceScope scope, string userId, string role)
        {
            return new EncounterController(scope.ServiceProvider.GetRequiredService<IEncounterService>(),
                                           scope.ServiceProvider.GetRequiredService<IRewardService>())
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext
                    {
                        User = new System.Security.Claims.ClaimsPrincipal(new System.Security.Claims.ClaimsIdentity(new[]
                        {
                            new System.Security.Claims.Claim("id", userId),
                            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, role)
                        }, "test"))
                    }
                }
            };
        }
    }
}