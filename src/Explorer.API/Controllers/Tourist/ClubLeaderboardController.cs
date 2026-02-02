using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist
{
    [Authorize]
    [Route("api/clubs")]
    public class ClubLeaderboardController : ControllerBase
    {
        private readonly IClubLeaderboardService _leaderboardService;

        public ClubLeaderboardController(IClubLeaderboardService leaderboardService)
        {
            _leaderboardService = leaderboardService;
        }

        [HttpGet("{clubId:long}/leaderboard")]
        public ActionResult<List<ClubLeaderboardRowDto>> GetLeaderboard(long clubId)
        {
            var requesterId = long.Parse(User.Claims.First(c => c.Type == "id").Value);
            var res = _leaderboardService.GetLeaderboard(clubId, requesterId);
            return Ok(res);
        }


        [HttpGet("leaderboard")]
        public ActionResult<List<ClubLeaderboardClubRowDto>> GetClubsLeaderboard()
        {
            var requesterId = long.Parse(User.Claims.First(c => c.Type == "id").Value);
            var res = _leaderboardService.GetClubsLeaderboard(requesterId);
            return Ok(res);
        }
        
    }
}
