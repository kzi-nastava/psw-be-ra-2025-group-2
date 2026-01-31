using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Encounters.Core.Domain.RepositoryInterfaces
{
    public interface IUserRewardRepository
    {
        UserReward Create(UserReward reward);
        UserReward? GetByCouponCode(string couponCode);
        List<UserReward> GetByUserId(long userId);
        UserReward Update(UserReward reward);
        bool HasReceivedRewardForLevel(long userId, int level);
    }
}
