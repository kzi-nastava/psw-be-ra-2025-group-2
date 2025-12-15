using AutoMapper;
using Explorer.Stakeholders.Core.Domain.ShoppingCarts;
using Explorer.Stakeholders.API.Dtos;
namespace Explorer.Stakeholders.Core.Mappers
{
    public class PurchaseProfile : Profile
    {
        public PurchaseProfile()
        {
            CreateMap<TourPurchaseToken, TourPurchaseTokenDto>().ReverseMap();
        }
    }
}
