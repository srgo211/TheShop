using Microsoft.EntityFrameworkCore;
using SharedDomainModels;

namespace IdentityAPI.Repositorys;

public class UserRepository : Interfaces.Repositorys.IUserRepository
{
    private readonly AppDbContext dbContext;

    public UserRepository(AppDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<User> CreateAsync(User user)
    {
        dbContext.Add(user);
        await dbContext.SaveChangesAsync();
        return user;
    }

    public async Task<User> UpdateAsync(User user)
    {
        dbContext.Update(user);
        await dbContext.SaveChangesAsync();
        return user;
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await dbContext.Set<User>().ToListAsync();
    }

    public async Task<User> GetByIdAsync(Guid id)
    {
        return await dbContext.Set<User>().FindAsync(id);
    }

    public async Task DeleteAsync(Guid id)
    {
        User? user = await dbContext.Set<User>().FindAsync(id);
        if (user != null)
        {
            dbContext.Remove(user);
            await dbContext.SaveChangesAsync();
        }
    }
}