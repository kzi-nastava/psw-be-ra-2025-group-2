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

            CreateMap<OrderItem, OrderItemDto>()
              .ForMember(d => d.TourId, opt => opt.MapFrom(s => s.TourId))
              .ForMember(d => d.BundleId, opt => opt.MapFrom(s => s.BundleId))
              .ForMember(d => d.TourIds, opt => opt.MapFrom(s => s.TourIds))
              .ForMember(d => d.ItemType, opt => opt.MapFrom(s => s.ItemType))
              .ForMember(d => d.Title, opt => opt.MapFrom(s => s.Title))
              .ForMember(d => d.AuthorId, opt => opt.MapFrom(s => s.AuthorId))
              .ForMember(d => d.Price, opt => opt.MapFrom(s => new MoneyDto
              {
          Amount = s.Price.Amount,
          Currency = s.Price.Currency
      }));

            CreateMap<ShoppingCart, ShoppingCartDto>()
                .ForMember(d => d.Items, opt => opt.MapFrom(s => s.Items))
                .ForMember(d => d.TotalPrice, opt => opt.MapFrom(s => new MoneyDto
                {
                    Amount = s.TotalPrice.Amount,
                    Currency = s.TotalPrice.Currency
                }));


        }
    }
}
