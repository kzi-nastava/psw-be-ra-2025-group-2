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
            Console.WriteLine($"=== SUBMIT REQUEST STARTED ===");
            Console.WriteLine($"TourId: {request.TourId}, OrdinalNo: {request.OrdinalNo}");

            var authorId = long.Parse(User.FindFirst("id")?.Value ?? "0");
            Console.WriteLine($"AuthorId: {authorId}");

            var result = await _service.SubmitRequestAsync(
                request.TourId,
                request.OrdinalNo,
                authorId);

            Console.WriteLine($"=== SUBMIT REQUEST SUCCESS ===");
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"=== InvalidOperationException: {ex.Message} ===");
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"=== EXCEPTION: {ex.Message} ===");
            return StatusCode(500, new
            {
                error = "An error occurred. Try again.",
                details = ex.Message
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
        catch (Npgsql.PostgresException ex) 
        {
            Console.WriteLine("--- Npgsql.PostgresException CAUGHT ---");
            Console.WriteLine($"SQLSTATE Code: {ex.SqlState}"); 
            Console.WriteLine($"Message: {ex.Message}");
            Console.WriteLine($"Detail: {ex.Detail}"); 
            Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
            Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            Console.WriteLine("--------------------------------------");

            return StatusCode(500, new { title = "A database error occurred.", detail = ex.Message });
        }
        catch (Exception ex) 
        {
            Console.WriteLine($"--- GENERAL Exception CAUGHT ---: {ex.Message}");
            return StatusCode(500, new { title = "An unexpected error occurred.", detail = ex.Message });
        }
    }
}