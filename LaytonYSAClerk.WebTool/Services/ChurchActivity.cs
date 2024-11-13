using MongoDB.Bson;

namespace LaytonYSAClerk.WebTool.Services;

public class ChurchActivity
{
    public ObjectId Id { get; set; }
    public string Name { get; set; }
    public List<long> JoinedMembers { get; set; }
    public List<MemberGroup> groups { get; set; } = [];
}