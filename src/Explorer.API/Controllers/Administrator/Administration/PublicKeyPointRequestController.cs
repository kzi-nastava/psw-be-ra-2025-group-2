using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Administrator;

[Authorize(Policy = "administratorPolicy")]
[Route("api/admin/public-key-point-requests")]
[ApiController]
public class PublicKeyPointRequestController : ControllerBase
{
    private readonly IPublicKeyPointService _service;

    public PublicKeyPointRequestController(IPublicKeyPointService service)
    {
        _service = service;
    }

    [HttpGet("pending")]
    public async Task<ActionResult<IEnumerable<PublicKeyPointRequestDto>>> GetPending()
    {
        var requests = await _service.GetPendingRequestsAsync();
        return Ok(requests);
    }

    [HttpPut("{requestId}/approve")]
    public async Task<ActionResult<PublicKeyPointRequestDto>> Approve(long requestId)
    {
        try
        {
            var adminId = long.Parse(User.FindFirst("id")?.Value ?? "0");
            var result = await _service.ApproveRequestAsync(requestId, adminId);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{requestId}/reject")]
    public async Task<ActionResult<PublicKeyPointRequestDto>> Reject(
        long requestId,
        [FromBody] RejectRequestDto dto)
    {
        try
        {
            var adminId = long.Parse(User.FindFirst("id")?.Value ?? "0");
            var result = await _service.RejectRequestAsync(requestId, adminId, dto.Reason);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}