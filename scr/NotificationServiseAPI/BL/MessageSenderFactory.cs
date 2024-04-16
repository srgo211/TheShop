using NotificationServiseAPI.BL.Chanels;
using NotificationServiseAPI.Interfaces;
using SharedInterfaces;

namespace NotificationServiseAPI.BL;

public static class MessageSenderFactory
{
    public static IMessageSender GetMessageSender(TypeChannel typeChanel)
    {
        var senders = new List<IMessageSender>();

        if (typeChanel.HasFlag(TypeChannel.Email))
        {
            senders.Add(new EmailSender());
        }
        if (typeChanel.HasFlag(TypeChannel.Telegram))
        {
            senders.Add(new TelegramSender());
        }
        if (typeChanel.HasFlag(TypeChannel.File))
        {
            const string pathFile = "notification_log.txt";
            senders.Add(new FileSender(pathFile));
        }

        return new CompositeMessageSender(senders);
    }
}