using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Explorer.BuildingBlocks.Core.Domain;
using Explorer.BuildingBlocks.Core.UseCases;
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
                .ForMember(dest => dest.Latitude,
                opt => opt.MapFrom(
                    src => src.Location.Latitude))
                .ForMember(dest => dest.Longitude,
                opt => opt.MapFrom(
                    src => src.Location.Longitude))
                .ForMember(dest => dest.XP,
                opt => opt.MapFrom(
                    src => src.XP.Value));

            CreateMap<CreateEncounterDto, Encounter>().ConstructUsing((src, ctx) => new Encounter(src.Name,
                src.Description,
                new GeoLocation(src.Latitude, src.Longitude),
                new ExperiencePoints(src.XP),
                ctx.Mapper.Map<EncounterType>(src.Type)));
        }
    }
}
