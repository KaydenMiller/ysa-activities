﻿using MongoDB.Bson;
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

    public async Task<List<ChurchActivity>> GetActivities()
    {
        var activities = await _database
           .GetCollection<ChurchActivity>(COLLECTION)
           .AsQueryable()
           .ToListAsync();
        return activities;
    }

    public async Task<bool> AddMemberToActivity(long memberId, ObjectId activityId)
    {
        _logger.LogInformation("Add Member {MemberId} to Activity {ActivityId}", memberId, activityId);
        var filter = Builders<ChurchActivity>.Filter.Eq("Id", activityId);
        var update = Builders<ChurchActivity>.Update.Push(ca => ca.JoinedMembers, memberId);
        await _database.GetCollection<ChurchActivity>(COLLECTION)
           .UpdateOneAsync(filter, update);
        return true;
    }
    
    public async Task<bool> RemoveMemberFromActivity(long memberId, ObjectId activityId)
    {
        _logger.LogInformation("Remove Member {MemberId} to Activity {ActivityId}", memberId, activityId);
        var filter = Builders<ChurchActivity>.Filter.Eq("Id", activityId);
        var update = Builders<ChurchActivity>.Update.Pull(ca => ca.JoinedMembers, memberId);
        await _database.GetCollection<ChurchActivity>(COLLECTION)
           .UpdateOneAsync(filter, update);
        return true;
    }
    
    public async Task<IEnumerable<long>> GetActivityMembers(ObjectId activityId)
    {
        var activity = await _database.GetCollection<ChurchActivity>(COLLECTION)
               .AsQueryable()
               .SingleOrDefaultAsync(a => a.Id == activityId);

        return activity.JoinedMembers;
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
}