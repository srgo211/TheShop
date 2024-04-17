using NotificationMenedjerAPI.Interfaces;
using NotificationServiceAPI.Interfaces;
using SharedDomainModels;

namespace NotificationMenedjerAPI.Apis;

public class RabbitApi : IApi
{
    private const string enpoint = "/rabbit";
    public void Register(WebApplication app)
    {
        app.MapPost($"{enpoint}/sendNotification", async (Notification notification, IRabbitMQService rabbitMQService) =>
        {
            rabbitMQService.SendNotification(notification);
            return Results.Ok(new { Status = "Notification sent" });
        });
    }
}