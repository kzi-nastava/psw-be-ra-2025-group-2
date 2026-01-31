using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Explorer.API.Controllers.Tourist;
using Explorer.Encounters.API.Dtos.Encounter;
using Explorer.Encounters.API.Public;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Encounters.Tests.Integration.Tourist
{
    [Collection("Sequential")]
    public class UserRewardQueryTests : BaseEncountersIntegrationTest
    {
        public UserRewardQueryTests(EncountersTestFactory factory) : base(factory) { }

        [Fact]
        public void Tourist_can_get_their_rewards()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateControllerWithRole(scope, "-22", "tourist");

            var result = controller.GetUserRewards(-22).Result.ShouldBeOfType<OkObjectResult>();
            var rewards = result.Value.ShouldBeOfType<List<UserRewardDto>>();

            rewards.ShouldNotBeEmpty();
            rewards.All(r => r.UserId == -22).ShouldBeTrue();
        }

        [Fact]
        public void Get_reward_for_non_existing_level_returns_not_found()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateControllerWithRole(scope, "-22", "tourist");

            var result = controller.GetRewardForLevel(999).Result;
            result.ShouldBeOfType<NotFoundResult>();
        }

        private static RewardController CreateControllerWithRole(IServiceScope scope, string userId, string role)
        {
            var controller = new RewardController(scope.ServiceProvider.GetRequiredService<IRewardService>())
            {
                ControllerContext = BuildContextWithRole(userId, role)
            };
            return controller;
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
