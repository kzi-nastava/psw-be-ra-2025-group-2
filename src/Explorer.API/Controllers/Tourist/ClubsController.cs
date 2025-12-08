using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
            {
                throw new Exception("User id claim not found in token.");
            }

            return long.Parse(idClaim.Value);
        }

        // ================== BASIC CRUD ==================

        // GET: api/tourist/clubs
        [HttpGet]
        public ActionResult<List<ClubDto>> GetAll()
        {
            var result = _clubService.GetAll();
            return Ok(result);
        }

        // POST: api/tourist/clubs
        [HttpPost]
        public ActionResult<ClubDto> Create([FromBody] ClubDto dto)
        {
            var currentUserId = GetCurrentUserId();

            // Ignorišemo ownerId iz body-ja, uvek postavljamo na ulogovanog korisnika
            dto.OwnerId = currentUserId;

            var created = _clubService.Create(dto);
            return Ok(created);
        }

        // PUT: api/tourist/clubs/{id}
        [HttpPut("{id:long}")]
        public ActionResult<ClubDto> Update(long id, [FromBody] ClubDto dto)
        {
            var currentUserId = GetCurrentUserId();

            var existing = _clubService.Get(id);
            if (existing == null) return NotFound();

            if (existing.OwnerId != currentUserId)
                return Forbid();

            dto.Id = id;
            dto.OwnerId = currentUserId;

            var updated = _clubService.Update(dto);
            return Ok(updated);
        }

        // DELETE: api/tourist/clubs/{id}
        [HttpDelete("{id:long}")]
        public ActionResult Delete(long id)
        {
            var currentUserId = GetCurrentUserId();

            var existing = _clubService.Get(id);
            if (existing == null) return NotFound();

            if (existing.OwnerId != currentUserId)
                return Forbid();

            _clubService.Delete(id);
            return NoContent();
        }

        [HttpGet("{id}")]
        public ActionResult<ClubDto> GetClubById(int id)
        {
            var club = _clubService.Get(id);
            if (club == null) return NotFound();
            return Ok(club);
        }

        // ================== INVITATIONS (POZIVNICE) ==================

        [HttpPost("{clubId:long}/invite/{touristId:long}")]
        public IActionResult InviteTourist(long clubId, long touristId)
        {
            var ownerId = GetCurrentUserId();

            try
            {
                _clubService.InviteTourist(clubId, ownerId, touristId);
                return Ok("Invitation sent.");
            }
            catch (KeyNotFoundException ex)
            {
                // npr. klub ne postoji
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                // klub nije aktivan, turist već član, već ima zahtev ili pozivnicu
                return BadRequest(ex.Message);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        [HttpPost("{clubId:long}/invitation/{touristId:long}/accept")]
        public IActionResult AcceptInvitation(long clubId, long touristId)
        {
            try
            {
                _clubService.AcceptInvitation(clubId, touristId);
                return Ok("Invitation accepted.");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                // "Invitation not found."
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{clubId:long}/invitation/{touristId:long}/reject")]
        public IActionResult RejectInvitation(long clubId, long touristId)
        {
            try
            {
                _clubService.RejectInvitation(clubId, touristId);
                return Ok("Invitation rejected.");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // ================== MEMBERS ==================

        [HttpDelete("{clubId:long}/members/{touristId:long}")]
        public IActionResult RemoveMember(long clubId, long touristId)
        {
            var ownerId = GetCurrentUserId();

            try
            {
                _clubService.RemoveMember(clubId, ownerId, touristId);
                return Ok("Member removed.");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                // "Member not found."
                return BadRequest(ex.Message);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        // ================== MEMBERSHIP REQUESTS (ZAHTEVI) ==================

        // Turista šalje zahtev
        [HttpPost("{clubId:long}/join-requests")]
        public IActionResult RequestMembership(long clubId)
        {
            var touristId = GetCurrentUserId();

            try
            {
                _clubService.RequestMembership(clubId, touristId);
                return Ok("Membership request sent.");
            }
            catch (KeyNotFoundException ex)
            {
                // klub ne postoji
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                // "Club is not active.", "Tourist is already a member.",
                // "Join request already exists."
                return BadRequest(ex.Message);
            }
        }

        // Turista povlači zahtev
        [HttpDelete("{clubId:long}/join-requests")]
        public IActionResult WithdrawMembershipRequest(long clubId)
        {
            var touristId = GetCurrentUserId();

            try
            {
                _clubService.WithdrawMembershipRequest(clubId, touristId);
                return Ok("Membership request withdrawn.");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                // "Join request not found."
                return BadRequest(ex.Message);
            }
        }

        // Vlasnik prihvata zahtev
        [HttpPost("{clubId:long}/join-requests/{touristId:long}/accept")]
        public IActionResult AcceptMembershipRequest(long clubId, long touristId)
        {
            var ownerId = GetCurrentUserId();

            try
            {
                _clubService.AcceptMembershipRequest(clubId, ownerId, touristId);
                return Ok("Membership request accepted.");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                // "Join request not found."
                return BadRequest(ex.Message);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        // Vlasnik odbija zahtev
        [HttpPost("{clubId:long}/join-requests/{touristId:long}/reject")]
        public IActionResult RejectMembershipRequest(long clubId, long touristId)
        {
            var ownerId = GetCurrentUserId();

            try
            {
                _clubService.RejectMembershipRequest(clubId, ownerId, touristId);
                return Ok("Membership request rejected.");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }
    }
}
