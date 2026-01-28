using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Encounters.API.Dtos.Encounter;

namespace Explorer.Encounters.API.Public
{
    public interface IRewardService
    {
        void CheckAndGrantRewards(long userId, int oldLevel, int newLevel);
        List<UserRewardDto> GetUserRewards(long userId);
        LevelRewardDto? GetRewardInfoForLevel(int level);
        List<LevelRewardDto> GetAllLevelRewards();
        void RedeemReward(long userId, string couponCode);
    }
}
