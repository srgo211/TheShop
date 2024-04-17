namespace NotificationMenedjerAPI.Settings;

public class RabbitMQSettings
{
    public string HostName { get; set; } = "localhost";
    public string UserName { get; set; } = "guest";
    public string Password { get; set; } = "guest";
    public string QueueName { get; set; } = "notifications";
    public int Port { get; set; } = 5672;
    public int NumberOfConsumers { get; set; } = 1;
}