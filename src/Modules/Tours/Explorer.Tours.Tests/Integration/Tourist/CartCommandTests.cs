using AutoMapper;
using Explorer.API.Controllers.Author;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.ShoppingCart.Core.Domain;
using Explorer.ShoppingCart.Core.Dtos;
using Explorer.ShoppingCart.Core.Interfaces;
using Explorer.ShoppingCart.Core.UseCases.Shopping;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Infrastructure.Database;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Security.Claims;

namespace Explorer.Tours.Tests.Integration.Tourist;
[Collection("Sequential")]
public class CartCommandTests : BaseToursIntegrationTest
{
    public CartCommandTests(ToursTestFactory factory) : base(factory) { }
    [Fact]
    public void AddItem_adds_item_when_tour_is_published()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-11");
        var cart = new Cart(1);
        var tour = new CreateTourDto { 
            Name = "Test",
            Description = "Istraživanje jezera i reke na strmoj planini!",
            Difficulty = 4,
            AuthorId = -11,
            Tags = new List<string> { "jezero", "reka", "strma planina" }
        };
        var result = ((ObjectResult)controller.Create(tour).Result)?.Value as Tour;
        cart.AddItem(result);

        cart.Items.Count.ShouldBe(1);
    }

    [Fact]
    public void AddItem_does_not_add_duplicate_items()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-11");
        var cart = new Cart(1);
        var tour = new CreateTourDto
        {
            Name = "Test",
            Description = "Istraživanje jezera i reke na strmoj planini!",
            Difficulty = 4,
            AuthorId = -11,
            Tags = new List<string> { "jezero", "reka", "strma planina" }
        };
        var result = ((ObjectResult)controller.Create(tour).Result)?.Value as Tour;
        cart.AddItem(result);
        cart.AddItem(result);

        cart.Items.Count.ShouldBe(1);
    }

    [Fact]
    public void AddItem_throws_if_tour_not_published()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-11");
        var cart = new Cart(1);
        var tour = new CreateTourDto
        {
            Name = "Test",
            Description = "Istraživanje jezera i reke na strmoj planini!",
            Difficulty = 4,
            AuthorId = -11,
            Tags = new List<string> { "jezero", "reka", "strma planina" }
        };
        var result = ((ObjectResult)controller.Create(tour).Result)?.Value as Tour;
        Should.Throw<InvalidOperationException>(() => cart.AddItem(result));
    }

    [Fact]
    public void RemoveItem_removes_correct_item()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-11");
        var cart = new Cart(1);
        var tour1 = new CreateTourDto
        {
            Name = "Test1",
            Description = "Tura u kojoj se prolazi kroz znamenitosti koje sadrzi Rim!",
            Difficulty = 2,
            AuthorId = -11,
            Tags = new List<string> { "Rim", "znamenitosti", "Koloseum" }
        };
        var tour2 = new CreateTourDto
        {
            Name = "Test2",
            Description = "Prolazak i upoznavanje sa istorijom muzeja Luvr!",
            Difficulty = 3,
            AuthorId = -11,
            Tags = new List<string> { "Pariz", "muzej", "Luvr" }
        };
        var result1 = ((ObjectResult)controller.Create(tour1).Result)?.Value as Tour;
        var result2 = ((ObjectResult)controller.Create(tour2).Result)?.Value as Tour;
        cart.AddItem(result1);
        cart.AddItem(result2);

        cart.RemoveItem(5);

        cart.Items.Count.ShouldBe(2);
        cart.Items.First().TourId.ShouldBe(6);
    }

    [Fact]
    public void ClearItems_empties_the_cart()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-11");
        var cart = new Cart(1);
        var tour = new CreateTourDto
        {
            Name = "Test",
            Description = "Istraživanje jezera i reke na strmoj planini!",
            Difficulty = 4,
            AuthorId = -11,
            Tags = new List<string> { "jezero", "reka", "strma planina" }
        };
        var result = ((ObjectResult)controller.Create(tour).Result)?.Value as Tour;
        cart.AddItem(result);
        cart.ClearItems();

        cart.Items.Count.ShouldBe(0);
    }
    [Fact]
    public async Task CompletePurchase_creates_tokens_and_clears_cart()
    {
        using var scope = Factory.Services.CreateScope();

        var cartService = scope.ServiceProvider.GetRequiredService<IShoppingCartService>();
        var tokenRepo = scope.ServiceProvider.GetRequiredService<ITourPurchaseTokenRepository>();
        var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();

        var purchaseService = new PurchaseService( cartService, tokenRepo, mapper );

        // 1.) kreiramo turu
        var controller = CreateController(scope, "-11");
        var tourDto = new CreateTourDto
        {
            Name = "Kupovina",
            Description = "Test kupovine",
            Difficulty = 2,
            AuthorId = -11,
            Tags = new List<string> { "test" }
        };

        var tour = ((ObjectResult)controller.Create(tourDto).Result)?.Value as Tour;
        tour.ShouldNotBeNull();

        long touristId = 99;

        // 2.) za tog turistu, mi dodajemo turu u korpu
        var addResult = await cartService.AddItem(touristId, tour.Id);
        addResult.IsSuccess.ShouldBeTrue();

        // 3.) izvršavamo kupovinu
        var purchaseResult = purchaseService.CompletePurchase(touristId);

        purchaseResult.IsSuccess.ShouldBeTrue();
        purchaseResult.Value.Count.ShouldBe(1);

        // 4.) korpa mora biti prazna
        var cartAfter = cartService.GetByTouristId(touristId).Value;
        cartAfter.Items.Count.ShouldBe(0);
    }
    [Fact]
    public void CompletePurchase_fails_on_empty_cart()
    {
        using var scope = Factory.Services.CreateScope();

        var cartService = scope.ServiceProvider.GetRequiredService<IShoppingCartService>();
        var tokenRepo = scope.ServiceProvider.GetRequiredService<ITourPurchaseTokenRepository>();
        var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();

        var purchaseService = new PurchaseService(cartService, tokenRepo, mapper);

        long touristId = 12345; // ovaj nema ništa u korpi

        var result = purchaseService.CompletePurchase(touristId);

        result.IsFailed.ShouldBeTrue();
        result.Errors[0].Message.ShouldBe("Shopping cart is empty.");
    }

    private static TourController CreateController(IServiceScope scope, string authorId = "-11")
    {
        var controller = new TourController(scope.ServiceProvider.GetRequiredService<ITourService>())
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                    new Claim("id", authorId)
                    }, "TestAuth"))
                }
            }
        };
        return controller;
    }
}