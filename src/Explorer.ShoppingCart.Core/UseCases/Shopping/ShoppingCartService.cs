using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.ShoppingCart.Core.Interfaces;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using FluentResults;
using System.Threading.Tasks;
using Explorer.ShoppingCart.Core.Domain;
using Explorer.Tours.Core.Domain;

namespace Explorer.ShoppingCart.Core.UseCases.Shopping;

public class ShoppingCartService : IShoppingCartService
{
    private readonly IShoppingCartRepository _cartRepository;
    private readonly ITourRepository _tourRepository;

    public ShoppingCartService(IShoppingCartRepository cartRepository, ITourRepository tourRepository)
    {
        _cartRepository = cartRepository;
        _tourRepository = tourRepository;
    }

    public async Task<Result<Cart>> AddItem(long touristId, long tourId)
    {
        try
        {
            var tour = await _tourRepository.GetByIdAsync(tourId);
            if (tour == null) return Result.Fail<Cart>("Tour not found.");

            var cart = _cartRepository.GetByTouristId(touristId) ?? new Cart(touristId);
            cart.AddItem(tour);

            var updatedCart = cart.Id == 0 ? _cartRepository.Create(cart) : _cartRepository.Update(cart);
            return Result.Ok(updatedCart);
        }
        catch (InvalidOperationException e)
        {
            return Result.Fail<Cart>(e.Message);
        }
    }

    public Result<Cart> GetByTouristId(long touristId)
    {
        var cart = _cartRepository.GetByTouristId(touristId) ?? new Cart(touristId);
        return Result.Ok(cart);
    }

    public Result<Cart> RemoveItem(long touristId, long tourId)
    {
        var cart = _cartRepository.GetByTouristId(touristId);
        if (cart == null) return Result.Fail<Cart>("Cart not found.");

        cart.RemoveItem(tourId);

        var updatedCart = _cartRepository.Update(cart);
        return Result.Ok(updatedCart);
    }
}