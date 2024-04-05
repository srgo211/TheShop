using TelegramBotProject.Interfaces;

namespace TelegramBotProject.Services;

public class HttpClientService : IHttpClientService
{
    private readonly HttpClient _httpClient;

    public HttpClientService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<HttpResponseMessage> GetAsync(string uri)
    {
        return await _httpClient.GetAsync(uri);
    }

    
}