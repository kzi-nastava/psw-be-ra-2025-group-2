using AutoMapper;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.Quizzes;
using Explorer.Stakeholders.API.Dtos.Quizzes;



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

        CreateMap<DiaryDto, Diary>()
    .ConstructUsing(dto => new Diary(
        dto.UserId,
        dto.Name,
        dto.Country,
        dto.City
    ))
    .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<Diary, DiaryDto>();
        CreateMap<TouristPosition, TouristPositionDto>().ReverseMap();
     

        CreateMap<QuizOption, QuizOptionDto>();
        CreateMap<Quiz, QuizDto>();
    }
}