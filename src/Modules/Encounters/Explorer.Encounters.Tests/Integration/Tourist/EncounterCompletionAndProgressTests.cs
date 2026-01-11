using Explorer.API.Controllers.Administrator.Administration;
using Explorer.Encounters.API.Dtos.Encounter;
using Explorer.Encounters.API.Dtos.TouristProgress;
using Explorer.Encounters.API.Public;
using Explorer.Encounters.Core.Domain;
using Explorer.Encounters.Infrastructure.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Xunit;

namespace Explorer.Encounters.Tests.Integration.Tourist
{
    [Collection("Sequential")]
    public class EncounterCompletionAndProgressTests : BaseEncountersIntegrationTest
    {
        public EncounterCompletionAndProgressTests(EncountersTestFactory factory) : base(factory) { }

        [Fact]
        public void Complete_records_completion_time_awards_xp_and_updates_progress()
        {
            using var scope = Factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IEncounterService>();
            var db = scope.ServiceProvider.GetRequiredService<EncountersContext>();

            var touristId = -21L;
            ResetTourist(db, touristId);

            var encounterId = CreateAndActivateMiscEncounter(service, xp: 150);
            var controller = CreateControllerWithRole(scope, touristId.ToString(), "tourist");

            controller.Complete(encounterId).ShouldBeOfType<OkResult>();

            var execution = db.Set<EncounterExecution>()
                .FirstOrDefault(e => e.UserId == touristId && e.EncounterId == encounterId);

            execution.ShouldNotBeNull();
            execution!.IsCompleted.ShouldBeTrue();
            execution.CompletionTime.ShouldNotBeNull();
            execution.XpAwarded.ShouldBe(150);

            var progress = db.Set<TouristProgress>().First(p => p.UserId == touristId);
            progress.TotalXp.ShouldBe(150);
            progress.Level.ShouldBe(2);
            progress.CanCreateChallenges.ShouldBeFalse();
        }


        [Fact]
        public void Complete_is_idempotent_second_call_does_not_double_award_xp()
        {
            using var scope = Factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IEncounterService>();
            var db = scope.ServiceProvider.GetRequiredService<EncountersContext>();

            var touristId = -21L;

            var encounterId = CreateAndActivateMiscEncounter(service, xp: 120);
            var controller = CreateControllerWithRole(scope, touristId.ToString(), "tourist");

            controller.Complete(encounterId).ShouldBeOfType<OkResult>();

            var progressAfterFirst = db.Set<TouristProgress>().First(p => p.UserId == touristId);
            var xpAfterFirst = progressAfterFirst.TotalXp;

            controller.Complete(encounterId).ShouldBeOfType<OkResult>();

            var progressAfterSecond = db.Set<TouristProgress>().First(p => p.UserId == touristId);
            progressAfterSecond.TotalXp.ShouldBe(xpAfterFirst);

            var executionCount = db.Set<EncounterExecution>()
                .Count(e => e.UserId == touristId && e.EncounterId == encounterId);

            executionCount.ShouldBe(1);
        }

        [Fact]
        public void Tourist_reaches_level_10_and_can_create_challenges()
        {
            using var scope = Factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IEncounterService>();
            var db = scope.ServiceProvider.GetRequiredService<EncountersContext>();

            var touristId = -21L;
            var controller = CreateControllerWithRole(scope, touristId.ToString(), "tourist");

            var ids = new List<long>();
            for (int i = 0; i < 9; i++)
                ids.Add(CreateAndActivateMiscEncounter(service, xp: 100));

            foreach (var id in ids)
                controller.Complete(id).ShouldBeOfType<OkResult>();

            var progress = db.Set<TouristProgress>().First(p => p.UserId == touristId);

            progress.TotalXp.ShouldBeGreaterThanOrEqualTo(900);
            progress.Level.ShouldBeGreaterThanOrEqualTo(10);
            progress.CanCreateChallenges.ShouldBeTrue();

            // and GetMyProgress endpoint should reflect that
            var dtoResult = controller.GetMyProgress().Result.ShouldBeOfType<OkObjectResult>();
            var dto = dtoResult.Value.ShouldBeOfType<TouristProgressDto>();

            dto.Level.ShouldBeGreaterThanOrEqualTo(10);
            dto.CanCreateChallenges.ShouldBeTrue();
        }
        [Fact]
        public void CreateByTourist_fails_when_level_below_10()
        {
            using var scope = Factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<EncountersContext>();

            var touristId = -21L;
            ResetTourist(db, touristId);

            var controller = CreateControllerWithRole(scope, touristId.ToString(), "tourist");

            var createDto = new CreateEncounterDto
            {
                Name = "Tourist challenge (should fail)",
                Description = "test",
                Latitude = 45.0,
                Longitude = 19.0,
                XP = 50,
                Type = "Miscellaneous"
            };

            var action = controller.CreateByTourist(createDto).Result;

            var obj = action.ShouldBeOfType<ObjectResult>();
            obj.StatusCode.ShouldBe(StatusCodes.Status403Forbidden);
        }


