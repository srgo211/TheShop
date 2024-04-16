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

    public async Task<IEnumerable<Notification>> FetchData()
    {
        logger.LogInformation("Получеем уведомления из MongoDB.");

        FilterDefinition<Notification>? filter = Builders<Notification>.Filter.Lte(n => n.SendDate, DateTime.UtcNow) &
                                                 Builders<Notification>.Filter.Eq(n => n.Status, NotificationStatus.Wait);
        
        List<Notification>? notifications = await collection.Find(filter).ToListAsync();

        logger.LogInformation($"Получено {notifications.Count} уведомлений.");
        return notifications;
    }
}