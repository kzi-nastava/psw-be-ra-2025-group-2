using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Payments.API.Internal
{
    public interface IInternalTokenService
    {
        IEnumerable<long> GetPurchasedTourIds(long touristId);
        IEnumerable<long> GetTouristIdsByTourId(long tourId);
    }
}
