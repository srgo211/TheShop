using Microsoft.Extensions.FileProviders;
using TelegramBotProject.Interfaces;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

var logger = builder.Services.BuildServiceProvider().GetRequiredService<ILogger<Program>>();
logger.LogInformation("����� ���������� TelegramBot");

RegisterServices(builder.Services);
WebApplication app = builder.Build();
ConfigureApplication(app);



app.MapPost($"/getUpdates", async (
        ITelegramBotClient botClient,
        HttpRequest request,
        HandleUpdateService handleUpdateService,
        NewtonsoftJsonUpdate update) =>
        {
            logger.LogInformation("���������� �� Webhook");
            if (update is null)
            {
                throw new ArgumentException(nameof(update));
            }

            await handleUpdateService.HandleUpdateAsync(update);
            return Results.Ok();
        }).WithName("TelegramWebhook");

app.Run();




void ConfigureApplication(WebApplication app)
{
    logger.LogInformation("������������ ����������");

    // ��������� ����������� ������
    app.UseStaticFiles();

    var pathStatic = Path.Combine(Directory.GetCurrentDirectory(), "img");
    logger.LogInformation($"�����: {pathStatic}");
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

    // ��������� �������� URL � ����� �� ������������
    string url = configuration.GetValue<string>("AppSettings:Url") ?? "http://localhost";
    int port = configuration.GetValue<int?>("AppSettings:Port")    ?? 5000;

    string baseUrl = $"{url}:{port}";
    logger.LogInformation($"������ ���������� ��:{baseUrl}");
    app.Urls.Add(baseUrl);
}

void RegisterServices(IServiceCollection services)
{
    logger.LogInformation("����������� ��������");
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


    builder.Services.AddSingleton<CommandSwitchController>();
    builder.Services.AddSingleton<BaseService>();
    builder.Services.AddSingleton<ITelegramBotClient>(sp => new TelegramBotClient(botToken));



    builder.Services.AddScoped<IHttpClientService, HttpClientService>();

    builder.Services.AddTransient<ICommandHandler, CommandHandler>();
    builder.Services.AddTransient<ICallbackQueryService, CallbackQueryService>();
    builder.Services.AddTransient<IMessageService, MessageService>();
    builder.Services.AddTransient<HandleUpdateService>();


}