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
    private readonly IPublicKeyPointService _publicKeyPointService;

    public PublicKeyPointController(IPublicKeyPointService publicKeyPointService)
    {
        _publicKeyPointService = publicKeyPointService;
    }

    [HttpPost("submit")]
    public async Task<ActionResult<PublicKeyPointDto>> SubmitKeyPointForPublicUse(
        [FromBody] SubmitPublicKeyPointRequestDto dto)
    {
        try
        {
            var authorId = GetUserId();
            var result = await _publicKeyPointService.SubmitKeyPointForPublicUseAsync(
                dto.TourId, dto.OrdinalNo, authorId);

            return Ok(result);
        }
        catch (KeyNotFoundException ex) { return NotFound(new { error = ex.Message }); }
        catch (UnauthorizedAccessException ex) { return StatusCode(403, new { error = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
        catch (Exception ex) { return StatusCode(500, new { error = ex.Message }); }
    }

    [HttpGet("my-requests")]
    public async Task<ActionResult<IEnumerable<PublicKeyPointDto>>> GetMyPublicKeyPoints()
    {
        try
        {
            var authorId = GetUserId();
            var result = await _publicKeyPointService.GetAuthorPublicKeyPointsAsync(authorId);
            return Ok(result);
        }
        catch (Exception) { return StatusCode(500, new { error = "An error occurred while loading the request." }); }
    }

    [HttpGet("approved")]
    public async Task<ActionResult<IEnumerable<PublicKeyPointDto>>> GetApprovedPublicKeyPoints()
    {
        try
        {
            var result = await _publicKeyPointService.GetApprovedPublicKeyPointsAsync();
            return Ok(result);
        }
        catch (Exception) { return StatusCode(500, new { error = "An error occurred while loading public points." }); }
    }

    [HttpPost("add-to-tour")]
    public async Task<ActionResult> AddPublicKeyPointToTour([FromBody] AddPublicKeyPointToTourDto dto)
    {
        try
        {
            var authorId = GetUserId();
            await _publicKeyPointService.AddPublicKeyPointToTourAsync(
                dto.PublicKeyPointId, dto.TourId, dto.OrdinalNo, authorId);
            return Ok(new { message = "Public point successfully added to the tour." });
        }
        catch (KeyNotFoundException ex) { return NotFound(new { error = ex.Message }); }
        catch (UnauthorizedAccessException ex) { return StatusCode(403, new { error = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
        catch (Exception) { return StatusCode(500, new { error = "An error occurred." }); }
    }

    private long GetUserId()
    {
        var userIdClaim = User.FindFirst("id")?.Value
            ?? throw new UnauthorizedAccessException("User ID not found in token.");

        return Math.Abs(long.Parse(userIdClaim));
    }
}
