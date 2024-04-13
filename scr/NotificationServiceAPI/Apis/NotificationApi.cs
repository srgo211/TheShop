using NotificationServiceAPI.DTO;
using NotificationServiceAPI.Interfaces;
using NotificationServiceAPI.Repositorys;

namespace NotificationServiceAPI.Apis;

public class NotificationApi : IApi
{
    public void Register(WebApplication app)
    {
        app.MapGet("/notifications", async (NotificationRepository service) => await service.GetAllNotificationsAsync());
        app.MapGet("/notifications/userGuid/{userGuid}", async (NotificationRepository service, Guid userGuid) => await service.GetNotificationsByUserGuidAsync(userGuid));
        app.MapGet("/notifications/userId/{userId}", async (NotificationRepository service, long userId) => await service.GetNotificationsByUserIdAsync(userId));
        app.MapGet("/notifications/id/{id}", async (NotificationRepository service, Guid id) => await service.GetNotificationByIdAsync(id));
        app.MapGet("/notifications/status", async (NotificationRepository service, SubscriptionStatus subscriptionStatus, NotificationStatus status, DateTime date) => await service.GetNotificationsByStatusAsync(subscriptionStatus, status, date));

        app.MapPost("/notifications", async (NotificationRepository service, Notification notification) => {
            await service.CreateNotificationAsync(notification);
            return Results.Created($"/notifications/id/{notification.Id}", notification);
        });

        app.MapPut("/notifications/{id}", async (NotificationRepository service, Guid id, Notification notification) => {
            await service.UpdateNotificationAsync(id, notification);
            return Results.Ok(notification);
        });

        app.MapDelete("/notifications/{id}", async (NotificationRepository service, Guid id) => {
            await service.DeleteNotificationAsync(id);
            return Results.Ok($"Notification with ID {id} deleted.");
        });


    }
}