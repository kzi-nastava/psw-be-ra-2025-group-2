namespace Explorer.Tours.Core.Domain.RepositoryInterfaces;

public interface INotificationRepository
{
    Task<Notification?> GetByIdAsync(long id);
    Task<IEnumerable<Notification>> GetByUserIdAsync(long userId);
    Task<IEnumerable<Notification>> GetUnreadByUserIdAsync(long userId);
    Task<int> GetUnreadCountAsync(long userId);
    Task AddAsync(Notification notification);
    Task UpdateAsync(Notification notification);
    Task MarkAllAsReadAsync(long userId);
}