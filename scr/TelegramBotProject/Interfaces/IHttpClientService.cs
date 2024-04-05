namespace TelegramBotProject.Interfaces;

public interface IHttpClientService
{
    Task<HttpResponseMessage> GetAsync(string uri);
}