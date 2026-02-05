using Explorer.API.Controllers.Tourist;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public;
using Explorer.Payments.Infrastructure.Database;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Tours.API.Public.Administration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Security.Claims;

namespace Explorer.Payments.Tests.Integration;

[Collection("Sequential")]
public class PurchaseRecordCommandTests : BaseStakeholdersIntegrationTest
{
    public PurchaseRecordCommandTests(PaymentsTestFactory factory) : base(factory) { }

    [Fact]
    public void Purchase_creates_payment_record_and_clears_cart()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();

        // turist -21 ima wallet u seed-u (100 AC) po onome sto si slala
        const string touristId = "-21";
        const long tourIdToBuy = -2; // published tura u seed-u
        var db = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

        var cartController = CreateShoppingCartController(scope, touristId);
        var purchaseController = CreatePurchaseController(scope, touristId);

        // 1) Ensure cart empty (opciono)
        cartController.ClearCart();

        // 2) Add tour to cart (OVO zavisi od tvog AddToCartDto)
        // Ako AddToCartDto ima samo TourId:
        var addDto = new Explorer.Stakeholders.API.Public.DTOs.AddToCartDto { TourId = tourIdToBuy };
        var addAction = cartController.AddItem(addDto).Result;

        if (addAction is ObjectResult obj && obj.StatusCode == 500)
        {
            throw new Exception($"AddItem returned 500: {obj.Value}");
        }

        var addResult = addAction.ShouldBeOfType<OkObjectResult>();

        var cart = addResult.Value.ShouldBeOfType<Explorer.Stakeholders.API.Dtos.ShoppingCartDto>();
        cart.Items.Count.ShouldBe(1);
        cart.Items[0].TourId.ShouldBe(tourIdToBuy);

        // 3) Purchase (bez kupona)
        var purchaseResult = purchaseController.Purchase(null).ShouldBeOfType<OkObjectResult>();
        var tokens = purchaseResult.Value.ShouldBeOfType<List<TourPurchaseTokenDto>>();
        tokens.Count.ShouldBe(1);
        tokens[0].TourId.ShouldBe(tourIdToBuy);

        // Assert - PaymentRecord created
        // Napomena: u DB su long, a u claims je string "-21"
        var tid = long.Parse(touristId);

        var record = db.PaymentRecords
            .OrderByDescending(r => r.CreatedAt)
            .FirstOrDefault(r => r.TouristId == tid && r.TourId == tourIdToBuy);

        record.ShouldNotBeNull();
        record.BundleId.ShouldBeNull();

        // Assert - cart cleared
        var afterCart = cartController.GetCart().Result.ShouldBeOfType<OkObjectResult>()
            .Value.ShouldBeOfType<Explorer.Stakeholders.API.Dtos.ShoppingCartDto>();

        afterCart.Items.ShouldBeEmpty();
        afterCart.TotalPrice.Amount.ShouldBe(0);
    }

    private static ShoppingCartController CreateShoppingCartController(IServiceScope scope, string touristId)
    {
        var ctrl = new ShoppingCartController(
            scope.ServiceProvider.GetRequiredService<IShoppingCartService>(),
            scope.ServiceProvider.GetRequiredService<ITourService>()
        )
        {
            ControllerContext = BuildContextWithRole(touristId, "tourist")
        };
        return ctrl;
    }

    private static PurchaseController CreatePurchaseController(IServiceScope scope, string touristId)
    {
        var ctrl = new PurchaseController(scope.ServiceProvider.GetRequiredService<IPurchaseService>())
        {
            ControllerContext = BuildContextWithRole(touristId, "tourist")
        };
        return ctrl;
    }

    private static ControllerContext BuildContextWithRole(string id, string role)
    {
        var claims = new List<Claim>
        {
            new Claim("id", id),
            new Claim(ClaimTypes.Role, role)
        };

        var identity = new ClaimsIdentity(claims, "test");
        var user = new ClaimsPrincipal(identity);

        return new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
    }
}
