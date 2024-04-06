namespace NotificationServiceAPI.Interfaces;

public enum NotificationStatus
{
    None = 0,
    Sent = 2,
    Failed = 4,
    Wait = 8,
}