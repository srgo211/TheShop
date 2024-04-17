using MongoDB.Driver;
using NotificationMenedjerAPI.Apis;
using NotificationMenedjerAPI.Interfaces;
using NotificationMenedjerAPI.Services;
using NotificationMenedjerAPI.Settings;
using NotificationServiceAPI.Apis;
using NotificationServiceAPI.Interfaces;
using NotificationServiceAPI.Repositorys;

var builder = WebApplication.CreateBuilder(args);

// Create a logger early on
var logger = builder.Services.BuildServiceProvider().GetRequiredService<ILogger<Program>>();

logger.LogInformation("����� ����������");
// ����� ����� �������� ��� ��� �������� ����������� ������������
var configSection = builder.Configuration.GetSection("MongoDB");
var connectionString = configSection["ConnectionString"];
logger.LogInformation($"�������� - ConnectionString: {connectionString}");




RegisterServices(builder.Services, builder.Configuration);
ConfigureLogging(builder.Logging);

var app = builder.Build();

ConfigureApplication(app);


app.Run();



void ConfigureApplication(WebApplication app)
{
    logger.LogInformation("������������ ����������");
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
   
    // ��������� �������� URL � ����� �� ������������
    string url = configuration.GetValue<string>("AppSettings:Url") ?? "https://theshoop.ru";
    int port   = configuration.GetValue<int?>("AppSettings:Port")  ?? 80;

    string baseUrl = $"{url}:{port}";
    logger.LogInformation($"������ ���������� ��:{baseUrl}");
    app.Urls.Add(baseUrl);
}
void RegisterServices(IServiceCollection services, IConfiguration configuration)
{
    logger.LogInformation("����������� ��������");

    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen(opt =>
    {
        opt.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Notification Service", Version = "v1" });
    });

    var mongoDbSettings   =  builder.Configuration.GetSection("MongoDB");
    var connection  =  mongoDbSettings["ConnectionString"] ?? "mongodb://62.113.109.181:27017"; 
    string databaseName   =  mongoDbSettings["DatabaseName"]     ?? "NotificationDb";
    string collectionName =  mongoDbSettings["CollectionName"]   ?? "Notifications";

    logger.LogInformation($"���������� mongoDb:{connection} :{databaseName}");

    // MongoDB configuration
    var mongoClient = new MongoClient(connection);

    services.AddSingleton<IMongoClient>(mongoClient);


    
    var rabbitMqConfig = builder.Configuration.GetSection("RabbitMQ").Get<RabbitMQSettings>();

    builder.Services.AddSingleton(rabbitMqConfig);
    builder.Services.AddSingleton<IRabbitMQService, RabbitMQService>(provider =>
        new RabbitMQService(rabbitMqConfig));




    builder.Services.AddScoped<NotificationRepository>(sp => new NotificationRepository(sp.GetRequiredService<IMongoClient>(), databaseName, collectionName));


    // API registration
    services.AddTransient<IApi, NotificationApi>();
    services.AddTransient<IApi, RabbitApi>();
}
void ConfigureLogging(ILoggingBuilder logging)
{
    logging.ClearProviders();
    logging.AddConsole();
}