using Explorer.Encounters.API.Dtos.Encounter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Encounters.API.Internal
{
    public interface IInternalTouristProgressService
    {
        List<UserXpDto> GetXpForUsers(IEnumerable<long> userIds);
    }
}
