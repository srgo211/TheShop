using RealTimeNotificationSystem.Interfaces;
using SharedDomainModels;
using SharedInterfaces;

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
        IEnumerable<Notification> notifications = await dataProvider.FetchDataAsync();
        foreach (Notification notification in notifications)
        {
            logger.LogInformation($"Отправка уведомления ID: {notification.Id} в очередь сообщений.");
            await messageSender.SendMessageAsync(notification);

            await UpdateNotification(notification);
        }
        logger.LogInformation("Завершена обработка уведомлений.");
    }

    private async Task UpdateNotification(Notification notification)
    {
        notification.Status = NotificationStatus.Sent;
        notification.SubscriptionStatus = SubscriptionStatus.Enable;
        await dataProvider.UpdateDataAsync(notification);
    }
}