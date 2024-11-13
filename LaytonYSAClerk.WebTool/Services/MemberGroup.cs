using Church.Ysa.Domain;
using MongoDB.Bson;

namespace LaytonYSAClerk.WebTool.Services;

public record MemberGroup(ObjectId GroupId, IEnumerable<ChurchMember> GroupMembers, IEnumerable<ChurchMember>? ObjectiveMembers, string Notes);