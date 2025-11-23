using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Stakeholders;

[Authorize]
[ApiController]
[Route("api/person")]
public class PersonController : ControllerBase
{
    private readonly IPersonService _service;

    public PersonController(IPersonService service)
    {
        _service = service;
    }

    [HttpGet]
    public ActionResult<PersonProfileDto> GetProfile()
    {
        var personId = GetPersonId();
        if (!personId.HasValue)
            return Unauthorized("Missing or invalid personId claim.");

        try
        {
            var profile = _service.GetProfile(personId.Value);
            if (profile == null)
                return NotFound("Person profile not found.");

            return Ok(profile);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpPut]
    public ActionResult<PersonProfileDto> UpdateProfile([FromBody] PersonProfileDto dto)
    {
        var personId = GetPersonId();
        if (!personId.HasValue)
            return Unauthorized("Missing or invalid personId claim.");

        try
        {
            var updated = _service.UpdateProfile(personId.Value, dto);
            if (updated == null)
                return NotFound("Failed to update profile.");

            return Ok(updated);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    private long? GetPersonId()
    {
        var claim = User.Claims.FirstOrDefault(c => c.Type == "personId");
        if (claim == null) return null;

        if (long.TryParse(claim.Value, out var id))
            return id;

        return null;
    }
}