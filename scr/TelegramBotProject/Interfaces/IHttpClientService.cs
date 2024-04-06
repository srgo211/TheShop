using TelegramBotProject.Interfaces.Models;

namespace TelegramBotProject.Interfaces;

public interface IHttpClientService
{
    Task<HttpResponseMessage> GetAsync(string uri);
    Task<IProduct> GetProduct(int page, int itemsPerPage);
}