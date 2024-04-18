using Microsoft.AspNetCore.Mvc;
using NotificationMenedjerAPI.Interfaces;
using NotificationMenedjerAPI.Models;
using NotificationServiceAPI.Interfaces;
using SharedDomainModels;
using SharedInterfaces;

namespace NotificationMenedjerAPI.Apis;

public class RabbitApi : IApi
{
    private const string enpoint = "/rabbit";
    public void Register(WebApplication app)
    {
        app.MapPost($"{enpoint}/sendNotification", async (Notification notification, IRabbitMQService rabbitMQService) =>
        {
            rabbitMQService.SendNotification(notification);
            return Results.Ok(new { Status = $"[{DateTime.UtcNow}] Notification sent" });
        });


        app.MapPost($"{enpoint}/sendNotificationFromGuid", 
            async ([FromQuery] Guid userGuid, [FromQuery] int typeChannel, [FromBody] NotificationDTO notification, 
                IRabbitMQService rabbitMQService, IHttpClientFactory httpClientFactory) =>
        {
            var client = httpClientFactory.CreateClient("UserService");
            string url = $"{client.BaseAddress}user/getUserFromGuid?guid={userGuid}";

            User user = default;
            try
            {

           
            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                user = await response.Content.ReadFromJsonAsync<User>();

            }
            else
            {
                return Results.Problem($"Не смогли получить пользователя {response.StatusCode}");
            }

            }
            catch (Exception e)
            {
                return Results.Problem($"{e.Message}");
            }

            Notification newNot = new Notification();

            newNot.Id                 = Guid.NewGuid();
            newNot.UserGuid           = userGuid;
            newNot.UserId             = user.UserId;
            newNot.Email              = user.Email;
            newNot.TypeChannel        = (TypeChannel)typeChannel;
            newNot.CreatedAt          = DateTime.UtcNow;
            newNot.SendDate           = DateTime.UtcNow;
            newNot.Status             = NotificationStatus.Sent;
            newNot.SubscriptionStatus = SubscriptionStatus.Enable;

            newNot.Theme              = notification.Theme;
            newNot.Message            = notification.Message;


            rabbitMQService.SendNotification(newNot);
            return Results.Ok(new { Status = $"[{DateTime.UtcNow}] Notification sent" });

        });

    }
}