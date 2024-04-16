using SharedDomainModels;

namespace NotificationServiseAPI.Interfaces;

public interface IMessageSender
{
    Task SendAsync(Notification notification);
}