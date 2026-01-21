using AutoMapper;
using AutoMapper.Configuration.Conventions;
using Explorer.Stakeholders.API.Dtos.Planner;
using Explorer.Stakeholders.Core.Domain.Planner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Core.Mappers
{
    public class PlannerProfile : Profile
    {
        public PlannerProfile()
        {
            CreateMap<ScheduleEntry, ScheduleEntryDto>()
                .ForMember(dest => dest.Start, opt => opt.MapFrom(src => src.ScheduledTime.Start))
                .ForMember(dest => dest.End, opt => opt.MapFrom(src => src.ScheduledTime.End))
                .ForMember(dest => dest.TourName, opt => opt.Ignore());

            CreateMap<DayEntry, DayEntryDto>()
                .ForMember(dest => dest.Entries, opt => opt.MapFrom(src => src.Entries));
        }
    }
}
