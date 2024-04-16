
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using NotificationServiseAPI.BL;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SharedDomainModels;
using SharedInterfaces;

var builder = WebApplication.CreateBuilder(args);

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Services.AddLogging();

var app = builder.Build();

ILogger<Program> logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation($"Start app");


// Configuration for RabbitMQ
var rabbitConfig = builder.Configuration.GetSection("RabbitMQ");
var hostName = rabbitConfig["HostName"];
var userName = rabbitConfig["UserName"];
var password = rabbitConfig["Password"];
var queueName = rabbitConfig["QueueName"];
var numberOfConsumers = int.Parse(rabbitConfig["NumberOfConsumers"]);



var factory = new ConnectionFactory()
{
    HostName = hostName,
    UserName = userName,
    Password = password
};
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();
channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);



for (int i = 0; i < numberOfConsumers; i++)
{
    var consumer = new AsyncEventingBasicConsumer(channel);
    consumer.Received += async (model, ea) =>
    {
        var body = ea.Body.ToArray();
        var notification = JsonSerializer.Deserialize<Notification>(body);
        if (notification != null)
        {
            var sender = MessageSenderFactory.GetMessageSender(notification.TypeChannel);
            logger.LogInformation($"Processing message for {notification.UserGuid}");
            try
            {
                await sender.SendAsync(notification);
                logger.LogInformation($"Successfully sent message for {notification.UserGuid}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error sending message for {notification.UserGuid}");
            }
        }
        else
        {
            logger.LogWarning("Received message that couldn't be deserialized.");
        }
    };
    channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
}

// Получение значений URL и порта из конфигурации
string url = app.Configuration.GetValue<string>("AppSettings:Url");
int port = app.Configuration.GetValue<int>("AppSettings:Port");
string baseUrl = $"{url}:{port}";
logger.LogInformation($"Запуск приложения на:{baseUrl}");

app.Run(baseUrl);

