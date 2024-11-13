using Church.Ysa.Domain;
using FuzzyString;
using MongoDB.Bson;

namespace LaytonYSAClerk.WebTool.Services;

public class MemberService
{
    private readonly MemberRepository _memberRepository;
    private readonly ActivityRepository _activityRepository;
    private readonly ILogger<MemberService> _logger;

    public MemberService(
        MemberRepository memberRepository,
        ActivityRepository activityRepository,
        ILogger<MemberService> logger)
    {
        _memberRepository = memberRepository;
        _activityRepository = activityRepository;
        _logger = logger;
    }
    
    public async Task<IEnumerable<ChurchMember>> GetMembers()
    {
        var churchMember = await _memberRepository.GetMembers();
        return churchMember;
    }

    public async Task<IEnumerable<ChurchMember>> SearchMembersFuzzy(string searchParam)
    {
        var members = await _memberRepository.GetMembers();

        List<FuzzyStringComparisonOptions> fuzzyOptions =
        [
            FuzzyStringComparisonOptions.UseOverlapCoefficient,
            FuzzyStringComparisonOptions.UseLongestCommonSubsequence,
            FuzzyStringComparisonOptions.UseLongestCommonSubstring
        ];

        return members.Where(m =>
            m.SpokenName.ApproximatelyEquals(searchParam, fuzzyOptions, FuzzyStringComparisonTolerance.Strong));
    }

    public async Task<bool> CreateActivity(string name)
    {
        _logger.LogInformation("Registering an Activity with name {Name}", name);
        await _activityRepository.RegisterActivity(name);
        return true;
    }

    public async Task<IEnumerable<ChurchActivity>> GetActivities()
    {
        var activities = await _activityRepository.GetActivities();
        return activities;
    }

    public async Task<IEnumerable<ChurchActivity>> SearchActivitiesFuzzy(string searchParam)
    {
        var activities = await _activityRepository.GetActivities();
        List<FuzzyStringComparisonOptions> fuzzyOptions =
        [
            FuzzyStringComparisonOptions.UseOverlapCoefficient,
            FuzzyStringComparisonOptions.UseLongestCommonSubsequence,
            FuzzyStringComparisonOptions.UseLongestCommonSubstring
        ];

        return activities.Where(m =>
            m.Name.ApproximatelyEquals(searchParam, fuzzyOptions, FuzzyStringComparisonTolerance.Strong));
    }

    
    
    public async Task<ChurchActivity> FindActivityByName(string name)
    {
        _logger.LogInformation("Searching for Activity with name {ActivityName}", name);
        var activities = await _activityRepository.GetActivities();
        return activities.Single(m => m.Name == name);
    }

    public async Task<bool> JoinActivity(ObjectId activityId, long memberId)
    {
        await _activityRepository.AddMemberToActivity(memberId, activityId);
        return true;
    }

    private const int GROUP_SIZE = 3;
    public async Task<bool> GeneratePartners(string activityName)
    {
        var activity = await FindActivityByName(activityName);
        _logger.LogInformation("Generating partners for Activity {ActivityId}", activity.Id.ToString());

        if (activity.groups.Count > 0)
        {
            _logger.LogInformation("Activity Already generated Partners use those");
            return true;
        }

        _logger.LogInformation("Generating new partners");

        var memberIds = activity.JoinedMembers;
        var members = (await _memberRepository.GetMembersByIds(memberIds)).ToList();
        var membersToSeek = (await _memberRepository.GetMembers()).Except(members).ToList();
        var randomizedMembers = members.ToList().Shuffle();
        var groups = randomizedMembers.Chunk(GROUP_SIZE).ToList()
           .Select(m => new MemberGroup(ObjectId.GenerateNewId(), m, [], ""))
           .ToList();
        var buckets = membersToSeek.Count / groups.Count;
        var shuffledMembersToSeek = membersToSeek.ToList();
        var membersToSeekBucket = shuffledMembersToSeek.Chunk(buckets).ToArray();

        for (var groupIdx = 0; groupIdx < groups.Count; groupIdx++)
        {
            var group = groups[groupIdx] with
            {
                ObjectiveMembers = membersToSeekBucket[groupIdx]
            };
            await _activityRepository.UpsertMemberGroup(activity.Id, group);
        }

        return true;
    }

    public async Task<IEnumerable<MemberGroup>> GetPartners(string activityName)
    {
        var activity = await FindActivityByName(activityName);
        if (activity.groups.Count == 0)
        {
            _logger.LogError("No groups created for {ActivityId} yet", activity.Id.ToString());
            throw new Exception();
        }
        _logger.LogInformation("Found activity {ActivityId} for {ActivityName}", activity.Id.ToString(), activityName);
        return activity.groups;
    }

    public async Task<IEnumerable<ChurchMember>> GetMembersToSeek(string activityName)
    {
        var activity = await FindActivityByName(activityName);
        if (activity.groups.Count == 0)
        {
            _logger.LogError("No groups created for {ActivityId} yet", activity.Id.ToString());
            throw new Exception();
        }

        var availableMembers = (await _memberRepository.GetMembers())
           .Where(m => !activity.JoinedMembers.Contains(m.ChruchMemberId));

        return availableMembers;
    }

    // private async Task<IEnumerable<NamedMemberGroup>> ConvertMemberGroups(IEnumerable<MemberGroup> groups)
    // {
    //     List<NamedMemberGroup> namedMemberGroups = [];
    //     foreach (var group in groups)
    //     {
    //         var groupMembers = await ConvertToMember(group.GroupMembers);
    //         var objectiveMembers = await ConvertToMember(group.ObjectiveMembers);
    //         namedMemberGroups.Add(new NamedMemberGroup(groupMembers, objectiveMembers));
    //     }
    //     return namedMemberGroups;
    // }

    // private async Task<IEnumerable<ChurchMember>> ConvertToMember(IEnumerable<long> memberIds)
    // {
    //     var members = await _memberRepository.GetMembers();
    //     var groupMembers = members.Where(m => memberIds.Contains(m.ChruchMemberId));
    //     return groupMembers;
    // }
}