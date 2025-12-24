using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Internal
{
    public interface IInternalUserService
    {
        public long? GetActiveTourIdByUserId(long userId);
        public long SetActiveTourIdByUserId(long userId, long tourId);
        public void ResetActiveTourIdByUserId(long userId);
        InternalUserDto? GetById(long userId);
    }
}
