using AutoMapper;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.Core.Domain;

namespace Explorer.Tours.Core.Mappers;

public class ToursProfile : Profile
{
    public ToursProfile()
    {
        CreateMap<EquipmentDto, Equipment>().ReverseMap();
        CreateMap<TouristObject, TouristObjectDto>()
            .ForMember(
                dest => dest.Category,
                opt => opt.MapFrom(src => src.Category.ToString())
            );

        CreateMap<TouristObjectDto, TouristObject>()
            .ForMember(
                dest => dest.Category,
                opt => opt.MapFrom(src => Enum.Parse<TouristObjectCategory>(src.Category))
            );
    }
}

            
        
