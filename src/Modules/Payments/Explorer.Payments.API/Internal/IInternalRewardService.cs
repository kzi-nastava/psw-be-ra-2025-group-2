using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Encounters.API.Internal
{
    public interface IInternalRewardService
    {
        string GrantCoupon(long userId, int discountPercentage, DateTime? validUntil, string description);
        void NotifyUser(long userId, string message);
    }
}
