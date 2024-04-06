using NotificationServiceAPI.Interfaces;

namespace NotificationServiceAPI.DTO;

public class Notification : INotification
{
    public Guid Id { get; set; }
    public string Message { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? SendDate { get; set; }
    public NotificationStatus Status { get; set; } = NotificationStatus.Wait;
}