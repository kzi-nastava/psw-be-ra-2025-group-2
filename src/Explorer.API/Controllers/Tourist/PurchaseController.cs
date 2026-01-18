using Explorer.Payments.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Explorer.API.Controllers.Tourist
{
    [Authorize(Policy = "touristPolicy")]
    [Route("api/shopping-cart/purchase")]
    [ApiController]
    public class PurchaseController : ControllerBase
    {
        private readonly IPurchaseService _purchaseService;

        public PurchaseController(IPurchaseService purchaseService)
        {
            _purchaseService = purchaseService;
        }

        [HttpPost]
        public IActionResult Purchase([FromBody] PurchaseWithCouponDto? dto)
        {

            try
            {
                var touristId = ExtractTouristId();
                var result = _purchaseService.CompletePurchase(touristId, dto?.CouponCode);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        //[HttpPost("bundles/{bundleId:long}")]
        //public IActionResult PurchaseBundle(long bundleId)
        //{
        //    try
        //    {
        //        var touristId = ExtractTouristId();
        //        var result = _purchaseService.CompleteBundlePurchase(touristId, bundleId);

        //        return Ok(result);
        //    }
        //    catch (UnauthorizedAccessException ex)
        //    {
        //        return Unauthorized(ex.Message);
        //    }
        //    catch (InvalidOperationException ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //    catch (KeyNotFoundException ex)
        //    {
        //        return NotFound(ex.Message);
        //    }
        //}

        private long ExtractTouristId()
        {
            var idClaim = User.FindFirstValue("id")
                          ?? User.FindFirstValue("personId")
                          ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(idClaim) || !long.TryParse(idClaim, out var touristId))
                throw new UnauthorizedAccessException("Tourist ID not found in token.");

            return touristId;
        }


    }
}