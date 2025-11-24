using AutoMapper;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.Core.Domain;

namespace Explorer.Tours.Core.Mappers;

public class ToursProfile : Profile
{
    public ToursProfile()
    {
        CreateMap<EquipmentDto, Equipment>().ReverseMap();
        CreateMap<TouristEquipment, TouristEquipmentDto>()
            .ForMember(dest => dest.Equipment, opt => opt.MapFrom(src => src.Equipment))
            .ForMember(dest => dest.Equipments, opt => opt.Ignore());

        CreateMap<TouristEquipmentDto, TouristEquipment>()
            .ForMember(dest => dest.Equipment, opt => opt.MapFrom(src => src.Equipment));

    }
}