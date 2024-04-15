using RabbitMQ.Client;
using SharedInterfaces;

namespace TelegramBotProject.Interfaces;

public interface IRabbitMQConnectionManager
{
    IModel CreateChannel(string queueName);
    
}