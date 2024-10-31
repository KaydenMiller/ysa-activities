using Church.Ysa.Domain;
using MongoDB.Bson;
using MongoDB.Driver;

namespace LaytonYSAClerk.WebTool.Services;

public class MemberRepository
{
    private readonly IMongoDatabase _database;
    private readonly ILogger<MemberRepository> _logger;
    private const string COLLECTION = "members";

    public MemberRepository(
        MongoClient client,
        ILogger<MemberRepository> logger)
    {
        _database = client.GetDatabase("church-ysa");
        _logger = logger;
    }
    
    public async Task AddMembers(IEnumerable<ChurchMember> members)
    {
        var enumeratedMembers = members.ToList();
        _logger.LogInformation("Add {UserCount} users to {Collection}", enumeratedMembers.Count, COLLECTION);
        var membersCollection = _database 
           .GetCollection<ChurchMember>(COLLECTION);
        await membersCollection.InsertManyAsync(enumeratedMembers);
    }
    
    public async Task<IEnumerable<ChurchMember>> GetMembers()
    {
        var filter = Builders<ChurchMember>.Filter.Empty;
        var members = await _database
           .GetCollection<ChurchMember>(COLLECTION)
           .Find(filter)
           .ToListAsync();
        return members;
    }
}