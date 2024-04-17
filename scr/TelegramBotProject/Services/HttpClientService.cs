using SharedInterfaces;
using TelegramBotProject.DTO;
using TelegramBotProject.Interfaces;
using TelegramBotProject.Interfaces.Models;


namespace TelegramBotProject.Services;

public class HttpClientService :  IHttpClientService
{
    private readonly IHttpClientFactory httpClientFactory;
    private readonly IJwtTokenService jwtTokenService;

    private readonly HttpClient httpClient;
    protected readonly BotConfiguration botConfig;


    public HttpClientService(IHttpClientFactory httpClientFactory, BotConfiguration botConfig, IJwtTokenService jwtTokenService)
    {
        httpClientFactory = httpClientFactory;
        httpClient = httpClientFactory.CreateClient();
        this.botConfig = botConfig;
        this.jwtTokenService = jwtTokenService;
    }

    public async Task<HttpResponseMessage> GetAsync(string uri)
    {
        return await httpClient.GetAsync(uri);
    }

    public async Task<IProduct> GetProduct(int page, int itemsPerPage=1)
    {
        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "<Your_JWT_Token>");

        string url = $"{botConfig.HostAddressCatalogProduct}/products/paged?page={page}&itemsPerPage={itemsPerPage}";
        HttpResponseMessage response = await httpClient.GetAsync(url);
        if (response.IsSuccessStatusCode)
        {

            string content = await response.Content.ReadAsStringAsync();
            List<Product>? products = System.Text.Json.JsonSerializer.Deserialize<List<Product>>(content);
        }


        ICollection<Product>? productCol = await httpClient.GetFromJsonAsync<ICollection<Product>>(url);
        return productCol?.FirstOrDefault() ?? new Product();
        

    }

    public async Task<bool> CheckUser(long userId)
    {
        string url = $"{botConfig.HostAddressIdentity}/user/isCheckUserFromUserId?id={userId}";

        HttpResponseMessage response = await httpClient.GetAsync(url);

        bool isUser = false;
        if (response.IsSuccessStatusCode)
        {

            string content = await response.Content.ReadAsStringAsync();

            var dictionary = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, bool>>(content);
            isUser = dictionary["isUser"];
            return isUser;
        }

        return false;

    }

    public async Task<bool> AddUser(long userId)
    {
        string url = $"{botConfig.HostAddressIdentity}/user/addUser";

       

        SharedDomainModels.User user = new SharedDomainModels.User()
        {
            Guid = Guid.NewGuid(),
            UserId = userId,
            TupeRole = TupeRole.user,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            UserName = $"User_{userId}"
        };

        var tokenJwt = jwtTokenService.GenerateJwtToken(user);

        // Add authorization token in the request header
        httpClient.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokenJwt);



        HttpResponseMessage response = await httpClient.PostAsJsonAsync(url, user);
        if (response.IsSuccessStatusCode)
        {
            string responseBody = await response.Content.ReadAsStringAsync();
            return true;
        }
        return false;

    }

    public async Task<SharedDomainModels.User> GetUserFromUserIdAsync(long userId)
    {
        
        string url = $"{botConfig.HostAddressIdentity}/user/getUserFromUserId?id={userId}";

        var response = await httpClient.GetAsync(url);

        if (response.IsSuccessStatusCode)
        {
            // Assuming the server responds with a JSON representation of the User
            SharedDomainModels.User user = await response.Content.ReadFromJsonAsync<SharedDomainModels.User>();
            return user;
        }
        else
        {
            // Log error response or handle it as needed
            Console.WriteLine($"Failed to retrieve user: {response.StatusCode}");
            return default;
        }
    }

    public async Task<bool> UpUser(long userId, string email)
    {
        
        SharedDomainModels.User? user = await GetUserFromUserIdAsync(userId);
        if(user is null) return false;

        user.Email = email;
        var tokenJwt = jwtTokenService.GenerateJwtToken(user);


        httpClient.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokenJwt);

        string url = $"{botConfig.HostAddressIdentity}/user/updateUser?guid={user.Guid}";

        var response = await httpClient.PutAsJsonAsync(url, user);

        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine("User updated successfully.");
            return true;
        }
        else
        {
            string responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Failed to update user: {response.StatusCode}, {responseBody}");
            return false;
        }

    }
}