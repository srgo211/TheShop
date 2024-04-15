using Microsoft.Extensions.FileProviders;
using RabbitMQ.Client;
using SharedInterfaces;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using SharedDomainModels;
using TelegramBotProject.Interfaces;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

ILogger<Program> logger = builder.Services.BuildServiceProvider().GetRequiredService<ILogger<Program>>();
logger.LogInformation("Старт приложения TelegramBot");

RegisterServices(builder.Services);
WebApplication app = builder.Build();
ConfigureApplication(app);



app.MapPost($"/getUpdates", async (
        ITelegramBotClient botClient,
        HttpRequest request,
        HandleUpdateService handleUpdateService,
        NewtonsoftJsonUpdate update) =>
        {
            logger.LogInformation("Обновление от Webhook");
            if (update is null)
            {
                throw new ArgumentException(nameof(update));
            }

            await handleUpdateService.HandleUpdateAsync(update);
            return Results.Ok();
        }).WithName("TelegramWebhook")
          .WithMetadata(new ApiExplorerSettingsAttribute { IgnoreApi = true }); 


app.MapPost("/send", async (string queueName, Notification message, IRabbitMQConnectionManager rabbitMQ) =>
{
    using var channel = rabbitMQ.CreateChannel(queueName);

    var json = System.Text.Json.JsonSerializer.Serialize(message);
    var body = Encoding.UTF8.GetBytes(json);

    channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: body);
    return Results.Ok($"Сообщение отправлено  на: {queueName}:\n[{message.Theme}]\n{message.Message}");
});


app.Run();




void ConfigureApplication(WebApplication app)
{
    logger.LogInformation("Конфигурация приложения");

    // Настройка статических файлов
    app.UseStaticFiles();

    string pathStatic = Path.Combine(Directory.GetCurrentDirectory(), "img");
    logger.LogInformation($"Файлы: {pathStatic}");
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(pathStatic),
        RequestPath = "/img"
    });



    // Applying middleware
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseHttpsRedirection();
    app.UseHttpLogging();


    IConfiguration configuration = app.Configuration;

    // Получение значений URL и порта из конфигурации
    string url = configuration.GetValue<string>("AppSettings:Url") ?? "http://localhost";
    int port = configuration.GetValue<int?>("AppSettings:Port")    ?? 5000;

    string baseUrl = $"{url}:{port}";
    logger.LogInformation($"Запуск приложения на:{baseUrl}");
    app.Urls.Add(baseUrl);
}

void RegisterServices(IServiceCollection services)
{
    logger.LogInformation("Регистрация сервисов");
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen(opt =>
    {
        opt.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Telegram Bot", Version = "v1" });
        
    });

    IConfigurationSection configurationSection = builder.Configuration.GetSection("BotConfiguration");
    BotConfiguration? botConfiguration = configurationSection.Get<BotConfiguration>();
    string botToken = botConfiguration.BotToken ?? string.Empty;
    builder.Services.Configure<BotConfiguration>(configurationSection);


    builder.Services.AddHostedService<ConfigureWebhook>();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.AddHttpClient();



    // Add services to the DI container.
    builder.Services.AddSingleton<BotConfiguration>(botConfiguration);

    // Регистрация фабрики подключений к RabbitMQ
    builder.Services.AddSingleton<IRabbitMQConnectionManager>(new RabbitMQConnectionManager("localhost"));



    builder.Services.AddSingleton<CommandSwitchController>();
    builder.Services.AddSingleton<BaseService>();
    builder.Services.AddSingleton<ITelegramBotClient>(sp => new TelegramBotClient(botToken));



    builder.Services.AddScoped<IHttpClientService, HttpClientService>();

    builder.Services.AddTransient<ICommandHandler, CommandHandler>();
    builder.Services.AddTransient<ICallbackQueryService, CallbackQueryService>();
    builder.Services.AddTransient<IMessageService, MessageService>();
    builder.Services.AddTransient<HandleUpdateService>();


}