using NotificationServiseAPI.Interfaces;
using SharedDomainModels;
using System.Net.Mail;
using System.Net;

namespace NotificationServiseAPI.BL.Chanels;

public class EmailSender : IMessageSender
{
    private readonly string smtpServer;
    private readonly string email;
    private readonly int port;
    private readonly string username;
    private readonly string password;

    public EmailSender(string smtpServer,string email, int port, string username, string password)
    {
        this.smtpServer = smtpServer;
        this.port = port;
        this.username = username;
        this.password = password;
        this.email = email;
    }


    public async Task SendAsync(Notification notification)
    {
        if (notification?.Email is null) return;

        try
        {
            using (var smtpClient = new SmtpClient(smtpServer))
            {
                smtpClient.Port = port;
                smtpClient.EnableSsl = true;
                smtpClient.Credentials = new NetworkCredential(username, password);
                
                
                using (var mailMessage = new MailMessage())
                {
                    mailMessage.From = new MailAddress(email);
                    mailMessage.To.Add(notification.Email);
                    mailMessage.Subject = notification.Theme;
                    mailMessage.Body = notification.Message;
                    mailMessage.IsBodyHtml = true;  

                    await smtpClient.SendMailAsync(mailMessage);
                   
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to send email. Error: {ex.Message}");
        }
    }
}