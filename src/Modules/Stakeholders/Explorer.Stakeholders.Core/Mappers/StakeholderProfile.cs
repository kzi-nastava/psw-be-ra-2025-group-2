using AutoMapper;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.Core.Domain;

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
    }
}