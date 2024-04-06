using NotificationServiceAPI.DTO;
using NotificationServiceAPI.Interfaces;

namespace NotificationServiceAPI.Apis;

public class NotificationApi : IApi
{
    public void Register(WebApplication app)
    {
        // Notification endpoints
        app.MapPost("/users/{userId}/notifications", async (Guid id, Notification notification, INotificationRepository notificationRepository) =>
        {
            await notificationRepository.AddNotificationToUserAsync(id, notification);
            return Results.Created($"/users/{id}/notifications/{notification.Id}", notification);
        });


        app.MapDelete("/users/{userId}/notifications/{notificationId}", async (Guid id, Guid notificationId, INotificationRepository notificationRepository) =>
        {
            await notificationRepository.DeleteNotificationFromUserAsync(id, notificationId);
            return Results.NoContent();
        });
    }
}