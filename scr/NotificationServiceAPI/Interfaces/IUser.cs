using NotificationServiceAPI.DTO;

namespace NotificationServiceAPI.Interfaces;

public enum SubscriptionStatus
{
    None = 0,
    Enable = 2,
    Disable = 4,
    Wait = 8,
}

public interface IUser
{
    Guid Id { get; set; }
    string Email { get; set; }
    SubscriptionStatus Status { get; set; }
    List<Notification> Notifications { get; set; }
}