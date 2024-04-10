using MongoDB.Driver;
using NotificationServiceAPI.DTO;
using NotificationServiceAPI.Interfaces;

namespace NotificationServiceAPI.Repositorys;

public class UserRepository : IUserRepository
{
    private readonly IMongoCollection<User> _usersCollection;

    public UserRepository(IMongoClient mongoClient, string databaseName)
    {
        _usersCollection = mongoClient.GetDatabase(databaseName).GetCollection<User>("Users");


        if (mongoClient == null) throw new ArgumentNullException(nameof(mongoClient));
        if (string.IsNullOrEmpty(databaseName)) throw new ArgumentException("Message", nameof(databaseName));

        _usersCollection = mongoClient.GetDatabase(databaseName).GetCollection<User>("Users");

    }

    public async Task<User> CreateUserAsync(User user)
    {
        if (user == null) throw new ArgumentNullException(nameof(user));
        await _usersCollection.InsertOneAsync(user);
        return user;
    }

    public async Task<User> GetUserByIdAsync(Guid id)
    {
        var filter = Builders<User>.Filter.Eq(u => u.Id, id);
        return await _usersCollection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task UpdateUserByIdAsync(Guid id, User user)
    {
        if (user == null) throw new ArgumentNullException(nameof(user));
        var filter = Builders<User>.Filter.Eq(u => u.Id, id);
        var update = Builders<User>.Update
            .Set(u => u.Email, user.Email)
            .Set(u => u.Status, user.Status)
            .Set(u => u.Notifications, user.Notifications);

        await _usersCollection.UpdateOneAsync(filter, update);
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _usersCollection.Find(_ => true).ToListAsync();
    }

    public async Task DeleteUsersAsync(Guid id)
    {
        var filter = Builders<User>.Filter.Eq(u => u.Id, id);
        await _usersCollection.DeleteOneAsync(filter);
    }

    public async Task<User> GetUserByEmailAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be null or whitespace.", nameof(email));

        return await _usersCollection.Find(user => user.Email == email).FirstOrDefaultAsync();
    }

    public async Task<User> GetUserByUserIdAsync(long userId)
    {
       
        return await _usersCollection.Find(user => user.UserId == userId).FirstOrDefaultAsync();
    }


}