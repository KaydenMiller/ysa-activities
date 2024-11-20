using Church.Ysa.Domain;
using MongoDB.Bson;

namespace LaytonYSAClerk.WebTool.Services;

public class MemberGroupFactory
{
    private readonly IEnumerable<ChurchMember> _maleMembers;
    private readonly IEnumerable<ChurchMember> _femaleMembers;
    private const int IDEAL_GROUP_SIZE = 2;

    public MemberGroupFactory(ICollection<ChurchMember> members)
    {
        _maleMembers = members.Where(m => m.Gender is Gender.Male);
        _femaleMembers = members.Where(m => m.Gender is Gender.Female);
    }

    public ICollection<MemberGroup> CreateGroups()
    {
        var shuffledFemales = ShuffleMembers(_femaleMembers.ToList());
        var groupedFemales = GenerateGroups(shuffledFemales);
        var shuffledMales = ShuffleMembers(_maleMembers.ToList());
        var groupedMales = GenerateGroups(shuffledMales);

        return
        [
            ..groupedFemales,
            ..groupedMales
        ];
    }

    private IEnumerable<MemberGroup> GenerateGroups(ICollection<ChurchMember> members)
    {
        var remainingMembers = members.Count % IDEAL_GROUP_SIZE;
        var lastGroupedMemberIndex = (members.Count - 1) - remainingMembers;
        var nonGroupedMembers = members.ToList()[lastGroupedMemberIndex..];
        var groups = members
           .ToList()[..^remainingMembers]
           .Chunk(IDEAL_GROUP_SIZE)
           .Select(m => new MemberGroup(ObjectId.GenerateNewId(), m, [], ""))
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

        return groups;
    }

    private ICollection<ChurchMember> ShuffleMembers(IList<ChurchMember> members)
    {
        return members.Shuffle();
    }
}