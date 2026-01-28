using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Payments.API.Public;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;
using Explorer.Payments.Core.Domain.Wallets;

namespace Explorer.Payments.Core.UseCases
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;

        public NotificationService(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public void NotifyWalletDeposit(long touristId, int amount)
        {
            var message = $"You received {amount} Adventure Coins! Your wallet has been credited.";
            var notification = new Notification(touristId, message, NotificationType.WalletDeposit);

            _notificationRepository.Create(notification);

            // TODO: Ovde možeš dodati SignalR za real-time notifikacije
            Console.WriteLine($"[NOTIFICATION] Tourist {touristId} received {amount} AC.");
        }

        public List<NotificationDto> GetTouristNotifications(long touristId)
        {
            var notifications = _notificationRepository.GetByTouristId(touristId);
            return MapToDto(notifications); 
        }

        public List<NotificationDto> GetUnreadNotifications(long touristId)
        {
            var notifications = _notificationRepository.GetUnreadByTouristId(touristId);
            return MapToDto(notifications); 
        }

        public void MarkAsRead(long notificationId)
        {
            var notification = _notificationRepository.GetById(notificationId);
            if (notification == null)
                throw new ArgumentException("Notification not found");

            notification.MarkAsRead();
            _notificationRepository.Update(notification);
        }
        private List<NotificationDto> MapToDto(List<Notification> notifications)
        {
            return notifications.Select(n => new NotificationDto
            {
                Id = n.Id,
                TouristId = n.TouristId,
                Message = n.Message,
                CreatedAt = n.CreatedAt,
                IsRead = n.IsRead,
                Type = n.Type.ToString()
            }).ToList();
        }

        public void NotifyUserMessage(long touristId, string message)
        {
            var notification = new Notification(touristId, message, NotificationType.General);
            _notificationRepository.Create(notification);
            
            Console.WriteLine($"[NOTIFICATION] Tourist {touristId}: {message}");
        }

    }

}
