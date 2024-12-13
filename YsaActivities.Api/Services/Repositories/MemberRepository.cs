using MongoDB.Driver;
using YsaActivities.Api.Models;

namespace YsaActivities.Api.Services.Repositories;

public class MemberRepository
{
    private readonly IMongoDatabase _database;
    private readonly ILogger<MemberRepository> _logger;
    private const string COLLECTION = "Members";
    
    public MemberRepository(
        MongoClient client,
        ILogger<MemberRepository> logger)
    {
        _database = client.GetDatabase("church-ysa");
        _logger = logger;
    }

    public async Task<Member> GetMemberById(MemberId memberId)
    {
        var collection = _database.GetCollection<Member>(COLLECTION);
        var filter = Builders<Member>.Filter.Eq(member => member.Id, memberId);
        var member = await collection.FindAsync(filter);
        return await member.SingleAsync();
    }
    
    public async Task UpdateMember(Member member)
    {
        var collection = _database.GetCollection<Member>(COLLECTION);
        var filter = Builders<Member>.Filter.Eq(member => member.Id, member.Id);
        await collection.ReplaceOneAsync(filter, member);
    }
}