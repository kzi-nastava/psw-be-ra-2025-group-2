using AutoMapper;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.Core.Domain;

namespace Explorer.Stakeholders.Core.Mappers
{
    public class AppRatingProfile : Profile
    {
        public AppRatingProfile()
        {
            CreateMap<AppRating, AppRatingDto>();
            CreateMap<AppRatingDto, AppRating>().ForMember(dest => dest.Id, opt => opt.Ignore());
        }
    }
}