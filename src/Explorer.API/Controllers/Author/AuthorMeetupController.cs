using Explorer.Stakeholders.Infrastructure.Authentication; // zbog User.PersonId()
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Explorer.API.Controllers.Author;

[Authorize(Policy = "authorPolicy")]
[Route("api/author/meetups")]
[ApiController]
public class AuthorMeetupController : ControllerBase
{
    private readonly IMeetupService _meetupService;

    public AuthorMeetupController(IMeetupService meetupService)
    {
        _meetupService = meetupService;
    }

    /// <summary>
    /// Autor vidi sve meetupe (svi autori i turisti vide sve meetupe).
    /// GET api/author/meetups
    /// </summary>
    [HttpGet]
    public ActionResult<IEnumerable<MeetupDto>> GetAll()
    {
        var result = _meetupService.GetAll();
        return Ok(result);
    }

    /// <summary>
    /// Autor vidi samo svoje meetupe (one koje je kreirao).
    /// GET api/author/meetups/mine
    /// </summary>
    [HttpGet("mine")]
    public ActionResult<IEnumerable<MeetupDto>> GetMine()
    {
        var personId = User.PersonId();
        var result = _meetupService.GetByCreator(personId);
        return Ok(result);
    }

    /// <summary>
    /// Autor kreira novi meetup.
    /// POST api/author/meetups
    /// </summary>
    [HttpPost]
    public ActionResult<MeetupDto> Create([FromBody] MeetupDto dto)
    {
        dto.CreatorId = User.PersonId(); // Ignorišemo eventualni CreatorId iz klijenta
        var result = _meetupService.Create(dto);
        return Ok(result);
    }

    /// <summary>
    /// Autor menja postojeći meetup.
    /// PUT api/author/meetups/{id}
    /// </summary>
    [HttpPut("{id:long}")]
    public ActionResult<MeetupDto> Update(long id, [FromBody] MeetupDto dto)
    {
        dto.Id = id; // osiguramo da se koristi id iz URL-a
        var result = _meetupService.Update(dto);
        return Ok(result);
    }

    /// <summary>
    /// Autor briše meetup koji je kreirao.
    /// DELETE api/author/meetups/{id}
    /// </summary>
    [HttpDelete("{id:long}")]
    public ActionResult Delete(long id)
    {
        _meetupService.Delete(id);
        return Ok();
    }
}