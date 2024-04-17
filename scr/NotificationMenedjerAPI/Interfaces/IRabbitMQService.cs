using SharedInterfaces;

namespace NotificationMenedjerAPI.Interfaces;

public interface IRabbitMQService
{
    void SendNotification(INotification notification);
}