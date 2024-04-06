using NotificationServiceAPI.DTO;

namespace NotificationServiceAPI.Interfaces;

public interface IUserRepository
{
    Task<User> CreateUserAsync(User user);
    Task<User> GetUserByIdAsync(Guid id);
    Task UpdateUserByIdAsync(Guid id, User user);
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task DeleteUsersAsync(Guid userId);

}