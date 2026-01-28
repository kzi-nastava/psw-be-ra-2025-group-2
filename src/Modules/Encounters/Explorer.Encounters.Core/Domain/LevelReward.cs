using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Encounters.Core.Domain
{
    public class LevelReward : Entity
    {
        public int Level { get; private set; }
        public int DiscountPercentage { get; private set; }
        public int CouponValidityDays { get; private set; }
        public string Description { get; private set; }

        private LevelReward() { }
        public LevelReward(int level, int discountPercentage, int couponValidityDays, string description)
        {
            if (level < 0) 
                throw new ArgumentException("Level must be greater than 0");
            if (discountPercentage < 0 || discountPercentage > 100)
                throw new ArgumentException("Discount percentage must be between 0 and 100");
            if (couponValidityDays <= 0)
                throw new ArgumentException("Coupon validity days must be greater than 0");

            Level = level;
            DiscountPercentage = discountPercentage;
            CouponValidityDays = couponValidityDays;
            Description = description ?? $"Level {level} Reward";
        }

        public static List<LevelReward> GetAllRewards()
        {
            return new List<LevelReward>
            {
                new LevelReward(5, 5, 30, "Congratulations! You’ve reached Level 5! 🎉"),
                new LevelReward(10, 10, 30, "Amazing! Level 10 unlocked! 🔓 You can now create challenges!"),
                new LevelReward(15, 15, 45, "Great job! Level 15 achieved! 🌟"),
                new LevelReward(20, 20, 45, "Fantastic! Level 20 is yours! 🏆"),
                new LevelReward(25, 25, 60, "Masterful! Level 25 reached! 💎"),
                new LevelReward(30, 30, 60, "LEGENDARY! Level 30 – Maximum discount! 👑")
            };
        }

        public static bool HasReward(int level)
        {
            if (GetAllRewards().Any(r => r.Level == level))
                return true;

            if (level > 30 && level % 5 == 0)
                return true;

            return false;
        }

        public static LevelReward? GetRewardForLevel(int level)
        {
            var predefined = GetAllRewards().FirstOrDefault(r => r.Level == level);
            if (predefined != null)
                return predefined;

            if (level > 30 && level % 5 == 0)
            {
                return new LevelReward(
                    level,
                    10,
                    30,
                    $"Bonus reward for Level {level}! Keep it up! 🎁"
                );
            }

            return null;
        }

        public string GenerateCouponCode(long userId)
        {
            return $"LEVEL{Level}_USER{userId}_{DateTime.UtcNow.Ticks}";
        }
    }
}
