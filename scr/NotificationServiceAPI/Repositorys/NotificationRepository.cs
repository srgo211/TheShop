using MongoDB.Driver;
using NotificationServiceAPI.DTO;
using NotificationServiceAPI.Interfaces;

namespace NotificationServiceAPI.Repositorys;

public class NotificationRepository : INotificationRepository
{
    private readonly IMongoCollection<User> _usersCollection;

    public NotificationRepository(IMongoClient mongoClient, string databaseName)
    {
        _usersCollection = mongoClient.GetDatabase(databaseName).GetCollection<User>("Users");
    }

    public async Task AddNotificationToUserAsync(Guid id , Notification notification)
    {
        var filter = Builders<User>.Filter.Eq(u => u.Id, id);
        var update = Builders<User>.Update.Push(u => u.Notifications, notification);
        await _usersCollection.UpdateOneAsync(filter, update);
    }

    public async Task AddNotificationsToUserAsync(Guid id, List<Notification> notifications)
    {
        var filter = Builders<User>.Filter.Eq(u => u.Id, id);
        var update = Builders<User>.Update.PushEach(u => u.Notifications, notifications);
        await _usersCollection.UpdateOneAsync(filter, update);
    }

    public async Task UpdateUserNotificationAsync(Guid id, Notification notification)
    {
        var userFilter = Builders<User>.Filter.Eq(u => u.Id, id);
        var notificationFilter = Builders<User>.Filter.ElemMatch(u => u.Notifications, n => n.Id == notification.Id);
        var combinedFilter = Builders<User>.Filter.And(userFilter, notificationFilter);

        var update = Builders<User>.Update.Set(u => u.Notifications[-1], notification);
        await _usersCollection.UpdateOneAsync(combinedFilter, update);
    }

    public async Task DeleteNotificationFromUserAsync(Guid id, Guid notificationId)
    {
        var filter = Builders<User>.Filter.Eq(u => u.Id, id);
        var update = Builders<User>.Update.PullFilter(u => u.Notifications, n => n.Id == notificationId);
        await _usersCollection.UpdateOneAsync(filter, update);
    }
}