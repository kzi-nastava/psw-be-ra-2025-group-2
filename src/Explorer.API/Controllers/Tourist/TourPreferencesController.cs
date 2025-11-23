using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Explorer.API.Controllers.Tourist;

[Authorize(Policy = "touristPolicy")]
[Route("api/tourist/preferences")]
[ApiController]
public class TourPreferencesController : ControllerBase
{
    private readonly ITourPreferencesService _tourPreferencesService;

    public TourPreferencesController(ITourPreferencesService tourPreferencesService)
    {
        _tourPreferencesService = tourPreferencesService;
    }

    private long GetAuthenticatedTouristId()
    {
        // Proveri različite claim tipove koje JWT može da koristi
        var claim = User.Claims.FirstOrDefault(c =>
            c.Type == ClaimTypes.NameIdentifier || 
            c.Type == "id" ||                      
            c.Type == "userId" ||                   
            c.Type == "sub");                       

        if (claim == null || !long.TryParse(claim.Value, out long touristId))
        {
            // Loguj sve claims za debugging
            var allClaims = string.Join(", ", User.Claims.Select(c => $"{c.Type}={c.Value}"));
            throw new UnauthorizedAccessException($"Invalid token. Available claims: {allClaims}");
        }

        return touristId;
    }

    [HttpGet]
    public ActionResult<TourPreferencesDto> Get()
    {
        try
        {
            var touristId = GetAuthenticatedTouristId();
            var result = _tourPreferencesService.GetByTourist(touristId);

            if (result == null)
            {
                return NotFound("You don't have any preferences yet.");
            }

            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
    }

    [HttpPost]
    public ActionResult<TourPreferencesDto> Create([FromBody] TourPreferencesDto dto)
    {
        try
        {
            var touristId = GetAuthenticatedTouristId();
            dto.TouristId = touristId;

            var existing = _tourPreferencesService.GetByTourist(touristId);
            if (existing != null)
            {
                return BadRequest("You already have preferences.");
            }

            var created = _tourPreferencesService.Create(dto);
            return Ok(created);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut]
    public ActionResult<TourPreferencesDto> Update([FromBody] TourPreferencesDto dto)
    {
        try
        {
            var touristId = GetAuthenticatedTouristId();

            var existing = _tourPreferencesService.GetByTourist(touristId);
            if (existing == null)
            {
                return NotFound("You don't have any preferences to update.");
            }

            dto.Id = existing.Id;
            dto.TouristId = touristId;

            var updated = _tourPreferencesService.Update(dto);
            return Ok(updated);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete]
    public ActionResult Delete()
    {
        try
        {
            var touristId = GetAuthenticatedTouristId();

            var existing = _tourPreferencesService.GetByTourist(touristId);
            if (existing == null)
            {
                return NotFound("You don't have any preferences to delete.");
            }

            _tourPreferencesService.Delete(existing.Id);
            return Ok("Preferences deleted successfully.");
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
    }
}