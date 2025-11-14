using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Stakeholders.API.Dtos;

namespace Explorer.Stakeholders.API.Public
{
    public interface IPersonService
    {
        PersonProfileDto GetProfile(long userId);
        PersonProfileDto UpdateProfile(long userId, UpdatePersonProfileDto dto);

    }
}
