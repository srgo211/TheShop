using NotificationServiceAPI.Interfaces;
using NotificationServiceAPI.Repositorys;
using SharedDomainModels;
using SharedInterfaces;

namespace NotificationServiceAPI.Apis;

public class NotificationApi : IApi
{
    private const string enpoint = "/notification";
    public void Register(WebApplication app)
    {
        app.MapGet($"{enpoint}/getAll", async (NotificationRepository service) => await service.GetAllNotificationsAsync());
        app.MapGet($"{enpoint}/getUserGuid", async (NotificationRepository service, Guid userGuid) => await service.GetNotificationsByUserGuidAsync(userGuid));
        app.MapGet($"{enpoint}/getUserId", async (NotificationRepository service, long userId) => await service.GetNotificationsByUserIdAsync(userId));
        app.MapGet($"{enpoint}/getIdNotification", async (NotificationRepository service, Guid guidNotigication) => await service.GetNotificationByIdAsync(guidNotigication));
        app.MapGet($"{enpoint}/getStatus", async (NotificationRepository service, SubscriptionStatus subscriptionStatus, NotificationStatus status, DateTime date) => await service.GetNotificationsByStatusAsync(subscriptionStatus, status, date));

        app.MapPost($"{enpoint}/addNotificationFromBd", async (NotificationRepository service, Notification notification) => {
            await service.CreateNotificationAsync(notification);
            return Results.Created($"{enpoint}/getIdNotification/{notification.Id}", notification);
        });

        app.MapPut($"{enpoint}/update/{{guid}}", async (NotificationRepository service, Guid id, Notification notification) => {
            await service.UpdateNotificationAsync(id, notification);
            return Results.Ok(notification);
        });

        app.MapDelete($"{enpoint}/delete/{{guid}}", async (NotificationRepository service, Guid id) => {
            await service.DeleteNotificationAsync(id);
            return Results.Ok($"Notification with ID {id} deleted.");
        });



    }
}