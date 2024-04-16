using NotificationServiseAPI.Interfaces;
using SharedDomainModels;

namespace NotificationServiseAPI.BL;

public class CompositeMessageSender : IMessageSender
{
    private readonly List<IMessageSender> senders = new List<IMessageSender>();

    public CompositeMessageSender(IEnumerable<IMessageSender> senders)
    {
        this.senders.AddRange(senders);
    }

    public async Task SendAsync(Notification notification)
    {
        foreach (var sender in senders)
        {
            await sender.SendAsync(notification);
        }
    }
}