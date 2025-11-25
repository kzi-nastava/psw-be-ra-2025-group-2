using AutoMapper;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.Core.Domain;
namespace Explorer.Stakeholders.Core.Mappers;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<AdminUserInfoDto, User>().ReverseMap();
        CreateMap<User, AdminUserInfoDto>().ReverseMap();
    }
}
