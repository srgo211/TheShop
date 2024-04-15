using SharedDomainModels;
using SharedInterfaces;

namespace NotificationServiceAPI.Interfaces;

public interface INotificationRepository
{
    Task<List<Notification>> GetAllNotificationsAsync();
    Task<List<Notification>> GetNotificationsByUserGuidAsync(Guid userGuid);
    Task<List<Notification>> GetNotificationsByUserIdAsync(long userId);
    Task<Notification> GetNotificationByIdAsync(Guid id);
    Task<List<Notification>> GetNotificationsByStatusAsync(SubscriptionStatus subscriptionStatus, NotificationStatus status, DateTime currentDate);
    Task CreateNotificationAsync(Notification notification);
    Task UpdateNotificationAsync(Guid id, Notification updatedNotification);
    Task DeleteNotificationAsync(Guid id);
}
