using NotificationMenedjerAPI.Interfaces;
using NotificationMenedjerAPI.Settings;
using RabbitMQ.Client;
using SharedInterfaces;
using System.Text;
using System.Text.Json;

namespace NotificationMenedjerAPI.Services;

public class RabbitMQService : IRabbitMQService, IDisposable
{
    private readonly IConnection connection;
    private readonly IModel channel;
    private readonly string queueName;

    public RabbitMQService(RabbitMQSettings settings)
    {
        var factory = new ConnectionFactory()
        {
            HostName = settings.HostName,
            UserName = settings.UserName,
            Password = settings.Password,
            Port = settings.Port
        };
        connection = factory.CreateConnection();
        channel = connection.CreateModel();

        queueName = settings.QueueName;
        channel.QueueDeclare(queue: queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);
    }

    public void SendNotification(INotification notification)
    {
        var json = JsonSerializer.Serialize(notification);
        var body = Encoding.UTF8.GetBytes(json);

        channel.BasicPublish(exchange: "",
            routingKey: queueName,
            basicProperties: null,
            body: body);
    }

    public void Dispose()
    {
        channel?.Close();
        connection?.Close();
    }
}