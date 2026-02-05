using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
    public class UserRewardCommandTests : BaseEncountersIntegrationTest
    {
        public UserRewardCommandTests(EncountersTestFactory factory) : base(factory) { }

        [Fact]
        public void Admin_can_create_and_redeem_reward()
        {
            using var scope = Factory.Services.CreateScope();

            var rewardController = CreateControllerWithRole(scope, "-21", "administrator");

            var rewardsBeforeResult = rewardController.GetUserRewards(-21).Result.ShouldBeOfType<OkObjectResult>();
            var rewardsBefore = rewardsBeforeResult.Value.ShouldBeOfType<List<UserRewardDto>>();
            int countBefore = rewardsBefore.Count;

            var redeemResult = rewardController.RedeemCoupon(new RedeemCouponRequestDto
            {
                UserId = -21,
                CouponCode = "WELCOME10"
            }).ShouldBeOfType<OkObjectResult>();

            redeemResult.Value.ShouldBe("Coupon redeemed successfully");

            var rewardsAfterResult = rewardController.GetUserRewards(-21).Result.ShouldBeOfType<OkObjectResult>();
            var rewardsAfter = rewardsAfterResult.Value.ShouldBeOfType<List<UserRewardDto>>();

            rewardsAfter.Count.ShouldBe(countBefore); 

            var usedReward = rewardsAfter.First(r => r.CouponCode == "WELCOME10");
            usedReward.IsUsed.ShouldBeTrue(); 
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
