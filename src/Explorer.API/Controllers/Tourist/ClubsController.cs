using System.Collections.Generic;
using Explorer.Stakeholders.API.Dtos;      
using Explorer.Stakeholders.API.Public;    
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;

namespace Explorer.API.Controllers.Tourist;

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
        

        if (existing.OwnerId != currentUserId)
            return Forbid();

        _clubService.Delete(id);
        return NoContent();
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<ClubDto>> GetClubById(int id)
    {
        var club = _clubService.Get(id);
        if (club == null) return NotFound();
        return Ok(club);

    }

    [HttpPost("{clubId:long}/invite/{touristId:long}")]
    public IActionResult InviteTourist(long clubId, long touristId)
    {
        var ownerId = GetCurrentUserId();

        _clubService.InviteTourist(clubId, ownerId, touristId);

        return Ok("Invitation sent.");
    }

    [HttpPost("{clubId:long}/invitation/{touristId:long}/accept")]
    public IActionResult AcceptInvitation(long clubId, long touristId)
    {
        _clubService.AcceptInvitation(clubId, touristId);
        return Ok("Invitation accepted.");
    }

    [HttpPost("{clubId:long}/invitation/{touristId:long}/reject")]
    public IActionResult RejectInvitation(long clubId, long touristId)
    {
        _clubService.RejectInvitation(clubId, touristId);
        return Ok("Invitation rejected.");
    }

    [HttpDelete("{clubId:long}/members/{touristId:long}")]
    public IActionResult RemoveMember(long clubId, long touristId)
    {
        var ownerId = GetCurrentUserId();

        _clubService.RemoveMember(clubId, ownerId, touristId);

        return Ok("Member removed.");
    }

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
}