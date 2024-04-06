using MongoDB.Driver;
using NotificationServiceAPI.Apis;
using NotificationServiceAPI.Interfaces;
using NotificationServiceAPI.Repositorys;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

await Configure(app);

var apis = app.Services.GetServices<IApi>();
foreach (var api in apis)
{
    if (api is null) throw new InvalidProgramException("Api not found");
    api.Register(app);
}
app.Run();

async Task Configure(WebApplication app)
{
    
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    app.UseHttpsRedirection();
}
void RegisterServices(IServiceCollection services)
{
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen(opt =>
    {
        opt.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Сервис уведомлений", Version = "v1" });
       
    });

    // MongoDB configuration
    var mongoClient = new MongoClient(builder.Configuration["MongoDB:ConnectionString"]);
    var databaseName = builder.Configuration["MongoDB:DatabaseName"];

    // Register repositories with DI container
    builder.Services.AddSingleton<IMongoClient>(mongoClient);
    builder.Services.AddScoped<IUserRepository>(sp => new UserRepository(mongoClient, databaseName));
    builder.Services.AddScoped<INotificationRepository>(sp => new NotificationRepository(mongoClient, databaseName));

    services.AddTransient<IApi, UserApi>();
    services.AddTransient<IApi, NotificationApi>();
    services.AddScoped<INotificationRepository, NotificationRepository>();
    services.AddScoped<IUserRepository, UserRepository>();
}