using AutoMapper;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.Core.Domain.ShoppingCarts;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.Core.Domain.ShoppingCarts;
using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Payments.Core.Mappers
{
    public class PaymentsProfile : Profile
    {
        public PaymentsProfile()
        {
            CreateMap<ShoppingCart, ShoppingCartDto>()
              .ForMember(dest => dest.ItemCount, opt => opt.MapFrom(src => src.GetItemCount())) // Eksplicitno mapiranje metode u property
              .ReverseMap();
            CreateMap<OrderItem, OrderItemDto>().ReverseMap();
            CreateMap<Money, MoneyDto>().ReverseMap();
            CreateMap<PaymentRecord, PaymentRecordDto>().ReverseMap();
        }
    }
}
