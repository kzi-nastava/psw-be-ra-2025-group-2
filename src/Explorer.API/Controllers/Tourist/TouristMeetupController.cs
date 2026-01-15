using System.Collections.Generic;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using Explorer.Stakeholders.Infrastructure.Authentication;
namespace Explorer.API.Controllers.Tourist;

[Authorize(Roles = "author,tourist")]
[Route("api/meetups")]
[ApiController]
public class TouristMeetupController : ControllerBase
{
    private readonly IMeetupService  _meetupService;

    public TouristMeetupController(IMeetupService meetupService)
    {
        _meetupService = meetupService;
    }

    /// <summary>
    /// Turista vidi sve meetupe (svi autori i turisti vide sve meetupe).
    /// GET api/tourist/meetups
    /// </summary>
    [HttpGet]
    public ActionResult<IEnumerable<MeetupDto>> GetAll()
    {
        var result = _meetupService.GetAll();
        return Ok(result);
    }

    /// <summary>
    /// Turista vidi samo svoje meetupe (one koje je kreirao).
    /// GET api/tourist/meetups/mine
    /// </summary>
    [HttpGet("mine")]
    public ActionResult<IEnumerable<MeetupDto>> GetMine()
    {
        var personId = User.PersonId();
        var result = _meetupService.GetByCreator(personId);
        return Ok(result);
    }

    /// <summary>
    /// Turista kreira novi meetup.
    /// POST api/tourist/meetups
    /// </summary>
    [HttpPost]
    public ActionResult<MeetupDto> Create([FromBody] MeetupDto dto)
    {
        dto.CreatorId = User.PersonId(); // CreatorId postavljamo iz tokena
        var result = _meetupService.Create(dto);
        return Ok(result);
    }

    /// <summary>
    /// Turista menja postojeći meetup.
    /// PUT api/tourist/meetups/{id}
    /// </summary>
    [HttpPut("{id:long}")]
    public ActionResult<MeetupDto> Update(long id, [FromBody] MeetupDto dto)
    {
        dto.Id = id;
        var result = _meetupService.Update(dto);
        return Ok(result);
    }

    /// <summary>
    /// Turista briše meetup koji je kreirao.
    /// DELETE api/tourist/meetups/{id}
    /// </summary>
    [HttpDelete("{id:long}")]
    public ActionResult Delete(long id)
    {
        _meetupService.Delete(id);
        return Ok();
    }
}