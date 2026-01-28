using Explorer.API.Controllers.Administrator.Administration;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Encounters.API.Dtos.Encounter;
using Explorer.Encounters.API.Public;
using Explorer.Encounters.Core.Domain;
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
    public class EncounterQueryTests : BaseEncountersIntegrationTest
    {
        public EncounterQueryTests(EncountersTestFactory factory) : base(factory) { }

        [Fact]
        public void GetById_as_admin()
        {
            using (var scope = Factory.Services.CreateScope())
            {
                var controller = CreateControllerWithRole(scope, "-1", "administrator");

                var result = controller.GetById(-1).Result.ShouldBeOfType<OkObjectResult>();
                var encounter = result.Value.ShouldBeOfType<EncounterDto>();

                encounter.ShouldNotBeNull();
                encounter.Id.ShouldBe(-1);
                encounter.Name.ShouldNotBeNullOrWhiteSpace();
            }
        }

        [Fact]
        public void GetById_returns_not_found()
        {
            using (var scope = Factory.Services.CreateScope())
            {
                var controller = CreateControllerWithRole(scope, "-1", "administrator");

                controller.GetById(-999)
                    .Result
                    .ShouldBeOfType<NotFoundObjectResult>();
            }
        }

        [Fact]
        public void GetActive_as_tourist()
        {
            using (var scope = Factory.Services.CreateScope())
            {
                var controller = CreateControllerWithRole(scope, "-21", "tourist");

                var result = controller.GetActive().Result.ShouldBeOfType<OkObjectResult>();
                var encounters = result.Value.ShouldBeOfType<List<EncounterDto>>();

                encounters.ShouldNotBeEmpty();
                encounters.All(e => e.State == "Active").ShouldBeTrue();
            }
        }

        [Fact]
        public void GetActive_as_admin()
        {
            using (var scope = Factory.Services.CreateScope())
            {
                var controller = CreateControllerWithRole(scope, "-1", "administrator");

                var result = controller.GetActive().Result.ShouldBeOfType<OkObjectResult>();
                var encounters = result.Value.ShouldBeOfType<List<EncounterDto>>();

                encounters.ShouldNotBeEmpty();
                encounters.All(e => e.State == "Active").ShouldBeTrue();
            }
        }

        [Fact]
        public void GetPaged_as_admin()
        {
            using (var scope = Factory.Services.CreateScope())
            {
                var controller = CreateControllerWithRole(scope, "-1", "administrator");

                var result = controller.GetPaged(1, 10).Result.ShouldBeOfType<OkObjectResult>();
                var page = result.Value.ShouldBeOfType<PagedResult<EncounterDto>>();

                page.Results.Count.ShouldBeGreaterThan(0);
                page.TotalCount.ShouldBeGreaterThanOrEqualTo(page.Results.Count);
            }
        }

        [Fact]
        public void GetCount_as_admin()
        {
            using (var scope = Factory.Services.CreateScope())
            {
                var controller = CreateControllerWithRole(scope, "-1", "administrator");

                var result = controller.GetCount().Result.ShouldBeOfType<OkObjectResult>();
                var count = result.Value.ShouldBeOfType<int>();

                count.ShouldBeGreaterThan(0);
            }
        }


        private static EncounterController CreateControllerWithRole(
            IServiceScope scope,
            string userId,
            string role)
        {
            return new EncounterController(
                scope.ServiceProvider.GetRequiredService<IEncounterService>(),
                scope.ServiceProvider.GetRequiredService<IRewardService>())
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
