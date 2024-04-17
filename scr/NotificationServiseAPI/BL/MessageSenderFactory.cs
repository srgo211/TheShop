using NotificationServiseAPI.BL.Chanels;
using NotificationServiseAPI.Interfaces;
using SharedInterfaces;

namespace NotificationServiseAPI.BL;

public static class MessageSenderFactory
{
    private static IConfiguration _configuration;

    public static void Initialize(IConfiguration configuration)
    { 
        _configuration = configuration;
    }


    public static IMessageSender GetMessageSender(TypeChannel typeChanel)
    {
        List<IMessageSender> senders = new List<IMessageSender>();
        var settings = _configuration.GetSection("MessageSenderSettings");

        if (typeChanel.HasFlag(TypeChannel.Email))
        {
            var emailSettings = settings.GetSection("Email");
            var smtpServer = emailSettings["SMTPServer"];
            var port = int.Parse(emailSettings["Port"]);
            var username = emailSettings["Username"];
            var password = emailSettings["Password"];

            senders.Add(new EmailSender(smtpServer,port,username,password));

        }

        if (typeChanel.HasFlag(TypeChannel.Telegram))
        {
            var telegramSettings = settings.GetSection("Telegram");
            string botToken = telegramSettings["BotToken"];

            senders.Add(new TelegramSender(botToken));
        }
        
        if (typeChanel.HasFlag(TypeChannel.File))
        {
            var fileSettings = settings.GetSection("File");
            string pathFile = fileSettings["Path"];

            senders.Add(new FileSender(pathFile));
        }

        return new CompositeMessageSender(senders);
    }
}