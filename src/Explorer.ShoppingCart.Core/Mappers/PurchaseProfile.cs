using AutoMapper;
using Explorer.ShoppingCart.Core.Domain;
using Explorer.ShoppingCart.Core.Dtos;
using Explorer.Tours.Core.Domain;

namespace Explorer.ShoppingCart.Core.Mappers
{
    public class PurchaseProfile : Profile
    {
        public PurchaseProfile()
        {
            CreateMap<TourPurchaseToken, TourPurchaseTokenDto>();
        }
    }
}
