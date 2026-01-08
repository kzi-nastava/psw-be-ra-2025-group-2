using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Payments.Core.Domain.Wallets;

namespace Explorer.Payments.Core.Domain.RepositoryInterfaces
{
    public interface INotificationRepository
    {
        Notification Create(Notification notification);
        List<Notification> GetByTouristId(long touristId);
        Notification? GetById(long id);
        Notification Update(Notification notification);
        List<Notification> GetUnreadByTouristId(long touristId);
    }
}
