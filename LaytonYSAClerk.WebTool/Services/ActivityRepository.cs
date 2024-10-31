using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace LaytonYSAClerk.WebTool.Services;

public class ActivityRepository
{
    private readonly IMongoDatabase _database;
    private readonly ILogger<ActivityRepository> _logger;
    private const string COLLECTION = "activities";

    public ActivityRepository(MongoClient client, ILogger<ActivityRepository> logger)
    {
        _database = client.GetDatabase("church-ysa");
        _logger = logger;
    }

    public async Task<bool> RegisterActivity(string activityName)
    {
        var activity = _database.GetCollection<ChurchActivity>(COLLECTION);
        await activity.InsertOneAsync(new ChurchActivity()
        {
            Name = activityName,
            JoinedMembers = []
        });
        return true;
    }

    public Task<List<ChurchActivity>> GetActivities()
    {
        var activities = _database
           .GetCollection<ChurchActivity>(COLLECTION)
           .AsQueryable()
           .ToListAsync();
        return activities;
    }

    public async Task<bool> AddMemberToActivity(long memberId, ObjectId activityId)
    {
        var filter = Builders<ChurchActivity>.Filter.Eq("Id", activityId);
        var update = Builders<ChurchActivity>.Update.Push(ca => ca.JoinedMembers, memberId);
        await _database.GetCollection<ChurchActivity>(COLLECTION)
           .UpdateOneAsync(filter, update);
        return true;
    }
}

public class ChurchActivity
{
    public ObjectId Id { get; set; }
    public string Name { get; set; }
    public List<long> JoinedMembers { get; set; }
}