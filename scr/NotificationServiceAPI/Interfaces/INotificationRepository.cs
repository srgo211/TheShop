using NotificationServiceAPI.DTO;
using System;

namespace NotificationServiceAPI.Interfaces;

public interface INotificationRepository
{
    Task AddNotificationToUserAsync(Guid id, Notification notification);
    Task AddNotificationsToUserAsync(Guid id, List<Notification> notifications);
    Task UpdateUserNotificationAsync(Guid id, Notification notification);
    Task DeleteNotificationFromUserAsync(Guid id, Guid notificationId);
}