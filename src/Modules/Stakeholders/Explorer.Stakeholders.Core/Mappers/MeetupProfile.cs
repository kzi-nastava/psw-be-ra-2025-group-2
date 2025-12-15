using AutoMapper;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.Core.Domain;

namespace Explorer.Stakeholders.Core.Mappers;

public class MeetupProfile : Profile
{
    public MeetupProfile()
    {
        CreateMap<Meetup, MeetupDto>().ReverseMap();
    }
}