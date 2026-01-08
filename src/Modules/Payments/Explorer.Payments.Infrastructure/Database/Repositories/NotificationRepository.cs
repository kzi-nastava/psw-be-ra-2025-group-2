using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;
using Explorer.Payments.Core.Domain.Wallets;

namespace Explorer.Payments.Infrastructure.Database.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly PaymentsContext _context;

        public NotificationRepository(PaymentsContext context)
        {
            _context = context;
        }

        public Notification Create(Notification notification)
        {
            _context.Notifications.Add(notification);
            _context.SaveChanges();
            return notification;
        }

        public List<Notification> GetByTouristId(long touristId)
        {
            return _context.Notifications
                .Where(n => n.TouristId == touristId)
                .OrderByDescending(n => n.CreatedAt)
                .ToList();
        }

        public Notification? GetById(long id)
        {
            return _context.Notifications.Find(id);
        }

        public Notification Update(Notification notification)
        {
            _context.Notifications.Update(notification);
            _context.SaveChanges();
            return notification;
        }

        public List<Notification> GetUnreadByTouristId(long touristId)
        {
            return _context.Notifications
                .Where(n => n.TouristId == touristId && !n.IsRead)
                .OrderByDescending(n => n.CreatedAt)
                .ToList();
        }
    }
}
