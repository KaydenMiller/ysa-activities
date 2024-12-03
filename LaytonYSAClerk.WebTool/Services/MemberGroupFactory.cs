using Church.Ysa.Domain;
using ListShuffle;
using MongoDB.Bson;

namespace LaytonYSAClerk.WebTool.Services;

public class MemberGroupFactory
{
    private readonly IEnumerable<SimpleMember> _maleMembers;
    private readonly IEnumerable<SimpleMember> _femaleMembers;
    private readonly IEnumerable<SimpleMember> _maleFellowshipMembers;
    private readonly IEnumerable<SimpleMember> _femaleFellowshipMembers;
    private const int IDEAL_GROUP_SIZE = 2;

    public MemberGroupFactory(ICollection<SimpleMember> members, ICollection<SimpleMember> membersToFellowship)
    {
        if (members is null || members.Count == 0)
            throw new Exception("Members cannot be null or 0");
        if (membersToFellowship is null || membersToFellowship.Count == 0)
            throw new Exception("Members to fellowship cannot be null or 0");
        
        _maleMembers = members.Where(m => m.Gender is Gender.Male);
        _femaleMembers = members.Where(m => m.Gender is Gender.Female);

        _femaleFellowshipMembers = membersToFellowship.Where(m => m.Gender is Gender.Female);
        _maleFellowshipMembers = membersToFellowship.Where(m => m.Gender is Gender.Male);
    }

    public ICollection<MemberGroup> CreateGroups()
    {
        var shuffledFemales = ShuffleMembers(_femaleMembers.ToList());
        var groupedFemales = GenerateGroups(shuffledFemales, ShuffleMembers(_femaleFellowshipMembers.ToList()));
        var shuffledMales = ShuffleMembers(_maleMembers.ToList());
        var groupedMales = GenerateGroups(shuffledMales, ShuffleMembers(_maleFellowshipMembers.ToList()));
            
        return
        [
            ..groupedFemales,
            ..groupedMales
        ];
    }

    private IEnumerable<MemberGroup> GenerateGroups(ICollection<SimpleMember> members, ICollection<SimpleMember> membersToFellowship)
    {
        if (members.Count == 0)
            return [];
        
        var remainingMembers = members.Count % IDEAL_GROUP_SIZE;
        var lastGroupedMemberIndex = members.Count - remainingMembers;
        var nonGroupedMembers = members.ToList()[lastGroupedMemberIndex..];
        var groups = members
           .ToList()[..^remainingMembers]
           .Chunk(IDEAL_GROUP_SIZE)
           .Select(m => new MemberGroup(ObjectId.GenerateNewId(), m, [], []))
           .ToList();

        if (remainingMembers > 0)
        {
            var updatedMembers = groups[^1].GroupMembers.ToList();
            updatedMembers.AddRange(nonGroupedMembers);
            // we have people remaining add them to the last group
            var updatedGroup = groups[^1] with
            {
                GroupMembers = updatedMembers
            };
            groups[^1] = updatedGroup;
        }

        var fellowshipMembersRemaining = membersToFellowship.Count % groups.Count;
        var lastFellowshipMemberIndex = membersToFellowship.Count - fellowshipMembersRemaining;
        var fellowshipMembersNotInAGroup = membersToFellowship.ToList()[lastFellowshipMemberIndex..];
        var fellowshipGroupSize = membersToFellowship.Count / groups.Count;
        var fellowshipGroups = membersToFellowship.Chunk(fellowshipGroupSize).ToList();

        for (var index = 0; index < groups.Count; index++)
        {
            groups[index] = groups[index] with
            {
                ObjectiveMembers = fellowshipGroups[index]
            };
        }

        var totalGroups = groups.Count;
        var groupIndex = 0;
        foreach (var memberNotInGroup in fellowshipMembersNotInAGroup)
        {
            var updatedMembersToFellowship = groups[groupIndex].ObjectiveMembers?.ToList() ?? [];
            updatedMembersToFellowship.Add(memberNotInGroup);
            updatedMembersToFellowship = updatedMembersToFellowship.Distinct().ToList();
            groups[groupIndex] = groups[groupIndex] with
            {
                ObjectiveMembers = updatedMembersToFellowship
            };
            groupIndex = (groupIndex + 1) % totalGroups;
        }

        return groups;
    }

    private ICollection<SimpleMember> ShuffleMembers(IList<SimpleMember> members)
    {
        var membersToShuffle = members.ToList();
        membersToShuffle.CryptoStrongShuffle();
        return membersToShuffle;
    }
}