using AutoMapper;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.Core.Domain;
using Explorer.BuildingBlocks.Core.Domain;
using Explorer.Stakeholders.Core.Domain.ShoppingCarts; 



namespace Explorer.Stakeholders.Core.Mappers;

public class StakeholderProfile : Profile
{
    public StakeholderProfile()
    {
        CreateMap<TourPreferences, TourPreferencesDto>().ReverseMap();
        CreateMap<Club, ClubDto>().ReverseMap();
        CreateMap<Person, PersonProfileDto>();
        CreateMap<PersonProfileDto, Person>()
    .ForMember(dest => dest.UserId, opt => opt.Ignore())
    .ForMember(dest => dest.Email, opt => opt.Ignore());
        CreateMap<AuthorAwardsDto, AuthorAwards>().ReverseMap();
        CreateMap<TouristPosition, TouristPositionDto>().ReverseMap();
        CreateMap<ShoppingCart, ShoppingCartDto>()
            .ForMember(dest => dest.ItemCount, opt => opt.MapFrom(src => src.GetItemCount())) // Eksplicitno mapiranje metode u property
            .ReverseMap();
        CreateMap<OrderItem, OrderItemDto>().ReverseMap();
        CreateMap<Money, MoneyDto>().ReverseMap();
    }
}