using AutoMapper;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

namespace Explorer.Tours.Core.UseCases;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _repository;
    private readonly IMapper _mapper;

    public NotificationService(INotificationRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<NotificationDto>> GetUserNotificationsAsync(long userId)
    {
        var notifications = await _repository.GetByUserIdAsync(userId);
        return _mapper.Map<IEnumerable<NotificationDto>>(notifications);
    }

    public async Task<int> GetUnreadCountAsync(long userId)
    {
        return await _repository.GetUnreadCountAsync(userId);
    }

    public async Task MarkAsReadAsync(long notificationId, long userId)
    {
        var notification = await GetNotificationAsync(notificationId);
        ValidateUserOwnership(notification, userId);

        notification.MarkAsRead();
        await _repository.UpdateAsync(notification);
    }

    public async Task MarkAllAsReadAsync(long userId)
    {
        await _repository.MarkAllAsReadAsync(userId);
    }

    public async Task NotifyAuthorApprovedAsync(long authorId, string keyPointName)
    {
        var notification = CreateApprovedNotification(authorId, keyPointName);
        await _repository.AddAsync(notification);
    }

    public async Task NotifyAuthorRejectedAsync(long authorId, string keyPointName, string? reason)
    {
        var notification = CreateRejectedNotification(authorId, keyPointName, reason);
        await _repository.AddAsync(notification);
    }

    private async Task<Notification> GetNotificationAsync(long notificationId)
    {
        return await _repository.GetByIdAsync(notificationId)
            ?? throw new KeyNotFoundException($"Notification with ID {notificationId} not found.");
    }

    private static void ValidateUserOwnership(Notification notification, long userId)
    {
        if (notification.UserId != userId)
            throw new UnauthorizedAccessException("You do not have permission to access this notification.");
    }

    private static Notification CreateApprovedNotification(long authorId, string keyPointName)
    {
        return new Notification(
            authorId,
            "KeyPoint Approved",
            $"Your keypoint \"{keyPointName}\" has been approved for public use.",
            NotificationType.PublicKeyPointApproved
        );
    }

    private static Notification CreateRejectedNotification(long authorId, string keyPointName, string? reason)
    {
        var message = BuildRejectionMessage(keyPointName, reason);

        return new Notification(
            authorId,
            "KeyPoint Request Rejected",
            message,
            NotificationType.PublicKeyPointRejected
        );
    }

    private static string BuildRejectionMessage(string keyPointName, string? reason)
    {
        var message = $"Your keypoint \"{keyPointName}\" was not approved.";

        if (!string.IsNullOrWhiteSpace(reason))
            message += $" Reason: {reason}";

        return message;
    }
}