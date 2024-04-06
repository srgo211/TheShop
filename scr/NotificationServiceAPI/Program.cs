using MongoDB.Driver;
using NotificationServiceAPI.Interfaces;
using NotificationServiceAPI.Repositorys;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// MongoDB configuration
var mongoClient = new MongoClient(builder.Configuration["MongoDB:ConnectionString"]);
var databaseName = builder.Configuration["MongoDB:DatabaseName"];

// Register repositories with DI container
builder.Services.AddSingleton<IMongoClient>(mongoClient);
builder.Services.AddScoped<IUserRepository>(sp => new UserRepository(mongoClient, databaseName));
builder.Services.AddScoped<INotificationRepository>(sp => new NotificationRepository(mongoClient, databaseName));

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();