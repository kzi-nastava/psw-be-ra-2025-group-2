using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Encounters.API.Dtos.Encounter;
using Explorer.Encounters.API.Internal;
using Explorer.Encounters.API.Public;
using Explorer.Encounters.Core.Domain;
using Explorer.Encounters.Core.Domain.RepositoryInterfaces;

namespace Explorer.Encounters.Core.UseCases
{
    public class RewardService : IRewardService
    {
        private readonly IUserRewardRepository _rewardRepository;
        private readonly IInternalRewardService _internalRewardService;

        public RewardService( IUserRewardRepository rewardRepository, IInternalRewardService internalRewardService = null)
        {
            _rewardRepository = rewardRepository;
            _internalRewardService = internalRewardService;
        }

        public void CheckAndGrantRewards(long userId, int oldLevel, int newLevel)
        {
            for (int level = oldLevel + 1; level <= newLevel; level++)
            {
                if (!LevelReward.HasReward(level))
                    continue;

                if (_rewardRepository.HasReceivedRewardForLevel(userId, level))
                    continue;

                GrantReward(userId, level);
            }
        }

        private void GrantReward(long userId, int level)
        {
            var rewardInfo = LevelReward.GetRewardForLevel(level);
            if (rewardInfo == null)
                return;

            var couponCode = rewardInfo.GenerateCouponCode(userId);

            var userReward = new UserReward(
                userId,
                level,
                couponCode,
                rewardInfo.DiscountPercentage,
                rewardInfo.CouponValidityDays,
                rewardInfo.Description
            );

            _rewardRepository.Create(userReward);

            if (_internalRewardService != null)
            {
                _internalRewardService.GrantCoupon(
                    userId,
                    rewardInfo.DiscountPercentage,
                    DateTime.UtcNow.AddDays(rewardInfo.CouponValidityDays),
                    rewardInfo.Description
                );
            }


        }

        public List<UserRewardDto> GetUserRewards(long userId)
        {
            var rewards = _rewardRepository.GetByUserId(userId);

            return rewards.Select(r => new UserRewardDto
            {
                UserId = r.UserId,
                Level = r.Level,
                CouponCode = r.CouponCode,
                DiscountPercentage = r.DiscountPercentage,
                GrantedAt = r.GrantedAt,
                ValidUntil = r.ValidUntil,
                IsUsed = r.IsUsed,
                Description = r.Description
            }).ToList();
        }

        public LevelRewardDto? GetRewardInfoForLevel(int level)
        {
            var reward = LevelReward.GetRewardForLevel(level);
            if (reward == null)
                return null;

            return new LevelRewardDto
            {
                Level = reward.Level,
                DiscountPercentage = reward.DiscountPercentage,
                CouponValidityDays = reward.CouponValidityDays,
                Description = reward.Description
            };
        }

        public List<LevelRewardDto> GetAllLevelRewards()
        {
            var rewards = LevelReward.GetAllRewards();

            return rewards.Select(r => new LevelRewardDto
            {
                Level = r.Level,
                DiscountPercentage = r.DiscountPercentage,
                CouponValidityDays = r.CouponValidityDays,
                Description = r.Description
            }).ToList();
        }

        public void RedeemReward(long userId, string couponCode)
        {
            var rewards = _rewardRepository.GetByUserId(userId);

            var reward = rewards.FirstOrDefault(r => r.CouponCode == couponCode);
            if (reward == null)
                throw new Exception("Coupon not found");

            if (reward.IsUsed)
                throw new Exception("Coupon is already used");

            if (reward.ValidUntil.HasValue && reward.ValidUntil.Value < DateTime.UtcNow)
                throw new Exception("Coupon has expired");

            reward.MarkAsUsed();
            _rewardRepository.Update(reward);
        }

    }
}
