using Explorer.API.Controllers.Administrator.Administration;
using Explorer.Encounters.API.Dtos.Encounter;
using Explorer.Encounters.API.Dtos.EncounterExecution;
using Explorer.Encounters.API.Public;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Explorer.Encounters.Tests.Integration.Administration
{
    [Collection("Sequential")]
    public class EncounterExecutionTests : BaseEncountersIntegrationTest
    {
        public EncounterExecutionTests(EncountersTestFactory factory) : base(factory) { }

        [Fact]
        public void ActivateExecution_as_tourist_returns_ok()
        {
            using var scope = Factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IEncounterService>();

            var encounterId = CreateAndActivateHiddenLocationEncounter(service);
            var controller = CreateControllerWithRole(scope, "-21", "tourist");

            var result = controller.ActivateExecution(encounterId);

            result.ShouldBeOfType<OkResult>();
        }

        [Fact]
        public void ActivateExecution_fails_when_encounter_not_active()
        {
            using var scope = Factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IEncounterService>();

            // Create but DO NOT activate
            var created = service.Create(new CreateEncounterDto
            {
                Name = "HL draft",
                Description = "test",
                Latitude = 45.0001,
                Longitude = 19.0001,
                XP = 10,
                Type = "Location",
                ImageUrl = "https://example.com/test.jpg",
                ImageLatitude = 45.2525,
                ImageLongitude = 19.8625,
                DistanceTreshold = 5
            });

            var controller = CreateControllerWithRole(scope, "-21", "tourist");

            var result = controller.ActivateExecution(created.Id);

            // service should reject because encounter is not Active
            result.ShouldBeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public void PingLocation_outside_zone_resets_progress_to_zero()
        {
            using var scope = Factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IEncounterService>();

            var encounterId = CreateAndActivateHiddenLocationEncounter(service);
            var controller = CreateControllerWithRole(scope, "-21", "tourist");

            controller.ActivateExecution(encounterId).ShouldBeOfType<OkResult>();

            var dto = new EncounterLocationPingDto
            {
                Latitude = 46.0000,
                Longitude = 20.0000,
                DeltaSeconds = 10
            };

            var status = ExecutePing(controller, encounterId, dto);

            status.IsCompleted.ShouldBeFalse();
            status.SecondsInsideZone.ShouldBe(0);
            status.RequiredSeconds.ShouldBe(30);
        }

        [Fact]
        public void PingLocation_completes_after_30_seconds_in_zone()
        {
            using var scope = Factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IEncounterService>();

            var encounterId = CreateAndActivateHiddenLocationEncounter(
                service,
                imageLat: 45.2525,
                imageLon: 19.8625,
                thresholdMeters: 5
            );

            var controller = CreateControllerWithRole(scope, "-21", "tourist");
            controller.ActivateExecution(encounterId).ShouldBeOfType<OkResult>();

            var inside = new EncounterLocationPingDto
            {
                Latitude = 45.2525,
                Longitude = 19.8625,
                DeltaSeconds = 10
            };

            var s1 = ExecutePing(controller, encounterId, inside);
            s1.IsCompleted.ShouldBeFalse();
            s1.SecondsInsideZone.ShouldBe(10);

            var s2 = ExecutePing(controller, encounterId, inside);
            s2.IsCompleted.ShouldBeFalse();
            s2.SecondsInsideZone.ShouldBe(20);

            var s3 = ExecutePing(controller, encounterId, inside);
            s3.IsCompleted.ShouldBeTrue();
            s3.SecondsInsideZone.ShouldBeGreaterThanOrEqualTo(30);
            s3.RequiredSeconds.ShouldBe(30);
        }

        [Fact]
        public void PingLocation_is_idempotent_after_completion()
        {
            using var scope = Factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IEncounterService>();

            var encounterId = CreateAndActivateHiddenLocationEncounter(
                service,
                imageLat: 45.2525,
                imageLon: 19.8625,
                thresholdMeters: 5
            );

            var controller = CreateControllerWithRole(scope, "-21", "tourist");
            controller.ActivateExecution(encounterId).ShouldBeOfType<OkResult>();

            var inside = new EncounterLocationPingDto
            {
                Latitude = 45.2525,
                Longitude = 19.8625,
                DeltaSeconds = 10
            };

            // complete
            ExecutePing(controller, encounterId, inside);
            ExecutePing(controller, encounterId, inside);
            var completed = ExecutePing(controller, encounterId, inside);
            completed.IsCompleted.ShouldBeTrue();

            // ping again
            var after = ExecutePing(controller, encounterId, inside);
            after.IsCompleted.ShouldBeTrue();
            after.SecondsInsideZone.ShouldBeGreaterThanOrEqualTo(30);
            after.RequiredSeconds.ShouldBe(30);
        }

        [Fact]
        public void PingLocation_for_non_location_encounter_returns_bad_request()
        {
            using var scope = Factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IEncounterService>();

            var miscId = CreateAndActivateMiscEncounter(service);
            var controller = CreateControllerWithRole(scope, "-21", "tourist");

            var dto = new EncounterLocationPingDto
            {
                Latitude = 45.0,
                Longitude = 19.0,
                DeltaSeconds = 10
            };

            var action = controller.PingLocation(miscId, dto);

            action.Result.ShouldBeOfType<BadRequestObjectResult>();
        }

        

        private static EncounterExecutionStatusDto ExecutePing(EncounterController controller, long encounterId, EncounterLocationPingDto dto)
        {
            var action = controller.PingLocation(encounterId, dto);
            var ok = action.Result.ShouldBeOfType<OkObjectResult>();
            return ok.Value.ShouldBeOfType<EncounterExecutionStatusDto>();
        }

        private static long CreateAndActivateHiddenLocationEncounter(
            IEncounterService service,
            double imageLat = 45.0000,
            double imageLon = 19.0000,
            double thresholdMeters = 5)
        {
            var created = service.Create(new CreateEncounterDto
            {
                Name = "HL test encounter",
                Description = "test",
                Latitude = 45.0001,
                Longitude = 19.0001,
                XP = 10,
                Type = "Location",
                ImageUrl = "https://example.com/test.jpg",
                ImageLatitude = imageLat,
                ImageLongitude = imageLon,
                DistanceTreshold = thresholdMeters
            });

            service.MakeActive(created.Id);
            return created.Id;
        }

        private static long CreateAndActivateMiscEncounter(IEncounterService service)
        {
            var created = service.Create(new CreateEncounterDto
            {
                Name = "Misc test encounter",
                Description = "test",
                Latitude = 45.1,
                Longitude = 19.1,
                XP = 10,
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
    }
}
