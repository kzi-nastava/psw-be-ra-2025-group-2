using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Stakeholders.API.Internal;

namespace Explorer.Stakeholders.Core.UseCases
{
    public class NotificationService : INotificationService
    {
        public void SendMembershipAccepted(long touristId, long clubId)
        {
            Console.WriteLine($"[NOTIFICATION] Tourist {touristId} accepted into club {clubId}.");
        }

        public void SendMembershipRejected(long touristId, long clubId)
        {
            Console.WriteLine($"[NOTIFICATION] Tourist {touristId} rejected from club {clubId}.");
        }

        public void SendInvitation(long touristId, long clubId)
        {
            Console.WriteLine($"[NOTIFICATION] Tourist {touristId} invited to club {clubId}.");
        }
    }
}