using MongoDB.Driver;
using NotificationServiceAPI.DTO;
using NotificationServiceAPI.Interfaces;

namespace NotificationServiceAPI.Repositorys;

public class UserRepository : IUserRepository
{
    private readonly IMongoCollection<User> _userCollection;

    public UserRepository(IMongoClient mongoClient, string databaseName)
    {
        _userCollection = mongoClient.GetDatabase(databaseName).GetCollection<User>("Users");
    }

    public async Task<User> CreateUserAsync(User user)
    {
        if (user == null) throw new ArgumentNullException(nameof(user));
        await _userCollection.InsertOneAsync(user);
        return user;
    }

    public async Task<User> GetUserByIdAsync(Guid id)
    {
        var filter = Builders<User>.Filter.Eq(u => u.Id, id);
        return await _userCollection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task UpdateUserByIdAsync(Guid id, User user)
    {
        if (user == null) throw new ArgumentNullException(nameof(user));
        var filter = Builders<User>.Filter.Eq(u => u.Id, id);
        var update = Builders<User>.Update
            .Set(u => u.Email, user.Email)
            .Set(u => u.Status, user.Status)
            .Set(u => u.Notifications, user.Notifications);

        await _userCollection.UpdateOneAsync(filter, update);
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _userCollection.Find(_ => true).ToListAsync();
    }

    public async Task DeleteUsersAsync(Guid userId)
    {
        var filter = Builders<User>.Filter.Eq(u => u.Id, userId);
        await _userCollection.DeleteOneAsync(filter);
    }
}