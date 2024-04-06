namespace NotificationServiceAPI.Interfaces;

public enum NotificationStatus
{
    None = 0,
    Sent = 2,
    Failed = 4,
    Wait = 8,
}
public interface INotification
{
    Guid Id { get; set; }
    string Message { get; set; }
    DateTime CreatedAt { get; set; }
    DateTime? SendDate { get; set; }
    NotificationStatus Status { get; set; }
}