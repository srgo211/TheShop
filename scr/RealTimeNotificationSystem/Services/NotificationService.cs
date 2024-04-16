using RealTimeNotificationSystem.Interfaces;
using SharedDomainModels;

namespace RealTimeNotificationSystem.Services;

public class NotificationService
{
    private readonly IDataProvider<Notification> dataProvider;
    private readonly IMessageSender<Notification> messageSender;
    private readonly ILogger<NotificationService> logger;

    public NotificationService(IDataProvider<Notification> dataProvider, IMessageSender<Notification> messageSender, ILogger<NotificationService> logger)
    {
        this.dataProvider = dataProvider;
        this.messageSender = messageSender;
        this.logger = logger;
    }

    public async Task ProcessNotifications()
    {
        logger.LogInformation("Начинаем обрабатывать уведомления");
        IEnumerable<Notification> notifications = await dataProvider.FetchData();
        foreach (Notification notification in notifications)
        {
            logger.LogInformation($"Отправка уведомления ID: {notification.Id} в очередь сообщений.");
            await messageSender.SendMessageAsync(notification);
        }
        logger.LogInformation("Завершена обработка уведомлений.");
    }
}