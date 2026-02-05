using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist
{
    [ApiController]
    [Authorize(Policy = "touristPolicy")]
    [Route("api/tourist/clubs/{clubId:long}/badges")]
    public class ClubBadgesController : ControllerBase
    {
        private readonly IClubBadgeService _service;

        public ClubBadgesController(IClubBadgeService service)
        {
            _service = service;
        }

        private long GetCurrentUserId()
        {
            var idClaim = User.Claims.FirstOrDefault(c =>
                c.Type == "id" || c.Type == ClaimTypes.NameIdentifier);

            if (idClaim == null)
                throw new Exception("User id claim not found in token.");

            return long.Parse(idClaim.Value);
        }

        [HttpGet]
        public ActionResult<List<int>> Get(long clubId)
        {
            var requesterId = GetCurrentUserId();
            return Ok(_service.GetClubBadges(clubId, requesterId));
        }

        [HttpPost("recalculate")]
        public ActionResult<ClubBadgeAwardResultDto> Recalculate(long clubId, [FromQuery] int stepXp = 500)
        {
            var requesterId = GetCurrentUserId();
            return Ok(_service.RecalculateAndAward(clubId, requesterId, stepXp));
        }
    }
}