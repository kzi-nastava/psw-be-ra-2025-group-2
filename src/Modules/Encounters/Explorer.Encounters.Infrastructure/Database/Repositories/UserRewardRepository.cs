using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Encounters.Core.Domain;
using Explorer.Encounters.Core.Domain.RepositoryInterfaces;

namespace Explorer.Encounters.Infrastructure.Database.Repositories
{
    public class UserRewardRepository : IUserRewardRepository
    {
        private readonly EncountersContext _context;

        public UserRewardRepository(EncountersContext context)
        {
            _context = context;
        }

        public UserReward Create(UserReward reward)
        {
            _context.UserRewards.Add(reward);
            _context.SaveChanges();
            return reward;
        }

        public UserReward? GetByCouponCode(string couponCode)
        {
            return _context.UserRewards.FirstOrDefault(r => r.CouponCode == couponCode);
        }

        public List<UserReward> GetByUserId(long userId)
        {
            return _context.UserRewards.Where(r => r.UserId == userId).OrderByDescending(r => r.GrantedAt).ToList();
        }

        public UserReward Update(UserReward reward)
        {
            _context.UserRewards.Update(reward);
            _context.SaveChanges();
            return reward;
        }

        public bool HasReceivedRewardForLevel(long userId, int level)
        {
            return _context.UserRewards.Any(r => r.UserId == userId && r.Level == level);
        }
    }
}
