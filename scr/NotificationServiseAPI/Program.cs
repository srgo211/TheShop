
using NotificationServiseAPI;
using NotificationServiseAPI.BL;
using NotificationServiseAPI.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SharedDomainModels;
using System.Text.Json;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Services.AddLogging();

WebApplication app = builder.Build();

ILogger<Program> logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation($"Start app");


// Configuration for RabbitMQ
IConfigurationSection rabbitConfig = builder.Configuration.GetSection("RabbitMQ");
string hostName = rabbitConfig["HostName"];
string userName = rabbitConfig["UserName"];
string password = rabbitConfig["Password"];
string queueName = rabbitConfig["QueueName"];
int portRb = int.Parse(rabbitConfig["Port"]);
int numberOfConsumers = int.Parse(rabbitConfig["NumberOfConsumers"]);



Settings.BotToken = app.Configuration.GetValue<string>("BotConfiguration:BotToken");


ConnectionFactory factory = new ConnectionFactory()
{
    HostName = hostName,
    UserName = userName,
    Password = password,
    Port = portRb,
};
Consumer2(factory, queueName, numberOfConsumers, logger);

// Получение значений URL и порта из конфигурации
string url = app.Configuration.GetValue<string>("AppSettings:Url");
int port = app.Configuration.GetValue<int>("AppSettings:Port");
string baseUrl = $"{url}:{port}";
logger.LogInformation($"Запуск приложения на:{baseUrl}");

app.Run(baseUrl);

void Consumer(ConnectionFactory connectionFactory, string? s, int numberOfConsumers1, ILogger<Program> logger1)
{
    IConnection connection = connectionFactory.CreateConnection();
    IModel channel = connection.CreateModel();
    channel.QueueDeclare(queue: s, durable: true, exclusive: false, autoDelete: false, arguments: null);


    for (int i = 0; i < numberOfConsumers1; i++)
    {
        AsyncEventingBasicConsumer consumer = new AsyncEventingBasicConsumer(channel);
        consumer.Received += async (model, ea) =>
        {
            byte[] body = ea.Body.ToArray();
            Notification notification = JsonSerializer.Deserialize<Notification>(body);
            if (notification != null)
            {
                IMessageSender sender = MessageSenderFactory.GetMessageSender(notification.TypeChannel);
                logger1.LogInformation($"Processing message for {notification.UserGuid}");
                try
                {
                    await sender.SendAsync(notification);
                    logger1.LogInformation($"Successfully sent message for {notification.UserGuid}");
                }
                catch (Exception ex)
                {
                    logger1.LogError(ex, $"Error sending message for {notification.UserGuid}");
                }
            }
            else
            {
                logger1.LogWarning("Received message that couldn't be deserialized.");
            }
        };
        channel.BasicConsume(queue: s, autoAck: true, consumer: consumer);
    }
}

void Consumer2(ConnectionFactory connectionFactory, string? queueName, int numberOfConsumers, ILogger<Program> logger)
{
    MessageSenderFactory.Initialize(builder.Configuration);

    var connection = connectionFactory.CreateConnection();
    var channel = connection.CreateModel();
    channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

    for (int i = 0; i < numberOfConsumers; i++)
    {
        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += async (model, ea) =>
        {
            byte[] body = ea.Body.ToArray();
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

    // Можно также добавить обработку событий закрытия соединения или ошибок для перезапуска потребителей.
}

