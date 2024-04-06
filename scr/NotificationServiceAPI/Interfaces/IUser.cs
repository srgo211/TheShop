using NotificationServiceAPI.DTO;

namespace NotificationServiceAPI.Interfaces;

public interface IUser
{
    Guid Id { get; set; }
    string Email { get; set; }
    SubscriptionStatus Status { get; set; }
    List<Notification> Notifications { get; set; }
}