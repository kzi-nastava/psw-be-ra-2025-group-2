using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Internal;

namespace Explorer.Stakeholders.API.Public
{
    public interface IUserService
    {
        List<TouristBasicDto> GetTourists(string? query);
        List<InternalUserDto> GetAllActiveUsers();
    }
}
