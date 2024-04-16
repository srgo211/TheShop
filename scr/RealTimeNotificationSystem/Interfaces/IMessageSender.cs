namespace RealTimeNotificationSystem.Interfaces;

public interface IMessageSender<T>
{
    Task SendMessageAsync(T message);
}