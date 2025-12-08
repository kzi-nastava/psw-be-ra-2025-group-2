using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public;

public interface INotificationService
{
    Task NotifyAuthorApprovedAsync(long authorId, string keyPointName);
    Task NotifyAuthorRejectedAsync(long authorId, string keyPointName, string? reason);

    Task<IEnumerable<NotificationDto>> GetUserNotificationsAsync(long userId);
    Task<IEnumerable<NotificationDto>> GetUnreadNotificationsAsync(long userId);
    Task<int> GetUnreadCountAsync(long userId);
    Task MarkAsReadAsync(long notificationId, long userId);
    Task MarkAllAsReadAsync(long userId);
    Task DeleteNotificationAsync(long notificationId, long userId);
}