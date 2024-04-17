using SharedInterfaces;

namespace SharedDomainModels;

public class Notification : INotification
{
    public Guid Id { get; set; }
    public Guid UserGuid { get; set; }
    public long UserId  { get; set; }
    public string Email { get; set; }
    public string Theme { get; set; }
    public string Message { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? SendDate { get; set; }
    public NotificationStatus Status { get; set; } = NotificationStatus.Wait;
    public SubscriptionStatus SubscriptionStatus { get; set; } = SubscriptionStatus.Enable;
    public TypeChannel TypeChannel { get; set; } = TypeChannel.Email | TypeChannel.Telegram | TypeChannel.File;

}