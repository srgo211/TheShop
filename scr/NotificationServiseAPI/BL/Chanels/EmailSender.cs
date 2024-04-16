using NotificationServiseAPI.Interfaces;
using SharedDomainModels;

namespace NotificationServiseAPI.BL.Chanels;

public class EmailSender : IMessageSender
{
    public Task SendAsync(Notification notification)
    {
        throw new NotImplementedException();
    }
}