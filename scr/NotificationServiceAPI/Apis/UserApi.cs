using NotificationServiceAPI.DTO;
using NotificationServiceAPI.Interfaces;

namespace NotificationServiceAPI.Apis;

public class UserApi : IApi
{
    const string endpoint = "/users";

    public void Register(WebApplication app)
    {
        app.MapPost($"{endpoint}/addUser", async (User user, IUserRepository userRepository) =>
        {
            await userRepository.CreateUserAsync(user);
            return Results.Created($"/getUser/{user.Id}", user);
        });


        app.MapGet(endpoint + "/getUser", async (Guid id, IUserRepository userRepository) =>
        {
            var user = await userRepository.GetUserByIdAsync(id);
            return user != null ? Results.Ok(user) : Results.NotFound();
        });

        app.MapGet($"{endpoint}/getEmail", async (string email, IUserRepository userRepository) =>
        {
            var user = await userRepository.GetUserByEmailAsync(email);
            return user != null ? Results.Ok(user) : Results.NotFound();
        });

        app.MapGet($"{endpoint}/getUserId", async (long userId, IUserRepository userRepository) =>
        {
            var user = await userRepository.GetUserByUserIdAsync(userId);
            return user != null ? Results.Ok(user) : Results.NotFound();
        });


        app.MapPut($"{endpoint}/update", async (Guid id, User user, IUserRepository userRepository) =>
        {
            await userRepository.UpdateUserByIdAsync(id, user);
            return Results.Ok();
        });


        app.MapDelete($"{endpoint}/delete", async (Guid id, IUserRepository userRepository) =>
        {
            await userRepository.DeleteUsersAsync(id);
            return Results.NoContent();
        });

    }
}