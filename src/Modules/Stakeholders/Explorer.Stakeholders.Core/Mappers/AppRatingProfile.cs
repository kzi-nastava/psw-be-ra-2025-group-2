using AutoMapper;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.Core.Domain;

namespace Explorer.Stakeholders.Core.Mappers
{
    public class AppRatingProfile : Profile
    {
        public AppRatingProfile()
        {
            CreateMap<AppRatingDto, AppRating>().ReverseMap();
        }
    }
}