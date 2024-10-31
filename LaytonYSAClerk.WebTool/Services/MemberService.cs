using Church.Ysa.Domain;
using FuzzyString;
using MongoDB.Bson;

namespace LaytonYSAClerk.WebTool.Services;

public class MemberService
{
    private readonly MemberRepository _memberRepository;
    private readonly ActivityRepository _activityRepository;

    public MemberService(
        MemberRepository memberRepository,
        ActivityRepository activityRepository)
    {
        _memberRepository = memberRepository;
        _activityRepository = activityRepository;
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
        await _activityRepository.RegisterActivity(name);
        return true;
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
        var activities = await _activityRepository.GetActivities();
        return activities.Single(m => m.Name == name);
    }
    
    public async Task<bool> JoinActivity(ObjectId activityId, long memberId)
    {
        await _activityRepository.AddMemberToActivity(memberId, activityId);
        return true;
    }
}