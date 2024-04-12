using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using TelegramBotProject.DTO;
using TelegramBotProject.Interfaces;
using TelegramBotProject.Interfaces.Models;

namespace TelegramBotProject.Services;

//public class HttpClientService : IHttpClientService
//{
//    private readonly HttpClient _httpClient;

//    public HttpClientService(HttpClient httpClient)
//    {
//        _httpClient = httpClient;
//    }

//    public async Task<HttpResponseMessage> GetAsync(string uri)
//    {
//        return await _httpClient.GetAsync(uri);
//    }


//}

public class HttpClientService :  IHttpClientService
{
    private readonly IHttpClientFactory httpClientFactory;
    private readonly HttpClient httpClient;
    protected readonly BotConfiguration botConfig;

    public HttpClientService(IHttpClientFactory httpClientFactory, BotConfiguration botConfig)
    {
        httpClientFactory = httpClientFactory;
        httpClient = httpClientFactory.CreateClient();
        this.botConfig = botConfig;
    }

    public async Task<HttpResponseMessage> GetAsync(string uri)
    {
        return await httpClient.GetAsync(uri);
    }

    public async Task<IProduct> GetProduct(int page, int itemsPerPage=1)
    {
        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "<Your_JWT_Token>");

        string url = $"{botConfig.HostAddressCatalogProduct}/products/paged?page={page}&itemsPerPage={itemsPerPage}";
        var response = await httpClient.GetAsync(url);
        if (response.IsSuccessStatusCode)
        {

            string content = await response.Content.ReadAsStringAsync();
            var products = System.Text.Json.JsonSerializer.Deserialize<List<Product>>(content);
        }


        var productCol = await httpClient.GetFromJsonAsync<ICollection<Product>>(url);
        return productCol?.FirstOrDefault() ?? new Product();
        

    }
}