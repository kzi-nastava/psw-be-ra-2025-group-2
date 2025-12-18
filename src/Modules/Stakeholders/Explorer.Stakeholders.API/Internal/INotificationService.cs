using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Internal
{
    public interface INotificationService
    {
        void SendMembershipAccepted(long touristId, long clubId);
        void SendMembershipRejected(long touristId, long clubId);
        void SendInvitation(long touristId, long clubId);
    }
}
