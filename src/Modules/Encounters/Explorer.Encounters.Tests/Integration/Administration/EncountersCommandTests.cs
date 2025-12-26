using Explorer.API.Controllers.Administrator.Administration;
using Explorer.Encounters.API.Dtos.Encounter;
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
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Encounters.Tests.Integration.Administration
{
    [Collection("Sequential")]
    public class EncounterCommandTests : BaseEncountersIntegrationTest
    {
        public EncounterCommandTests(EncountersTestFactory factory) : base(factory) { }

        [Fact]
        public void Creates()
        {
            using (var scope = Factory.Services.CreateScope())
            {
                var controller = CreateControllerWithRole(scope, "-1", "administrator");
                var dbContext = scope.ServiceProvider.GetRequiredService<EncountersContext>();

                var createDto = new CreateEncounterDto
                {
                    Name = "New encounter",
                    Description = "A difficult task",
                    Latitude = 45.0,
                    Longitude = 19.0,
                    XP = 150,
                    Type = "soCIaL"
                };

                var result = controller.Create(createDto).Result.ShouldBeOfType<OkObjectResult>();
                var encounter = result.Value.ShouldBeOfType<EncounterDto>();

                encounter.ShouldNotBeNull();
                encounter.Id.ShouldNotBe(0);
                encounter.Name.ShouldBe("New encounter");
                encounter.State.ShouldBe("Draft");

                var entity = dbContext.Encounters.Find(encounter.Id);

                entity.ShouldNotBeNull();
                entity.Name.ShouldBe("New encounter");
                entity.Description.ShouldBe("A difficult task");
                entity.State.ShouldBe(EncounterState.Draft);
                entity.Type.ShouldBe(EncounterType.Social);
            }
        }

        [Fact]
        public void Updates()
        {
            using (var scope = Factory.Services.CreateScope())
            {
                var controller = CreateControllerWithRole(scope, "-1", "administrator");
                var dbContext = scope.ServiceProvider.GetRequiredService<EncountersContext>();

                var encounter = dbContext.Encounters.First(e => e.Id == -16); // Draft

                var updateDto = new UpdateEncounterDto
                {
                    Id = encounter.Id,
                    Name = "Updated name",
                    Description = "Updated description",
                    Latitude = 44.8,
                    Longitude = 20.4,
                    XP = 200,
                    Type = "LocaTIOn"
                };

                var result = controller.Update(updateDto).Result.ShouldBeOfType<OkObjectResult>();
                var updated = result.Value.ShouldBeOfType<EncounterDto>();

                updated.Name.ShouldBe("Updated name");
                updated.Description.ShouldBe("Updated description");
                updated.State.ShouldBe("Draft");

                var entity = dbContext.Encounters.Find(encounter.Id);

                entity.ShouldNotBeNull();
                entity.Name.ShouldBe("Updated name");
                entity.Type.ShouldBe(EncounterType.Location);
            }
        }

        [Fact]
        public void Update_fails_when_not_draft()
        {
            using (var scope = Factory.Services.CreateScope())
            {
                var controller = CreateControllerWithRole(scope, "-1", "administrator");

                var updateDto = new UpdateEncounterDto
                {
                    Id = -1, // Active
                    Name = "Illegal update",
                    Description = "Should fail",
                    Latitude = 45,
                    Longitude = 19,
                    XP = 100,
                    Type = "MISCELLANEOUS"
                };

                controller.Update(updateDto).Result.ShouldBeOfType<BadRequestObjectResult>();
            }
        }

        [Fact]
        public void Activates()
        {
            using (var scope = Factory.Services.CreateScope())
            {
                var controller = CreateControllerWithRole(scope, "-1", "administrator");
                var dbContext = scope.ServiceProvider.GetRequiredService<EncountersContext>();

                controller.Activate(-17).ShouldBeOfType<OkResult>();

                var entity = dbContext.Encounters.Find(-17L);

                entity.State.ShouldBe(EncounterState.Active);
            }
        }

        [Fact]
        public void Archives()
        {
            using (var scope = Factory.Services.CreateScope())
            {
                var controller = CreateControllerWithRole(scope, "-1", "administrator");
                var dbContext = scope.ServiceProvider.GetRequiredService<EncountersContext>();

                controller.Archive(-1).ShouldBeOfType<OkResult>();

                var entity = dbContext.Encounters.Find(-1L);

                entity.State.ShouldBe(EncounterState.Archived);
            }
        }

        [Fact]
        public void Deletes()
        {
            using (var scope = Factory.Services.CreateScope())
            {
                var controller = CreateControllerWithRole(scope, "-1", "administrator");
                var dbContext = scope.ServiceProvider.GetRequiredService<EncountersContext>();

                controller.Delete(-2).ShouldBeOfType<NoContentResult>();

                dbContext.Encounters.Find(-2L).ShouldBeNull();
            }
        }

        [Fact]
        public void Tourist_can_get_active_only()
        {
            using (var scope = Factory.Services.CreateScope())
            {
                var controller = CreateControllerWithRole(scope, "-21", "tourist");

                var result = controller.GetActive().Result.ShouldBeOfType<OkObjectResult>();
                var encounters = result.Value.ShouldBeOfType<List<EncounterDto>>();

                encounters.All(e => e.State == "Active").ShouldBeTrue();
                encounters.Count().ShouldBe(9);
            }
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
