using SharedDomainModels;

namespace IdentityAPI.Interfaces.Repositorys;

public interface IUserRepository
{
    Task<User> CreateAsync(User user);
    Task<User> UpdateAsync(User user);
    Task<IEnumerable<User>> GetAllAsync();
    Task<User> GetByIdAsync(Guid id);
    Task<User> GetByUserIdAsync(long userId);
    Task DeleteAsync(Guid id);
}