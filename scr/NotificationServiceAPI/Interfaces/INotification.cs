namespace NotificationServiceAPI.Interfaces;

public interface INotification
{
    Guid Id { get; set; }
    string Message { get; set; }
    DateTime CreatedAt { get; set; }
    DateTime? SendDate { get; set; }
    NotificationStatus Status { get; set; }
}