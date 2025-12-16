using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Author;

[Authorize(Policy = "authorPolicy")]
[Route("api/author/public-key-points")]
[ApiController]
public class PublicKeyPointController : ControllerBase
{
    private readonly IPublicKeyPointService _service;

    public PublicKeyPointController(IPublicKeyPointService service)
    {
        _service = service;
    }

    [HttpPost("submit")]
    public async Task<ActionResult<PublicKeyPointRequestDto>> Submit(
    [FromBody] SubmitKeyPointRequestDto request)
    {
        try
        {
            var authorId = long.Parse(User.FindFirst("id")?.Value ?? "0");
            var result = await _service.SubmitRequestAsync(request.TourId, request.OrdinalNo, authorId);
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
        catch (Exception ex)
        {
            // DODAJ ex.Message i ex.StackTrace
            Console.WriteLine($"❌ ERROR: {ex.Message}");
            Console.WriteLine($"❌ INNER: {ex.InnerException?.Message}");
            Console.WriteLine($"❌ STACK: {ex.StackTrace}");
            return StatusCode(500, new
            {
                error = ex.Message,
                innerError = ex.InnerException?.Message,
                stackTrace = ex.StackTrace
            });
        }
    }

    [HttpGet("my-requests")]
    public async Task<ActionResult<IEnumerable<PublicKeyPointRequestDto>>> GetMyRequests()
    {
        try
        {
            var authorId = long.Parse(User.FindFirst("id")?.Value ?? "0");
            var requests = await _service.GetAuthorRequestsAsync(authorId);
            return Ok(requests);
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = "An error occurred while fetching your requests." });
        }
    }
}