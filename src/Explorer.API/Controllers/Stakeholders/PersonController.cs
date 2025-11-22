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

    [HttpGet]
    public ActionResult<PersonProfileDto> GetProfile()
    {
        var userId = long.Parse(User.Claims.First(c => c.Type == "id").Value);

        return Ok(_service.GetProfile(userId));
    }

    [HttpPut]
    public ActionResult<PersonProfileDto> UpdateProfile([FromBody] UpdatePersonProfileDto dto)
    {
        var userId = long.Parse(User.Claims.First(c => c.Type == "id").Value);

        return Ok(_service.UpdateProfile(userId, dto));
    }
}