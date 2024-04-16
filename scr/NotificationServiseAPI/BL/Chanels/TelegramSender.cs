using NotificationServiseAPI.Interfaces;
using SharedDomainModels;

namespace NotificationServiseAPI.BL.Chanels;

public class TelegramSender : IMessageSender
{
    public Task SendAsync(Notification notification)
    {
        throw new NotImplementedException();
    }
}