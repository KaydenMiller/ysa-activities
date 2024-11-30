using Church.Ysa.Domain;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MudBlazor.Extensions;

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

    public async Task<List<ChurchActivity>> GetActivities()
    {
        var activities = await _database
           .GetCollection<ChurchActivity>(COLLECTION)
           .AsQueryable()
           .ToListAsync();
        return activities;
    }

    public async Task<bool> AddMemberToActivity(SimpleMember member, ObjectId activityId)
    {
        _logger.LogInformation("Add Member {MemberName} to Activity {ActivityId}", member.Name, activityId);
        var filter = Builders<ChurchActivity>.Filter.Eq("Id", activityId);
        var update = Builders<ChurchActivity>.Update.Push(ca => ca.JoinedMembers, member);
        await _database.GetCollection<ChurchActivity>(COLLECTION)
           .UpdateOneAsync(filter, update);
        return true;
    }
    
    public async Task<bool> RemoveMemberFromActivity(SimpleMember member, ObjectId activityId)
    {
        _logger.LogInformation("Remove Member {MemberName} to Activity {ActivityId}", member.Name, activityId);
        var filter = Builders<ChurchActivity>.Filter.Eq("Id", activityId);
        var update = Builders<ChurchActivity>.Update.Pull(ca => ca.JoinedMembers, member);
        await _database.GetCollection<ChurchActivity>(COLLECTION)
           .UpdateOneAsync(filter, update);
        return true;
    }
    
    public async Task<bool> AddFellowshipMemberToActivity(SimpleMember member, ObjectId activityId)
    {
        _logger.LogInformation("Add Member {MemberName} to Activity {ActivityId}", member.Name, activityId);
        var filter = Builders<ChurchActivity>.Filter.Eq("Id", activityId);
        var update = Builders<ChurchActivity>.Update.Push(ca => ca.MembersToFellowship, member);
        await _database.GetCollection<ChurchActivity>(COLLECTION)
           .UpdateOneAsync(filter, update);
        return true;
    }
    
    public async Task<bool> RemoveFellowshipMemberFromActivity(SimpleMember member, ObjectId activityId)
    {
        _logger.LogInformation("Remove Member {MemberName} to Activity {ActivityId}", member.Name, activityId);
        var filter = Builders<ChurchActivity>.Filter.Eq("Id", activityId);
        var update = Builders<ChurchActivity>.Update.Pull(ca => ca.MembersToFellowship, member);
        await _database.GetCollection<ChurchActivity>(COLLECTION)
           .UpdateOneAsync(filter, update);
        return true;
    }
    
    public async Task<IEnumerable<SimpleMember>> GetActivityMembers(ObjectId activityId)
    {
        var activity = await _database.GetCollection<ChurchActivity>(COLLECTION)
               .AsQueryable()
               .SingleOrDefaultAsync(a => a.Id == activityId);

        return activity.JoinedMembers;
    }

    public async Task<MemberGroup?> GetMemberGroup(ObjectId activityId, ObjectId groupId)
    {
        var activity = await _database.GetCollection<ChurchActivity>(COLLECTION)
           .AsQueryable()
           .SingleOrDefaultAsync(a => a.Id == activityId);
        var group = activity.groups.SingleOrDefault(g => g.GroupId == groupId);
        return group;
    }

    public async Task<string> GetGroupNotes(ObjectId activityId, ObjectId groupId)
    {
        
        var activity = await _database.GetCollection<ChurchActivity>(COLLECTION)
           .AsQueryable()
           .SingleOrDefaultAsync(a => a.Id == activityId);
        var group = activity.groups.SingleOrDefault(g => g.GroupId == groupId);
        return group?.Notes ?? "";
    }

    public async Task UpsertMemberGroup(ObjectId activityId, MemberGroup memberGroup)
    {
        _logger.LogInformation("Upsert Member Group in mongo");
        var activityFilter = Builders<ChurchActivity>.Filter.Eq("_id", activityId);
        var activity = await _database.GetCollection<ChurchActivity>(COLLECTION)
           .AsQueryable()
           .SingleAsync(a => a.Id == activityId);

        var groupToUpdate = activity.groups.SingleOrDefault(g => g.GroupId == memberGroup.GroupId);

        if (groupToUpdate is not null)
        {
            activity.groups.Remove(groupToUpdate);
        }
        activity.groups.Add(memberGroup);

        var update = Builders<ChurchActivity>.Update.Set("groups", activity.groups);

        await _database.GetCollection<ChurchActivity>(COLLECTION)
           .UpdateOneAsync(activityFilter, update);
    }

    public async Task DeleteActivity(ObjectId activityId)
    {
        _logger.LogInformation("Deleting activity with id {ActivityId}", activityId);
        await _database.GetCollection<ChurchActivity>(COLLECTION)
           .DeleteOneAsync(Builders<ChurchActivity>.Filter.Eq("_id", activityId));
    }

    public async Task ClearGroups(ObjectId activityId)
    {
        _logger.LogWarning("Someone cleared the groups");
        await _database.GetCollection<ChurchActivity>(COLLECTION)
           .UpdateOneAsync(
                Builders<ChurchActivity>.Filter.Eq("_id", activityId),
                Builders<ChurchActivity>.Update.Set(x => x.groups, []));
    }
}