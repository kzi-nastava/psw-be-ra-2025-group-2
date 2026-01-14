using AutoMapper;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.Execution;
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
             .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
             .ForMember(dest => dest.KeyPoints, opt => opt.MapFrom(src => src.KeyPoints))
             .ForMember(dest => dest.Durations, opt => opt.MapFrom(src => src.Durations))
             .ForMember(dest => dest.PublishedAt, opt => opt.MapFrom(src => src.PublishedAt));

        CreateMap<CreateTourDto, Tour>()
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags ?? new List<string>()))
            .ForMember(dest => dest.KeyPoints, opt => opt.MapFrom(src => src.KeyPoints));

        CreateMap<UpdateTourDto, Tour>()
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags ?? new List<string>()))
            .ForMember(dest => dest.KeyPoints, opt => opt.MapFrom(src => src.KeyPoints));

        CreateMap<MonumentDto, Monument>().ReverseMap();
        CreateMap<KeyPoint, KeyPointDto>().ReverseMap();

        CreateMap<PublicKeyPoint, PublicKeyPointDto>();

        CreateMap<Notification, NotificationDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()));

        CreateMap<PublicKeyPointRequest, PublicKeyPointRequestDto>()
      .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
      .ForMember(dest => dest.PublicKeyPoint, opt => opt.MapFrom(src => src.PublicKeyPoint));


        CreateMap<TourDurationDto, TourDuration>().ConstructUsing(src => new TourDuration((TransportType)src.TransportType,src.Minutes));
        CreateMap<TourDuration, TourDurationDto>().ForMember(dest => dest.TransportType,opt => opt.MapFrom(src => (TransportTypeDto)src.TransportType));
        CreateMap<CreateTourDto, Tour>().ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags ?? new List<string>())).ForMember(dest => dest.KeyPoints, opt => opt.MapFrom(src => src.KeyPoints));
        CreateMap<UpdateTourDto, Tour>().ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags ?? new List<string>())).ForMember(dest => dest.KeyPoints, opt => opt.MapFrom(src => src.KeyPoints));
        CreateMap<TourReviewDto, TourReview>().ReverseMap();
        CreateMap<TourReview, TourReviewPublicDto>().ForMember(d => d.TouristName, o => o.Ignore());

        // ========== BUNDLE MAPPINGS ==========
        CreateMap<Bundle, BundleDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

        CreateMap<CreateBundleDto, Bundle>();
        CreateMap<UpdateBundleDto, Bundle>();

        CreateMap<EstimatedCostItem, EstimatedCostItemDto>()
    .ForMember(d => d.Category, o => o.MapFrom(s => (int)s.Category))
    .ForMember(d => d.CategoryName, o => o.MapFrom(s => s.Category.ToString()))
    .ForMember(d => d.AmountPerPerson, o => o.MapFrom(s => s.AmountPerPerson.Amount))
    .ForMember(d => d.Currency, o => o.MapFrom(s => s.AmountPerPerson.Currency));
        
        CreateMap<EstimatedTourCost, EstimatedTourCostDto>()
        .ForMember(d => d.TotalPerPerson, o => o.MapFrom(s => s.TotalPerPerson.Amount))
        .ForMember(d => d.Currency, o => o.MapFrom(s => s.TotalPerPerson.Currency))
        .ForMember(d => d.Breakdown, o => o.MapFrom(s => s.Breakdown))
        .ForMember(d => d.Disclaimer, o => o.Ignore()); 

    }
}