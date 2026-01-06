using AutoMapper;
using Explorer.BuildingBlocks.Core.Domain;
using Explorer.Encounters.API.Dtos.Encounter;
using Explorer.Encounters.Core.Domain;
using Explorer.Encounters.Core.Mappers.Converters;

namespace Explorer.Encounters.Core.Mappers
{
    public class EncountersProfile : Profile
    {
        public EncountersProfile()
        {
            CreateMap<string, EncounterType>().ConvertUsing<EncounterTypeConverter>();

            CreateMap<Encounter, EncounterDto>()
                .Include<SocialEncounter, EncounterDto>()
                .Include<HiddenLocationEncounter, EncounterDto>()
                .Include<MiscEncounter, EncounterDto>()
                .ForMember(dest => dest.Latitude, opt => opt.MapFrom(src => src.Location.Latitude))
                .ForMember(dest => dest.Longitude, opt => opt.MapFrom(src => src.Location.Longitude))
                .ForMember(dest => dest.XP, opt => opt.MapFrom(src => src.XP.Value))
                .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.State.ToString()))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()));

            CreateMap<SocialEncounter, EncounterDto>()
                .ForMember(dest => dest.RequiredPeople, opt => opt.MapFrom(src => src.RequiredPeople))
                .ForMember(dest => dest.Range, opt => opt.MapFrom(src => src.Range));

            CreateMap<HiddenLocationEncounter, EncounterDto>()
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImageUrl))
                .ForMember(dest => dest.DistanceTreshold, opt => opt.MapFrom(src => src.DistanceTreshold))
                .ForMember(dest => dest.ImageLatitude, opt => opt.MapFrom(src => src.ImageLocation.Latitude))
                .ForMember(dest => dest.ImageLongitude, opt => opt.MapFrom(src => src.ImageLocation.Longitude));

            CreateMap<MiscEncounter, EncounterDto>();
        }
    }
}