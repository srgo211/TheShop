namespace RealTimeNotificationSystem.Services;

public class NotificationWorker : BackgroundService
{
    private readonly NotificationService notificationService;
    private readonly IConfiguration configuration;

    public NotificationWorker(NotificationService notificationService, IConfiguration configuration)
    {
        this.notificationService = notificationService;
        this.configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        int interval = configuration.GetValue<int>("NotificationFetchIntervalMinutes", 60);
        while (!stoppingToken.IsCancellationRequested)
        {
            await notificationService.ProcessNotifications();
            await Task.Delay(TimeSpan.FromMinutes(interval), stoppingToken);
        }
    }
}