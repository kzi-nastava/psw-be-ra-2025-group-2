using AutoMapper;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Explorer.Stakeholders.Core.Domain.ShoppingCarts;

namespace Explorer.Stakeholders.Core.UseCases;

public class ShoppingCartService : IShoppingCartService
{
    private readonly IShoppingCartRepository _cartRepository;
    private readonly IMapper _mapper;

    public ShoppingCartService(
        IShoppingCartRepository cartRepository,
        IMapper mapper)
    {
        _cartRepository = cartRepository;
        _mapper = mapper;
    }

    public ShoppingCartDto GetCart(long touristId)
    {
        var cart = _cartRepository.GetByTouristId(touristId);
        if (cart == null)
        {
            cart = new ShoppingCart(touristId);
            _cartRepository.Create(cart); 
        }
        return _mapper.Map<ShoppingCartDto>(cart);
    }

    public ShoppingCartDto AddTourToCart(long touristId, long tourId, string tourName, double price, string category, long authorId)
    {
        var cart = _cartRepository.GetByTouristId(touristId);
        if (cart == null)
        {
            cart = new ShoppingCart(touristId);
            _cartRepository.Create(cart);
        }

        // OVDE JE BITNO: Kastujemo 'double' (iz API-ja) u 'decimal' (za Domain)
        var moneyPrice = new Money((decimal)price, "AC");

        cart.AddItem(tourId, tourName, moneyPrice, "Published", authorId); // Status možemo hardkodirati ili proslediti

        _cartRepository.Update(cart);

        return _mapper.Map<ShoppingCartDto>(cart);
    }

    public ShoppingCartDto AddBundleToCart(long touristId, long bundleId, string bundleName, double price, List<long> tourIds, long authorId)
    {
        var cart = _cartRepository.GetByTouristId(touristId);
        if (cart == null)
        {
            cart = new ShoppingCart(touristId);
            _cartRepository.Create(cart);
        }

        var moneyPrice = new Money((decimal)price, "AC");
        cart.AddBundleItem(bundleId, bundleName, moneyPrice, tourIds, authorId);

        _cartRepository.Update(cart);
        return _mapper.Map<ShoppingCartDto>(cart);
    }


    public ShoppingCartDto RemoveItemFromCart(long touristId, long itemId)
    {
        var cart = _cartRepository.GetByTouristId(touristId);
        if (cart == null) throw new KeyNotFoundException("Shopping cart not found.");

        cart.RemoveItem(itemId);
        _cartRepository.Update(cart);
        return _mapper.Map<ShoppingCartDto>(cart);
    }

    public void ClearCart(long touristId)
    {
        var cart = _cartRepository.GetByTouristId(touristId);
        if (cart == null) throw new KeyNotFoundException("Shopping cart not found.");

        cart.ClearCart();
        _cartRepository.Update(cart);
    }
}