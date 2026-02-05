using Explorer.Encounters.API.Dtos.Encounter;
using Explorer.Encounters.API.Public;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist
{
    [ApiController]
    [Route("api/[controller]")]
    public class RewardController : ControllerBase
    {
        private readonly IRewardService _rewardService;

        public RewardController(IRewardService rewardService)
        {
            _rewardService = rewardService;
        }

        [HttpGet("levels")]
        public ActionResult<List<LevelRewardDto>> GetAllLevelRewards()
        {
            var rewards = _rewardService.GetAllLevelRewards();
            return Ok(rewards);
        }

        [HttpGet("level/{level}")]
        public ActionResult<LevelRewardDto?> GetRewardForLevel(int level)
        {
            var reward = _rewardService.GetRewardInfoForLevel(level);
            if (reward == null)
                return NotFound();
            return Ok(reward);
        }

        [HttpGet("user/{userId}")]
        public ActionResult<List<UserRewardDto>> GetUserRewards(long userId)
        {
            var userRewards = _rewardService.GetUserRewards(userId);
            return Ok(userRewards);
        }

        [HttpPost("redeem")]
        public ActionResult RedeemCoupon([FromBody] RedeemCouponRequestDto request)
        {
            try
            {
                _rewardService.RedeemReward(request.UserId, request.CouponCode);
                return Ok("Coupon redeemed successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
