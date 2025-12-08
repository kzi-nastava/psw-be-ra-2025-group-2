using AutoMapper;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

namespace Explorer.Tours.Core.UseCases.Administration;

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

    public async Task<IEnumerable<NotificationDto>> GetUnreadNotificationsAsync(long userId)
    {
        var notifications = await _repository.GetUnreadByUserIdAsync(userId);
        return _mapper.Map<IEnumerable<NotificationDto>>(notifications);
    }

    public async Task<int> GetUnreadCountAsync(long userId)
    {
        return await _repository.GetUnreadCountAsync(userId);
    }

    public async Task MarkAsReadAsync(long notificationId, long userId)
    {
        var notification = await _repository.GetByIdAsync(notificationId)
            ?? throw new KeyNotFoundException("Notification not found.");

        if (notification.UserId != userId)
            throw new UnauthorizedAccessException("You do not have access to this notification.");

        notification.MarkAsRead();
        await _repository.UpdateAsync(notification);
    }

    public async Task MarkAllAsReadAsync(long userId)
    {
        await _repository.MarkAllAsReadAsync(userId);
    }

    public async Task DeleteNotificationAsync(long notificationId, long userId)
    {
        var notification = await _repository.GetByIdAsync(notificationId)
            ?? throw new KeyNotFoundException("Notification not found.");

        if (notification.UserId != userId)
            throw new UnauthorizedAccessException("You do not have access to this notification.");

        await _repository.DeleteAsync(notification);
    }

    public async Task NotifyAuthorApprovedAsync(long authorId, string keyPointName)
    {
        var notification = new Notification(
      authorId,
      "Key point approved!",
      $"Your keypoint \"{keyPointName}\" has been approved for public use.",
      NotificationType.PublicKeyPointApproved
  );
        await _repository.AddAsync(notification);
    }

    public async Task NotifyAuthorRejectedAsync(long authorId, string keyPointName, string? reason)
    {
        var message = $"Your keypoint \"{keyPointName}\" has not been approved.";
        if (!string.IsNullOrWhiteSpace(reason))
            message += $" Reason: {reason}";

        var notification = new Notification(
        authorId,
        "Request rejected",
        message,
        NotificationType.PublicKeyPointRejected
        );
        await _repository.AddAsync(notification);
    }
}