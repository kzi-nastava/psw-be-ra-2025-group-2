using Explorer.Payments.API.Public;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Administrator.Administration;

[Authorize(Roles ="administrator")]
[Route("api/wallet")]
[ApiController]

public class WalletController:ControllerBase
{
    private readonly IWalletService _walletService;
    private readonly IUserService _userService;


    public WalletController(IWalletService walletService, IUserService userService)
    {
        _walletService = walletService;
        _userService = userService;
    }


    [HttpPost("{touristId}/deposit")]
    public IActionResult AdminDeposit(long touristId, [FromBody] DepositRequest request) // IZMENJENO
    {
        try
        {
            if (request.Amount <= 0)
            {
                return BadRequest("Amount must be greater than 0");
            }

            _walletService.AdminDeposit(touristId, request.Amount); // IZMENJENO
            return Ok($"Successfully deposited {request.Amount} AC to tourist {touristId}"); // IZMENJENO
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }
    [HttpGet("tourists")]
    public ActionResult<List<TouristBasicDto>> GetTourists([FromQuery] string? query)
    {
        return Ok(_userService.GetTourists(query));
    }


    public class DepositRequest
    {public int Amount { get; set; }}
}
