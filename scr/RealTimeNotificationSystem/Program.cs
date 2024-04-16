// Correctly closed namespace usage and ensure all using statements are present
using RealTimeNotificationSystem.Interfaces;
using RealTimeNotificationSystem.Services;
using SharedDomainModels;

IHostBuilder builder = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((hostingContext, config) =>
    {
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        config.AddEnvironmentVariables();
    })
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        logging.AddConsole();
        logging.AddDebug();
    })
    .ConfigureServices((hostContext, services) =>
    {
        services.AddSingleton<IDataProvider<Notification>, MongoDbDataProvider>();
        services.AddSingleton<IMessageSender<Notification>, RabbitMqSender>();
        services.AddSingleton<NotificationService>();
        services.AddHostedService<NotificationWorker>();

        // Access configuration within ConfigureServices
        IConfiguration configuration = hostContext.Configuration;
        string url = configuration.GetValue<string>("AppSettings:Url");
        int port = configuration.GetValue<int>("AppSettings:Port");
        string baseUrl = $"{url}:{port}";

        // Configure Kestrel server to use baseUrl
        services.Configure<Microsoft.AspNetCore.Server.Kestrel.Core.KestrelServerOptions>(options =>
        {
            options.Listen(System.Net.IPAddress.Any, port);
        });
       
    });

builder.Build().Run();