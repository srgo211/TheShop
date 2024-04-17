using NotificationServiseAPI.Interfaces;
using SharedDomainModels;

namespace NotificationServiseAPI.BL.Chanels;

public class TelegramSender : IMessageSender
{
    private static readonly HttpClient client = new HttpClient();

    private readonly string botToken;
    public TelegramSender(string botToken)
    {
        this.botToken = botToken;
      
    }

    public async Task SendAsync(Notification notification)
    {
        string chatId = notification.UserId.ToString();

        string text = $"[{notification.Id}]\n\n<u>{notification.Theme}</u>\n\n{notification.Message}";

        string uri = $"https://api.telegram.org/bot{botToken}/sendMessage";

        var values = new Dictionary<string, string>
        {
            { "chat_id", chatId },
            { "text", text },
            { "parse_mode", "HTML" }
        };

        try
        {
           
            var content = new FormUrlEncodedContent(values);

            var response = await client.PostAsync(uri, content);

            var responseString = await response.Content.ReadAsStringAsync();
            Console.WriteLine(responseString);
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine("\nException Caught!");
            Console.WriteLine("Message :{0} ", e.Message);
        }
    }
}