        [Fact]
        public void CreateByTourist_succeeds_on_level_10_and_requires_admin_approval_to_become_active()
        {
            using var scope = Factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IEncounterService>();
            var db = scope.ServiceProvider.GetRequiredService<EncountersContext>();

            var touristId = -21L;

            // level tourist up to 10
            var touristController = CreateControllerWithRole(scope, touristId.ToString(), "tourist");
            for (int i = 0; i < 9; i++)
            {
                var id = CreateAndActivateMiscEncounter(service, xp: 100);
                touristController.Complete(id).ShouldBeOfType<OkResult>();
            }

            // tourist creates challenge
            var createDto = new CreateEncounterDto
            {
                Name = "Tourist created challenge",
                Description = "Needs admin approval",
                Latitude = 45.123,
                Longitude = 19.456,
                XP = 80,
                Type = "Miscellaneous"
            };

            var createdResult = touristController.CreateByTourist(createDto).Result.ShouldBeOfType<OkObjectResult>();
            var created = createdResult.Value.ShouldBeOfType<EncounterDto>();

            created.ShouldNotBeNull();
            created.Name.ShouldBe("Tourist created challenge");

            created.State.ShouldBe("Draft");

            var activeResult = touristController.GetActive().Result.ShouldBeOfType<OkObjectResult>();
            var actives = activeResult.Value.ShouldBeOfType<List<EncounterDto>>();

            actives.Any(e => e.Id == created.Id).ShouldBeFalse();

            var adminController = CreateControllerWithRole(scope, "-1", "administrator");
            adminController.Activate(created.Id).ShouldBeOfType<OkResult>();

            var activeAfterApprove = touristController.GetActive().Result.ShouldBeOfType<OkObjectResult>();
            var activesAfter = activeAfterApprove.Value.ShouldBeOfType<List<EncounterDto>>();

            activesAfter.Any(e => e.Id == created.Id && e.State == "Active").ShouldBeTrue();

            var entity = db.Encounters.Find(created.Id);
            entity.ShouldNotBeNull();
            entity!.State.ShouldBe(EncounterState.Active);
        }

        private static long CreateAndActivateMiscEncounter(IEncounterService service, int xp = 10)
        {
            var created = service.Create(new CreateEncounterDto
            {
                Name = "Misc completion test encounter",
                Description = "test",
                Latitude = 45.0001,
                Longitude = 19.0001,
                XP = xp,
                Type = "Miscellaneous"
            });

            service.MakeActive(created.Id);
            return created.Id;
        }

        private static EncounterController CreateControllerWithRole(
            IServiceScope scope,
            string userId,
            string role)
        {
            return new EncounterController(
                scope.ServiceProvider.GetRequiredService<IEncounterService>())
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

        private static void ResetTourist(EncountersContext db, long userId)
        {
            var execs = db.Set<EncounterExecution>().Where(e => e.UserId == userId).ToList();
            if (execs.Any()) db.RemoveRange(execs);

            var progress = db.Set<TouristProgress>().FirstOrDefault(p => p.UserId == userId);
            if (progress != null) db.Remove(progress);

            db.SaveChanges();
        }

    }
}
