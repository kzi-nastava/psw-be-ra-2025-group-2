using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Stakeholders;

[Authorize]   // korisnik mora biti ulogovan, prilagodi ako imaš specifične role
[ApiController]
[Route("api/person")]
public class PersonController : ControllerBase
{
    private readonly IPersonService _service;

    public PersonController(IPersonService service)
    {
        _service = service;
    }

    [HttpGet("{userId:long}")]
    public ActionResult<PersonProfileDto> GetProfile(long userId)
    {
        //TODO trenutno korisnik moze da pogleda svaciji profil, pretpostavljam to nije okej

        return Ok(_service.GetProfile(userId));
    }

    [HttpPut("{userId:long}")]
    public ActionResult<PersonProfileDto> UpdateProfile(long userId, [FromBody] UpdatePersonProfileDto dto)
    {
        //TODO trenutno korisnik moze da menja svaciji profil, pretpostavljam to nije okej

        return Ok(_service.UpdateProfile(userId, dto));
    }
}