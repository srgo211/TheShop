using NotificationServiceAPI.Interfaces;

namespace NotificationServiceAPI.DTO;

public class User : IUser
{
    public Guid Id { get; set; }
    public long UserId { get; set; }
    public string Email { get; set; }
    public SubscriptionStatus Status { get; set; } = SubscriptionStatus.None;
    public List<Notification> Notifications { get; set; } = new List<Notification>();
}

