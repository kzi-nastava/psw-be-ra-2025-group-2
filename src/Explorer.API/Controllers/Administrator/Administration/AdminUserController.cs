using Explorer.Stakeholders.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using Explorer.BuildingBlocks.Core.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Explorer.Stakeholders.API.Public;

namespace Explorer.Stakeholders.API.Controllers;

[Authorize(Policy = "administratorPolicy")]
[Route("api/administration/users")]
[ApiController]
public class AdminUserController : ControllerBase
{
    private readonly IAdminUserService _service;

    public AdminUserController(IAdminUserService service)
    {
        _service = service;
    }

    [HttpPost]
    public IActionResult CreateAccount([FromBody] AccountRegistrationDto dto)
    {
        try
        {
            _service.CreateAccount(dto);
            return Ok("Account created.");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPut("{username}/block")]
    public IActionResult BlockUser(string username)
    {
        try
        {
            _service.BlockUser(username);
            return Ok("User blocked.");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPut("{username}/unblock")]
    public IActionResult UnblockUser(string username)
    {
        try
        {
            _service.UnblockUser(username);
            return Ok("User unblocked.");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("{username}")]
    public ActionResult<AdminUserInfoDto> GetUserInfo(string username)
    {
        try
        {
            return Ok(_service.GetUserInfoByName(username));
        }
        catch (Exception e)
        {
            return NotFound(e.Message);
        }
    }

    [HttpGet]
    public ActionResult<PagedResult<AdminUserInfoDto>> GetUsers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        return Ok(_service.GetUsers(pageNumber, pageSize));
    }
}
