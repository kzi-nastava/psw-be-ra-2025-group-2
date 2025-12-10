using AutoMapper;
using Explorer.ShoppingCart.Core.Dtos; 
using Explorer.ShoppingCart.Core.Domain;

namespace Explorer.ShoppingCart.Core.Mappers;

public class ShoppingCartProfile : Profile
{
    public ShoppingCartProfile()
    {
        CreateMap<Cart, ShoppingCartDto>().ReverseMap();

        CreateMap<OrderItem, OrderItemDto>().ReverseMap();
    }
}