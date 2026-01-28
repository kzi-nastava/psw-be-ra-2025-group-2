using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Payments.API.Dtos;

namespace Explorer.Payments.API.Public
{
    public interface INotificationService
    {
        void NotifyWalletDeposit(long touristId, int amount);
        void NotifyUserMessage(long touristId, string message);
        List<NotificationDto> GetTouristNotifications(long touristId);
        List<NotificationDto> GetUnreadNotifications(long touristId);
        void MarkAsRead(long notificationId);
    }
}
