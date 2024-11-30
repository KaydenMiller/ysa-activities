using Church.Ysa.Domain;
using MongoDB.Bson;

namespace LaytonYSAClerk.WebTool.Services;

public record MemberGroup(ObjectId GroupId, IEnumerable<SimpleMember> GroupMembers, IEnumerable<SimpleMember>? ObjectiveMembers, string Notes);