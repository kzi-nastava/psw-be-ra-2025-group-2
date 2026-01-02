using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Tours.Infrastructure.Database.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly ToursContext _context;

    public NotificationRepository(ToursContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Notification?> GetByIdAsync(long id)
    {
        return await _context.Notifications.FindAsync(id);
    }

    public async Task<IEnumerable<Notification>> GetByUserIdAsync(long userId)
    {
        return await GetNotificationsQuery(userId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Notification>> GetUnreadByUserIdAsync(long userId)
    {
        return await GetUnreadNotificationsQuery(userId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }

    public async Task<int> GetUnreadCountAsync(long userId)
    {
        return await GetUnreadNotificationsQuery(userId).CountAsync();
    }

    public async Task AddAsync(Notification notification)
    {
        ValidateNotificationNotNull(notification);

        await _context.Notifications.AddAsync(notification);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Notification notification)
    {
        ValidateNotificationNotNull(notification);

        _context.Notifications.Update(notification);
        await _context.SaveChangesAsync();
    }

    public async Task MarkAllAsReadAsync(long userId)
    {
        var unreadNotifications = await GetUnreadNotificationsQuery(userId).ToListAsync();

        if (!unreadNotifications.Any())
            return;

        MarkNotificationsAsRead(unreadNotifications);
        await _context.SaveChangesAsync();
    }


    private IQueryable<Notification> GetNotificationsQuery(long userId)
    {
        return _context.Notifications.Where(n => n.UserId == userId);
    }

    private IQueryable<Notification> GetUnreadNotificationsQuery(long userId)
    {
        return GetNotificationsQuery(userId).Where(n => !n.IsRead);
    }

    private static void ValidateNotificationNotNull(Notification notification)
    {
        if (notification == null)
            throw new ArgumentNullException(nameof(notification));
    }

    private static void MarkNotificationsAsRead(IEnumerable<Notification> notifications)
    {
        foreach (var notification in notifications)
        {
            notification.MarkAsRead();
        }
    }
}