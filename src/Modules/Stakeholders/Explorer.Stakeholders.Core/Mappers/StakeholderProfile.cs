using AutoMapper;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.Core.Domain;

namespace Explorer.Stakeholders.Core.Mappers;

public class StakeholderProfile : Profile
{
    public StakeholderProfile()
    {
        CreateMap<Person, PersonProfileDto>();
        CreateMap<UpdatePersonProfileDto, Person>()
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.Email, opt => opt.Ignore())
            .ForMember(dest => dest.Name, opt => opt.Ignore())
            .ForMember(dest => dest.Surname, opt => opt.Ignore());

        CreateMap<AuthorAwardsDto, AuthorAwards>().ReverseMap();
    }
}