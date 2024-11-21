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

        var groupFactory = new MemberGroupFactory(members, membersToSeek);
        var groups = groupFactory.CreateGroups().ToList();
        
        foreach (var group in groups) {
            await _activityRepository.UpsertMemberGroup(activity.Id, group);
        }

        return true;
    }

    public async Task DeleteActivity(string activityName)
    {
        var activity = await FindActivityByName(activityName);
        _logger.LogInformation("Deleting activity by the name {Name}", activityName);
        await _activityRepository.DeleteActivity(activity.Id);
    }
}