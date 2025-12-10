using Explorer.ShoppingCart.Core.Interfaces;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist
{
    [Authorize(Policy = "touristPolicy")]
    [Route("api/tourist/purchase")]
    public class PurchaseController : ControllerBase
    {
        private readonly IPurchaseService _purchaseService;

        public PurchaseController(IPurchaseService purchaseService)
        {
            _purchaseService = purchaseService;
        }

        [HttpPost]
        public IActionResult Purchase()
        {
            var result = _purchaseService.CompletePurchase(User.PersonId());

            if (result.IsFailed)
                return BadRequest(result.Errors);

            return Ok(result.Value);
        }
    }
}