using MongoDB.Bson;

namespace YsaActivities.Api.Models;

public record struct ActivityId(ObjectId Id);

/// <summary>
/// When a <see cref="Unit">Unit</see> gets togeather they join
/// an activity which will describe what they are doing and whom
/// they are visiting. This acts as the core what this application
/// should do and enable.
/// </summary>
public class Activity
{
    public ActivityId Id { get; set; }
    
    /// <summary>
    /// This is the Unit to which the Activity belongs
    /// </summary>
    public UnitId AssociatedUnitId { get; set; }
    
    /// <summary>
    /// This is the human friendly name of the activity
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// What will you be doing for the duration of the activity
    /// </summary>
    public string Description { get; set; } = "";

    /// <summary>
    /// When do you expect to start the activity
    /// </summary>
    public DateTime? StartTime { get; set; } = null;

    /// <summary>
    /// When do you expect the activity to end by
    /// </summary>
    public DateTime? EndTime { get; set; } = null;

    /// <summary>
    /// The members of the activity which are going to visit the
    /// members that are requested to be visited.
    /// </summary>
    public IEnumerable<Member> MembersAtActivity { get; set; } = [];

    /// <summary>
    /// The members which are planned or desired to be visited during
    /// the course of the activity.
    /// </summary>
    public IEnumerable<Member> MembersToVisit { get; set; } = [];
}