using Church.Ysa.Domain;
using MongoDB.Bson;

namespace LaytonYSAClerk.WebTool.Services;

public class ChurchActivity
{
    public ObjectId Id { get; set; }
    public string Name { get; set; }
    public List<SimpleMember> JoinedMembers { get; set; } = [];
    public List<SimpleMember> MembersToFellowship { get; set; } = [];
    public List<MemberGroup> groups { get; set; } = [];
}