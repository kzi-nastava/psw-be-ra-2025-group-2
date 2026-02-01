using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Encounters.Core.Domain;
using Shouldly;

namespace Explorer.Encounters.Tests.Unit
{
    public class UserRewardUnitTests
    {
        [Fact]
        public void Can_create_user_reward_and_validate_properties()
        {
            var reward = new UserReward(1, 1, "COUPON123", 20, 7, "Test reward");

            reward.UserId.ShouldBe(1);
            reward.Level.ShouldBe(1);
            reward.CouponCode.ShouldBe("COUPON123");
            reward.DiscountPercentage.ShouldBe(20);
            reward.IsUsed.ShouldBeFalse();
            reward.ValidUntil.ShouldNotBeNull();
            reward.IsValid().ShouldBeTrue();
        }

        [Fact]
        public void MarkAsUsed_works_and_fails_when_already_used()
        {
            var reward = new UserReward(1, 1, "COUPON123", 20, 7, "Test reward");

            reward.MarkAsUsed();
            reward.IsUsed.ShouldBeTrue();
            Should.Throw<InvalidOperationException>(() => reward.MarkAsUsed());
        }

        [Fact]
        public void IsValid_returns_false_when_expired()
        {
            var reward = new UserReward(1, 1, "COUPON123", 20, -1, "Expired reward");
            typeof(UserReward).GetProperty("GrantedAt")!.SetValue(reward, DateTime.UtcNow.AddDays(-10));
            typeof(UserReward).GetProperty("ValidUntil")!.SetValue(reward, DateTime.UtcNow.AddDays(-1));
            reward.IsValid().ShouldBeFalse();
        }
    }
}
