using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Encounters.Core.Domain
{
    public class UserReward : Entity
    {
        public long UserId { get; private set; }
        public int Level { get; private set; }
        public string CouponCode { get; private set; }
        public int DiscountPercentage { get; private set; }
        public DateTime GrantedAt { get; private set; }
        public DateTime? ValidUntil { get; private set; }
        public bool IsUsed { get; private set; }
        public string Description { get; private set; }

        protected UserReward() { }

        public UserReward( long userId, int level, string couponCode, int discountPercentage, int validityDays, string description)
        {
            if (level <= 0)
                throw new ArgumentException("Invalid level");
            if (string.IsNullOrWhiteSpace(couponCode))
                throw new ArgumentException("Coupon code cannot be empty");
            if (discountPercentage < 0 || discountPercentage > 100)
                throw new ArgumentException("Discount percentage must be between 0 and 100");

            UserId = userId;
            Level = level;
            CouponCode = couponCode;
            DiscountPercentage = discountPercentage;
            GrantedAt = DateTime.UtcNow;
            ValidUntil = validityDays > 0 ? DateTime.UtcNow.AddDays(validityDays) : (DateTime?)null;
            IsUsed = false;
            Description = description ?? $"Level {level} reward";
        }

        public void MarkAsUsed()
        {
            if (IsUsed)
                throw new InvalidOperationException("Reward is already used");

            if (ValidUntil.HasValue && DateTime.UtcNow > ValidUntil.Value)
                throw new InvalidOperationException("Reward has expired");

            IsUsed = true;
        }

        public bool IsValid()
        {
            return !IsUsed && (!ValidUntil.HasValue || DateTime.UtcNow <= ValidUntil.Value);
        }
    }
}
