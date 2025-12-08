using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Administrator;

[Authorize(Policy = "administratorPolicy")]
[Route("api/administrator/public-key-point-requests")]
[ApiController]
public class PublicKeyPointRequestController : ControllerBase
{
    private readonly IPublicKeyPointService _publicKeyPointService;

    public PublicKeyPointRequestController(IPublicKeyPointService publicKeyPointService)
    {
        _publicKeyPointService = publicKeyPointService;
    }

    [HttpGet("pending")]
    public async Task<ActionResult<IEnumerable<PublicKeyPointDto>>> GetPendingRequests()
    {
        try
        {
            var requests = await _publicKeyPointService.GetPendingRequestsAsync();
            return Ok(requests);
        }
        catch (Exception) { return StatusCode(500, new { error = "An error occurred while loading the request." }); }
    }

    [HttpPut("{publicKeyPointId}/approve")]
    public async Task<ActionResult<PublicKeyPointDto>> ApproveRequest(long publicKeyPointId)
    {
        try
        {
            var adminId = GetUserId();
            var result = await _publicKeyPointService.ApproveAsync(publicKeyPointId, adminId);
            return Ok(result);
        }
        catch (KeyNotFoundException ex) { return NotFound(new { error = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
        catch (Exception) { return StatusCode(500, new { error = "An error occurred while approving the request." }); }
    }

    [HttpPut("{publicKeyPointId}/reject")]
    public async Task<ActionResult<PublicKeyPointDto>> RejectRequest(
        long publicKeyPointId,
        [FromBody] RejectRequestDto dto)
    {
        try
        {
            var adminId = GetUserId();
            var result = await _publicKeyPointService.RejectAsync(publicKeyPointId, adminId, dto.Reason);
            return Ok(result);
        }
        catch (KeyNotFoundException ex) { return NotFound(new { error = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
        catch (Exception) { return StatusCode(500, new { error = "An error occurred while rejecting the request." }); }
    }

    private long GetUserId()
    {
        var userIdClaim = User.FindFirst("id")?.Value
            ?? throw new UnauthorizedAccessException("User ID not found in token.");

        return Math.Abs(long.Parse(userIdClaim));
    }
}

public class RejectRequestDto
{
    public string? Reason { get; set; }
}
