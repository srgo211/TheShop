using RabbitMQ.Client;
using RealTimeNotificationSystem.Interfaces;
using SharedDomainModels;
using System.Text;

namespace RealTimeNotificationSystem.Services;

public class RabbitMqSender : IMessageSender<Notification>
{
    private readonly IConnection connection;
    private readonly IModel channel;
    private readonly ILogger<RabbitMqSender> logger;

    public RabbitMqSender(IConfiguration configuration, ILogger<RabbitMqSender> logger)
    {

       

        ConnectionFactory factory = new ConnectionFactory()
        {
            HostName = configuration["RabbitMQ:HostName"],
            UserName = configuration["RabbitMQ:UserName"],
            Password = configuration["RabbitMQ:Password"],
            Port     = int.Parse(configuration["RabbitMQ:Port"]),
        };

        string queueName = configuration["RabbitMQ:QueueName"];


        this.connection = factory.CreateConnection();
        this.channel = connection.CreateModel();
        this.channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
        this.logger = logger;
    }

    public Task SendMessageAsync(Notification message)
    {
        logger.LogInformation($"Отправляем уведомление ID:{message.Id} в RabbitMQ.");
        byte[] body = Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(message));
        channel.BasicPublish(exchange: "", routingKey: "notifications", basicProperties: null, body: body);
        return Task.CompletedTask;
    }

    public Task UpdateMessageAsync(Notification message)
    {
        logger.LogInformation($"Обновляем уведомление ID:{message.Id}");


       
        return Task.CompletedTask;
    }

}