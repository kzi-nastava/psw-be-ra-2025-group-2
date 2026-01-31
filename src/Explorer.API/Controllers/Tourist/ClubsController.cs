using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace Explorer.API.Controllers.Tourist
{
    [Authorize(Policy = "touristPolicy")]
    [Route("api/tourist/clubs")]
    [ApiController]
    public class ClubsController : ControllerBase
    {
        private readonly IClubService _clubService;

        public ClubsController(IClubService clubService)
        {
            _clubService = clubService;
        }

        private long GetCurrentUserId()
        {
            var idClaim = User.Claims.FirstOrDefault(c =>
                c.Type == "id" || c.Type == ClaimTypes.NameIdentifier);

            if (idClaim == null)
                throw new Exception("User id claim not found in token.");

            return long.Parse(idClaim.Value);
        }

        // BASIC CRUD
        [HttpGet]
        public ActionResult<List<ClubDto>> GetAll() => Ok(_clubService.GetAll());

        [HttpGet("{id:long}")]
        public ActionResult<ClubDto> GetClubById(long id)
        {
            var club = _clubService.Get(id);
            return club == null ? NotFound() : Ok(club);
        }

        [HttpPost]
        public ActionResult<ClubDto> Create([FromBody] ClubDto dto)
        {
            dto.OwnerId = GetCurrentUserId();
            return Ok(_clubService.Create(dto));
        }

        [HttpPut("{id:long}")]
        public ActionResult<ClubDto> Update(long id, [FromBody] ClubDto dto)
        {
            var currentUserId = GetCurrentUserId();
            var existing = _clubService.Get(id);
            if (existing == null) return NotFound();
            if (existing.OwnerId != currentUserId) return Forbid();

            dto.Id = id;
            dto.OwnerId = currentUserId;

            return Ok(_clubService.Update(dto));
        }

        [HttpDelete("{id:long}")]
        public ActionResult Delete(long id)
        {
            var currentUserId = GetCurrentUserId();
            var existing = _clubService.Get(id);
            if (existing == null) return NotFound();
            if (existing.OwnerId != currentUserId) return Forbid();

            _clubService.Delete(id);
            return NoContent();
        }

        // INVITE (owner)
        [HttpPost("{clubId:long}/invite/{touristId:long}")]
        public IActionResult InviteTourist(long clubId, long touristId)
        {
            var ownerId = GetCurrentUserId();
            _clubService.InviteTourist(clubId, ownerId, touristId);
            return Ok("Invitation sent.");
        }

        // INVITATION accept/reject (tourist from token)
        [HttpPost("{clubId:long}/invitation/accept")]
        public IActionResult AcceptInvitation(long clubId)
        {
            var touristId = GetCurrentUserId();
            _clubService.AcceptInvitation(clubId, touristId);
            return Ok("Invitation accepted.");
        }

        [HttpPost("{clubId:long}/invitation/reject")]
        public IActionResult RejectInvitation(long clubId)
        {
            var touristId = GetCurrentUserId();
            _clubService.RejectInvitation(clubId, touristId);
            return Ok("Invitation rejected.");
        }

        // MEMBERS (owner)
        [HttpDelete("{clubId:long}/members/{touristId:long}")]
        public IActionResult RemoveMember(long clubId, long touristId)
        {
            var ownerId = GetCurrentUserId();
            _clubService.RemoveMember(clubId, ownerId, touristId);
            return Ok("Member removed.");
        }

        // JOIN REQUESTS (tourist sends/withdraws)
        [HttpPost("{clubId:long}/join-requests")]
        public IActionResult RequestMembership(long clubId)
        {
            var touristId = GetCurrentUserId();
            _clubService.RequestMembership(clubId, touristId);
            return Ok("Membership request sent.");
        }

        [HttpDelete("{clubId:long}/join-requests")]
        public IActionResult WithdrawMembershipRequest(long clubId)
        {
            var touristId = GetCurrentUserId();
            _clubService.WithdrawMembershipRequest(clubId, touristId);
            return Ok("Membership request withdrawn.");
        }

        // JOIN REQUESTS (owner accepts/rejects specific tourist)
        [HttpPost("{clubId:long}/join-requests/{touristId:long}/accept")]
        public IActionResult AcceptMembershipRequest(long clubId, long touristId)
        {
            var ownerId = GetCurrentUserId();
            _clubService.AcceptMembershipRequest(clubId, ownerId, touristId);
            return Ok("Membership request accepted.");
        }

        [HttpPost("{clubId:long}/join-requests/{touristId:long}/reject")]
        public IActionResult RejectMembershipRequest(long clubId, long touristId)
        {
            var ownerId = GetCurrentUserId();
            _clubService.RejectMembershipRequest(clubId, ownerId, touristId);
            return Ok("Membership request rejected.");
        }

        // LISTS
        [HttpGet("{clubId:long}/invitable-tourists")]
        public ActionResult<List<InvitableTouristDto>> GetInvitableTourists(long clubId, [FromQuery] string? q)
        {
            var ownerId = GetCurrentUserId();
            return Ok(_clubService.GetInvitableTourists(clubId, ownerId, q));
        }

        [HttpGet("{clubId:long}/join-requests/list")]
        public ActionResult<List<TouristBasicDto>> GetJoinRequestsList(long clubId)
        {
            var ownerId = GetCurrentUserId();
            return Ok(_clubService.GetJoinRequests(clubId, ownerId));
        }

        [HttpGet("{clubId:long}/members/list")]
        public ActionResult<List<TouristBasicDto>> GetMembersList(long clubId)
        {
            var ownerId = GetCurrentUserId();
            return Ok(_clubService.GetMembers(clubId, ownerId));
        }

        // MY CLUB IDS
        [HttpGet("my-invitations")]
        public ActionResult<List<long>> GetMyInvitations()
        {
            var touristId = GetCurrentUserId();
            return Ok(_clubService.GetMyInvitationClubIds(touristId));
        }

        [HttpGet("my-memberships")]
        public ActionResult<List<long>> GetMyMemberships()
        {
            var touristId = GetCurrentUserId();
            return Ok(_clubService.GetMyMembershipClubIds(touristId));
        }

        [HttpGet("my-join-requests")]
        public ActionResult<List<long>> GetMyJoinRequests()
        {
            var touristId = GetCurrentUserId();
            return Ok(_clubService.GetMyJoinRequestClubIds(touristId));
        }

        // UPLOAD
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("upload-image")]
        [RequestSizeLimit(10_000_000)]
        public async Task<ActionResult<string>> UploadImage([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0) return BadRequest("File is required.");

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            if (!allowed.Contains(ext)) return BadRequest("Unsupported file type.");

            var fileName = $"{Guid.NewGuid()}{ext}";
            var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "clubs-images");
            Directory.CreateDirectory(folder);

            var fullPath = Path.Combine(folder, fileName);
            await using var stream = System.IO.File.Create(fullPath);
            await file.CopyToAsync(stream);

            return Ok($"/clubs-images/{fileName}");
        }
    }
}
