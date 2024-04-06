using NotificationServiceAPI.DTO;

namespace NotificationServiceAPI.Interfaces;

public interface INotificationRepository
{
    Task AddNotificationToUserAsync(Guid userId, Notification notification);
    Task AddNotificationsToUserAsync(Guid userId, List<Notification> notifications);
    Task UpdateUserNotificationAsync(Guid userId, Notification notification);
    Task DeleteNotificationFromUserAsync(Guid userId, Guid notificationId);
}