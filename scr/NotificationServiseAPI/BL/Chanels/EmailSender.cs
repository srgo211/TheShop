using NotificationServiseAPI.Interfaces;
using SharedDomainModels;

namespace NotificationServiseAPI.BL.Chanels;

public class EmailSender : IMessageSender
{
    private readonly string smtpServer;
    private readonly int port;
    private readonly string username;
    private readonly string password;

    public EmailSender(string smtpServer, int port, string username, string password)
    {
        this.smtpServer = smtpServer;
        this.port = port;
        this.username = username;
        this.password = password;
    }


    public async Task SendAsync(Notification notification)
    {
        await Task.Delay(2000);
    }
}