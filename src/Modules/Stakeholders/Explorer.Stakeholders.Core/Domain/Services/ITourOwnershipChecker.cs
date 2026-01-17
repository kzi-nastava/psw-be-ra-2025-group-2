using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Core.Domain.Services
{
    public interface ITourOwnershipChecker
    {
        void CheckOwnership(long touristId, long tourId);
    }
}
