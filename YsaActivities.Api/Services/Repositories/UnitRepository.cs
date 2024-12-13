using MongoDB.Driver;
using YsaActivities.Api.Models;

namespace YsaActivities.Api.Services.Repositories;

public class UnitRepository
{
    private readonly IMongoDatabase _database;
    private readonly ILogger<UnitRepository> _logger;
    private const string COLLECTION = "Units";

    public UnitRepository(
        MongoClient client,
        ILogger<UnitRepository> logger)
    {
        _database = client.GetDatabase("church-ysa");
        _logger = logger;
    }

    public async Task<Unit> FindById(UnitId id)
    {
        var collection = _database.GetCollection<Unit>(COLLECTION);
        var filter = Builders<Unit>.Filter.Eq(u => u.Id, id);
        var unit = await (await collection.FindAsync(filter)).SingleAsync();
        return unit;
    }

    public async Task<IEnumerable<Unit>> GetAllUnits()
    {
        var collection = _database.GetCollection<Unit>(COLLECTION);
        var filter = Builders<Unit>.Filter.Empty;
        var units = await (await collection.FindAsync(filter)).ToListAsync();
        return units;
    }

    public async Task<UnitId> CreateUnit(Unit unit)
    {
        var collection = _database.GetCollection<Unit>(COLLECTION);
        await collection.InsertOneAsync(unit);
        return unit.Id;
    }

    public async Task UpdateUnit(Unit unit)
    {
        var collection = _database.GetCollection<Unit>(COLLECTION);
        var filter = Builders<Unit>.Filter.Eq(u => u.Id, unit.Id);
        await collection.ReplaceOneAsync(filter, unit);
    }

    public async Task AddMemberToUnit(UnitId id, Member member)
    {
        var collection = _database.GetCollection<Unit>(COLLECTION);
        var filter = Builders<Unit>.Filter.Eq(u => u.Id, id);
        var update = Builders<Unit>.Update.Push(u => u.Members, member);
        await collection.UpdateOneAsync(filter, update);
    }

    public async Task RemoveMemberFromUnit(UnitId id, MemberId memberId)
    {
        var collection = _database.GetCollection<Unit>(COLLECTION);
        var filter = Builders<Unit>.Filter.Eq(u => u.Id, id);
        var update = Builders<Unit>.Update.PullFilter(u => u.Members, m => m.Id == memberId);
        await collection.UpdateOneAsync(filter, update);
    }

    public async Task DeleteUnit(Unit unit)
    {
        var collection = _database.GetCollection<Unit>(COLLECTION);
        var filter = Builders<Unit>.Filter.Eq(u => u.Id, unit.Id);
        await collection.DeleteOneAsync(filter);
    }

    public async Task DeleteUnitById(UnitId id)
    {
        var collection = _database.GetCollection<Unit>(COLLECTION);
        var filter = Builders<Unit>.Filter.Eq(u => u.Id, id);
        await collection.DeleteOneAsync(filter);
    }
}