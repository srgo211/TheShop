using MongoDB.Driver;
using NotificationServiceAPI.Interfaces;
using SharedDomainModels;
using SharedInterfaces;

namespace NotificationServiceAPI.Repositorys;

public class NotificationRepository : INotificationRepository
{
    private readonly IMongoCollection<Notification> notificationCollection;

    public NotificationRepository(IMongoClient mongoClient, string databaseName, string collectionName)
    {
        var database = mongoClient.GetDatabase(databaseName);
        notificationCollection = database.GetCollection<Notification>(collectionName);
    }

    public async Task<List<Notification>> GetAllNotificationsAsync()
    {
        return await notificationCollection
            .Find(_ => true).ToListAsync();
    }

    public async Task<List<Notification>> GetNotificationsByUserGuidAsync(Guid userGuid)
    {
        return await notificationCollection
            .Find(n => n.UserGuid == userGuid).ToListAsync();
    }

    public async Task<List<Notification>> GetNotificationsByUserIdAsync(long userId)
    {
        return await notificationCollection
            .Find(n => n.UserId == userId).ToListAsync();
    }

    public async Task<Notification> GetNotificationByIdAsync(Guid id)
    {
        return await notificationCollection
            .Find(n => n.Id == id).FirstOrDefaultAsync();
    }

    public async Task<List<Notification>> GetNotificationsByStatusAsync(SubscriptionStatus subscriptionStatus, NotificationStatus status, DateTime currentDate)
    {
        return await notificationCollection
            .Find(n => n.SubscriptionStatus == subscriptionStatus && n.Status == status && n.SendDate <= currentDate).ToListAsync();
    }

    public async Task CreateNotificationAsync(Notification notification)
    {
        await notificationCollection
            .InsertOneAsync(notification);
    }

    public async Task UpdateNotificationAsync(Guid id, Notification updatedNotification)
    {
        var updateDefinition = Builders<Notification>.Update
            .Set(n => n.Theme, updatedNotification.Theme)
            .Set(n => n.Message, updatedNotification.Message)
            .Set(n => n.SendDate, updatedNotification.SendDate)
            .Set(n => n.Status, updatedNotification.Status)
            .Set(n => n.TypeChannel, updatedNotification.TypeChannel)
            .Set(n => n.SubscriptionStatus, updatedNotification.SubscriptionStatus);

        await notificationCollection.UpdateOneAsync(n => n.Id == id, updateDefinition);
    }



    public async Task DeleteNotificationAsync(Guid id)
    {
        await notificationCollection
            .DeleteOneAsync(n => n.Id == id);
    }
}
