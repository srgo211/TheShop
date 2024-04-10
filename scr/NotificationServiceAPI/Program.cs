using MongoDB.Driver;
using MongoDB.Driver.Core.Connections;
using NotificationServiceAPI.Apis;
using NotificationServiceAPI.Interfaces;
using NotificationServiceAPI.Repositorys;

var builder = WebApplication.CreateBuilder(args);

// Create a logger early on
var logger = builder.Services.BuildServiceProvider().GetRequiredService<ILogger<Program>>();

logger.LogInformation("Старт приложения");
// Здесь можно добавить код для проверки содержимого конфигурации
var configSection = builder.Configuration.GetSection("MongoDB");
var connectionString = configSection["ConnectionString"];
logger.LogInformation($"Проверка - ConnectionString: {connectionString}");




RegisterServices(builder.Services, builder.Configuration);
ConfigureLogging(builder.Logging);

var app = builder.Build();

ConfigureApplication(app);


app.Run();

void ConfigureApplication(WebApplication app)
{
    logger.LogInformation("Конфигурация приложения");
    // Applying middleware
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseHttpsRedirection();

    // Register API routes
    var apis = app.Services.GetServices<IApi>();
    foreach (var api in apis)
    {
        if (api is null) throw new InvalidProgramException("Api not found");
        api.Register(app);
    }

    IConfiguration configuration = app.Configuration;
   
    // Получение значений URL и порта из конфигурации
    string url = configuration.GetValue<string>("AppSettings:Url") ?? "https://theshoop.ru";
    int port   = configuration.GetValue<int?>("AppSettings:Port")  ?? 80;

    string baseUrl = $"{url}:{port}";
    logger.LogInformation($"Запуск приложения на:{baseUrl}");
    app.Urls.Add(baseUrl);
}
void RegisterServices(IServiceCollection services, IConfiguration configuration)
{
    Console.WriteLine("Регистрация сервисов");

    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen(opt =>
    {
        opt.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Notification Service", Version = "v1" });
    });

    var mongoDbSettings = builder.Configuration.GetSection("MongoDB");
    var connection = mongoDbSettings["ConnectionString"] ?? "mongodb://62.113.109.181:27017"; ;
    string databaseName =  mongoDbSettings["DatabaseName"] ?? "NotificationDb";

    logger.LogInformation($"Подключаем mongoDb:{connection} :{databaseName}");

    // MongoDB configuration
    var mongoClient = new MongoClient(connection);

    services.AddSingleton<IMongoClient>(mongoClient);

    builder.Services.AddScoped<IUserRepository>(sp => new UserRepository(mongoClient, databaseName));
    builder.Services.AddScoped<INotificationRepository>(sp => new NotificationRepository(mongoClient, databaseName));

    // API registration
    services.AddTransient<IApi, UserApi>();
    services.AddTransient<IApi, NotificationApi>();
}
void ConfigureLogging(ILoggingBuilder logging)
{
    logging.ClearProviders();
    logging.AddConsole();
}