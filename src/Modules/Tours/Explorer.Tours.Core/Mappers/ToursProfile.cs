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

        CreateMap<TourProblem, TourProblemDto>().ReverseMap();
        CreateMap<CreateTourProblemDto, TourProblem>();
       /* CreateMap<TouristEquipment, TouristEquipmentDto>()
            .ForMember(dest => dest.Equipment, opt => opt.MapFrom(src => src.Equipment))
            .ForMember(dest => dest.Equipments, opt => opt.Ignore());

        CreateMap<TouristEquipmentDto, TouristEquipment>()
            .ForMember(dest => dest.Equipment, opt => opt.MapFrom(src => src.Equipment));*/

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
        CreateMap<Tour, TourDto>()
           .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

        CreateMap<CreateTourDto, Tour>()
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags ?? new List<string>()));

        CreateMap<UpdateTourDto, Tour>()
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags ?? new List<string>()));
        CreateMap<MonumentDto, Monument>().ReverseMap();


    }
}

            
        

       
