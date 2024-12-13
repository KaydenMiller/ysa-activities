using MongoDB.Driver;
using YsaActivities.Api.Models;

namespace YsaActivities.Api.Services.Repositories;

public class ActivityRepository
{
    private readonly IMongoDatabase _database;
    private readonly ILogger<ActivityRepository> _logger;
    private const string COLLECTION = "Activities";

    public ActivityRepository(
        MongoClient client,
        ILogger<ActivityRepository> logger)
    {
        _database = client.GetDatabase("church-ysa");
        _logger = logger;
    }

    public async Task<Activity> GetActivityById(ActivityId id)
    {
        var collection = _database.GetCollection<Activity>(COLLECTION);
        var filter = Builders<Activity>.Filter.Eq(x => x.Id, id);
        var activity = await (await collection.FindAsync(filter)).SingleAsync();
        return activity;
    }

    public async Task<IEnumerable<Activity>> GetAllActivities()
    {
        var collection = _database.GetCollection<Activity>(COLLECTION);
        var filter = Builders<Activity>.Filter.Empty;
        var activities = await (await collection.FindAsync(filter)).ToListAsync();
        return activities;
    }

    public async Task<ActivityId> CreateActivity(Activity activity)
    {
        var collection = _database.GetCollection<Activity>(COLLECTION);
        await collection.InsertOneAsync(activity);
        return activity.Id;
    }

    public async Task UpdateActivity(Activity activity)
    {
        var collection = _database.GetCollection<Activity>(COLLECTION);
        var filter = Builders<Activity>.Filter.Eq(x => x.Id, activity.Id);
        await collection.ReplaceOneAsync(filter, activity);
    }

    public async Task DeleteActivityById(ActivityId id)
    {
        var collection = _database.GetCollection<Activity>(COLLECTION);
        var filter = Builders<Activity>.Filter.Eq(x => x.Id, id);
        await collection.DeleteOneAsync(filter);
    }
}