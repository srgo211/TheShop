using RabbitMQ.Client;
using TelegramBotProject.Interfaces;

namespace TelegramBotProject.Services;

public class RabbitMQConnectionManager : IRabbitMQConnectionManager, IDisposable
{
    private readonly ConnectionFactory _connectionFactory;
    private IConnection _connection;

    public RabbitMQConnectionManager(string hostname)
    {
        _connectionFactory = new ConnectionFactory() { HostName = hostname };
        _connection = _connectionFactory.CreateConnection();
    }

    public IModel CreateChannel(string queueName)
    {
        var channel = _connection.CreateModel();
        channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
        return channel;
    }

    public void Dispose()
    {
        _connection?.Dispose();
    }
}