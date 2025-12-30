using Explorer.Payments.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Administrator.Administration;

[Authorize(Roles ="administrator")]
[Route("api/wallet")]
[ApiController]

public class WalletController:ControllerBase
{
    private readonly IWalletService _walletService;

    public WalletController(IWalletService walletService)
    {
        _walletService = walletService;
    }

    [HttpPost("{touristId}/deposit")]
    public IActionResult AdminDeposit(long touristId, [FromQuery] int amount)
    {
        try
        {
            _walletService.AdminDeposit(touristId, amount);
            return Ok($"Successfully deposited {amount} AC to tourist {touristId}");
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
}
