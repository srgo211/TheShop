namespace NotificationServiceAPI.Interfaces;

public interface INotification
{
    Guid Id { get; set; }
    Guid UserGuid { get; set; }
    long UserId { get; set; }
    string Theme { get; set; }
    string Message { get; set; }
    DateTime CreatedAt { get; set; }
    DateTime? SendDate { get; set; }
    NotificationStatus Status { get; set; }
    SubscriptionStatus SubscriptionStatus { get; set; }
}