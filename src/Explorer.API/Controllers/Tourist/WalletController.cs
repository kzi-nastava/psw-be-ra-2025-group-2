using Explorer.Payments.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Explorer.API.Controllers.Tourist;
[Authorize (Roles ="tourist")]
[Route("api/wallet")]
[ApiController]

public class WalletController : ControllerBase
{
    private readonly IWalletService _walletService;

    public WalletController(IWalletService walletService)
    {
        _walletService = walletService;
    }
    private long ExtractTouristId()
    {
        var idClaim = User.FindFirstValue("id")
            ??User.FindFirstValue("personId")
            ??User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(idClaim) || !long.TryParse(idClaim,out var touristId))
        {
            throw new UnauthorizedAccessException("Tourist ID not found in token");
        }
        return touristId;
    }

    [HttpGet("balance")]
    public ActionResult<int> GetBalance()
    {
        try
        {
            var touristId = ExtractTouristId();
            var balance = _walletService.GetBalance(touristId);
            return Ok(balance);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error:{ex.Message}");
        }
    }
}
