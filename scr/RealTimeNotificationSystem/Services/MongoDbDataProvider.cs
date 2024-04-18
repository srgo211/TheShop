using MongoDB.Driver;
using RealTimeNotificationSystem.Interfaces;
using SharedDomainModels;
using SharedInterfaces;

namespace RealTimeNotificationSystem.Services;

public class MongoDbDataProvider : IDataProvider<Notification>
{
    private readonly IMongoCollection<Notification> collection;
    private readonly ILogger<MongoDbDataProvider> logger;

    public MongoDbDataProvider(IConfiguration configuration, ILogger<MongoDbDataProvider> logger)
    {
        logger.LogInformation("MongoDB settings.");
        var connectionString = configuration["MongoDbSettings:ConnectionString"];

        MongoClient client = new MongoClient(connectionString);
        IMongoDatabase? database = client.GetDatabase(configuration["MongoDbSettings:DatabaseName"]);
        this.collection = database.GetCollection<Notification>(configuration["MongoDbSettings:CollectionName"]);
        this.logger = logger;
    }

    public async Task<IEnumerable<Notification>> FetchDataAsync()
    {
        logger.LogInformation("Получеем уведомления из MongoDB.");

        FilterDefinition<Notification>? filter = Builders<Notification>.Filter.Lte(n => n.SendDate, DateTime.UtcNow) &
                                                 Builders<Notification>.Filter.Eq(n => n.Status, NotificationStatus.Wait);
        
        List<Notification>? notifications = await collection.Find(filter).ToListAsync();

        logger.LogInformation($"Получено {notifications.Count} уведомлений.");
        return notifications;
    }

    public async Task<bool>  UpdateDataAsync(Notification updatedNotification)
    {
        try
        {
            var updateDefinition = Builders<Notification>.Update
                .Set(n => n.Theme, updatedNotification.Theme)
                .Set(n => n.Message, updatedNotification.Message)
                .Set(n => n.SendDate, updatedNotification.SendDate)
                .Set(n => n.Status, updatedNotification.Status)
                .Set(n => n.TypeChannel, updatedNotification.TypeChannel)
                .Set(n => n.SubscriptionStatus, updatedNotification.SubscriptionStatus);

            await collection.UpdateOneAsync(n => n.Id == updatedNotification.Id, updateDefinition);
            return true;
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            return false;
        }
     
        
    }
}