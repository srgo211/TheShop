

using Microsoft.Extensions.Configuration;
using TelegramBotProject.BusinessLogic;
using TelegramBotProject.Interfaces;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

//BotConfiguration botConfig = builder.Configuration.GetSection("BotConfiguration").Get<BotConfiguration>();


IConfigurationSection configurationSection = builder.Configuration.GetSection("BotConfiguration");
BotConfiguration? botConfiguration = configurationSection.Get<BotConfiguration>();
string botToken = botConfiguration.BotToken ?? string.Empty;
builder.Services.Configure<BotConfiguration>(configurationSection);


builder.Services.AddHostedService<ConfigureWebhook>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient();

//builder.Services.AddHttpClient("TelegramWebhook").AddTypedClient<ITelegramBotClient>(httpClient => new TelegramBotClient(botToken, httpClient));
//builder.Services.AddHttpClient<IHttpClientService, HttpClientService>();
//builder.Services.AddSingleton<BotConfiguration>();
//builder.Services.AddSingleton<CommandSwitchController>();
//builder.Services.AddScoped<HandleUpdateService>();
//builder.Services.AddScoped<CallbackQuerysService>();



// Add services to the DI container.
builder.Services.AddSingleton<BotConfiguration>(botConfiguration);


builder.Services.AddSingleton<CommandSwitchController>();
builder.Services.AddSingleton<BaseService>();
builder.Services.AddSingleton<ITelegramBotClient>(sp => new TelegramBotClient(botToken));



builder.Services.AddTransient<IHttpClientService, HttpClientService>();
builder.Services.AddTransient<ICommandHandler, CommandHandler>();
builder.Services.AddTransient<ICallbackQueryService, CallbackQueryService>();
builder.Services.AddTransient<IMessageService, MessageService>();
builder.Services.AddTransient<HandleUpdateService>();






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

            await handleUpdateService.HandleUpdateAsync(update);
            return Results.Ok();
        }).WithName("TelegramWebhook");

app.Run();