using NotificationServiceAPI.DTO;
using NotificationServiceAPI.Interfaces;

namespace NotificationServiceAPI.Apis;

public class NotificationApi : IApi
{
    public void Register(WebApplication app)
    {
        // Notification endpoints
        app.MapPost("/notifications/Add", async (Guid id, Notification notification, INotificationRepository notificationRepository) =>
        {
            await notificationRepository.AddNotificationToUserAsync(id, notification);
            return Results.Created($"/users/{id}/notifications/{notification.Id}", notification);
        });


        app.MapDelete("/notifications/Delete", async (Guid id, Guid notificationId, INotificationRepository notificationRepository) =>
        {
            await notificationRepository.DeleteNotificationFromUserAsync(id, notificationId);
            return Results.NoContent();
        });

        app.MapGet("/notifications/statusNotification", async (NotificationStatus status, INotificationRepository notificationRepository) =>
        {
            var users = await notificationRepository.GetAllUsersWithNotificationsByStatusAsync(status);
            return Results.Ok(users);
        });

        app.MapGet("/notifications/filterStatus", async (NotificationStatus notificationStatus, SubscriptionStatus subscriptionStatus, INotificationRepository notificationRepository) =>
        {
            var users = await notificationRepository.GetUsersAndNotificationsByStatusAsync(notificationStatus, subscriptionStatus);
            return Results.Ok(users);
        });


        app.MapGet("/notifications/filterStatusAndDate", async (NotificationStatus notificationStatus, SubscriptionStatus subscriptionStatus, DateTime sendDate, INotificationRepository notificationRepository) =>
        {
            var users = await notificationRepository.GetUsersNotificationsByStatusAndSendDateAsync(notificationStatus, subscriptionStatus, sendDate);
            return Results.Ok(users);
        });


        app.MapPut("/notifications/Update", async (Guid notificationId, Notification updateModel, INotificationRepository notificationRepository) =>
        {
            // Convert the update model to a Notification object. This step may vary based on your model design.
            var updatedNotification = new Notification
            {
                Message  = updateModel.Message,
                SendDate = updateModel.SendDate,
                Status   = updateModel.Status
            };
            
            var success = await notificationRepository.UpdateNotificationAsync(notificationId, updatedNotification);

            return success ? Results.Ok() : Results.NotFound();
        });

    }
}