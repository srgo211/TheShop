

using Microsoft.Extensions.Configuration;
using TelegramBotProject.BusinessLogic;
using TelegramBotProject.Interfaces;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

//BotConfiguration botConfig = builder.Configuration.GetSection("BotConfiguration").Get<BotConfiguration>();


IConfigurationSection configurationSection = builder.Configuration.GetSection("BotConfiguration");
builder.Services.Configure<BotConfiguration>(configurationSection);
string botToken = configurationSection.Get<BotConfiguration>().BotToken ?? string.Empty;



builder.Services.AddHostedService<ConfigureWebhook>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddHttpClient("TelegramWebhook").AddTypedClient<ITelegramBotClient>(httpClient => new TelegramBotClient(botToken, httpClient));
builder.Services.AddHttpClient<IHttpClientService, HttpClientService>();
builder.Services.AddSingleton<BotConfiguration>();
builder.Services.AddSingleton<CommandSwitchController>();

builder.Services.AddScoped<HandleUpdateService>();
builder.Services.AddScoped<CallbackQuerysService>();

WebApplication app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseHttpLogging();

app.MapPost($"/getUpdates", async (
        ITelegramBotClient botClient,
        HttpRequest request,
        HandleUpdateService handleUpdateService,
        NewtonsoftJsonUpdate update) =>
    {

        if (update is null)
        {
            throw new ArgumentException(nameof(update));
        }

        await handleUpdateService.UpdateMessageAsync(update);
       

        

        return Results.Ok();
    })
    .WithName("TelegramWebhook");






app.Run();