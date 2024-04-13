﻿using MongoDB.Driver;
using NotificationServiceAPI.DTO;
using NotificationServiceAPI.Interfaces;

namespace NotificationServiceAPI.Repositorys;

public class NotificationRepository : INotificationRepository
{
    private readonly IMongoCollection<Notification> _notificationCollection;

    public NotificationRepository(IMongoClient mongoClient, string databaseName, string collectionName)
    {
        var database = mongoClient.GetDatabase(databaseName);
        _notificationCollection = database.GetCollection<Notification>(collectionName);
    }

    public async Task<List<Notification>> GetAllNotificationsAsync()
    {
        return await _notificationCollection.Find(_ => true).ToListAsync();
    }

    public async Task<List<Notification>> GetNotificationsByUserGuidAsync(Guid userGuid)
    {
        return await _notificationCollection.Find(n => n.UserGuid == userGuid).ToListAsync();
    }

    public async Task<List<Notification>> GetNotificationsByUserIdAsync(long userId)
    {
        return await _notificationCollection.Find(n => n.UserId == userId).ToListAsync();
    }

    public async Task<Notification> GetNotificationByIdAsync(Guid id)
    {
        return await _notificationCollection.Find(n => n.Id == id).FirstOrDefaultAsync();
    }

    public async Task<List<Notification>> GetNotificationsByStatusAsync(SubscriptionStatus subscriptionStatus, NotificationStatus status, DateTime currentDate)
    {
        return await _notificationCollection.Find(n => n.SubscriptionStatus == subscriptionStatus && n.Status == status && n.SendDate <= currentDate).ToListAsync();
    }

    public async Task CreateNotificationAsync(Notification notification)
    {
        await _notificationCollection.InsertOneAsync(notification);
    }

    public async Task UpdateNotificationAsync(Guid id, Notification updatedNotification)
    {
        await _notificationCollection.ReplaceOneAsync(n => n.Id == id, updatedNotification);
    }

    public async Task DeleteNotificationAsync(Guid id)
    {
        await _notificationCollection.DeleteOneAsync(n => n.Id == id);
    }
}
