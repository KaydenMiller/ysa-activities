using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;

namespace YsaActivities.Api.Models;

public record struct UnitId(ObjectId Id);

/// <summary>
/// A unit is typically the same as a ward, but effectively it
/// just represents a group of individuals that commonly work
/// together for the purpose of the activities. This could be
/// a ward, stake, building, group of friends, etc
/// </summary>
public class Unit
{
    public UnitId Id { get; set; }
    
    /// <summary>
    /// The name of the group / unit to help with identifying it
    /// via searches.
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// These are the members of the unit that typically join
    /// the activities of this unit.
    /// </summary>
    public IEnumerable<Member> Members { get; set; }

    public static Unit Create(string name)
    {
        return new Unit
        {
            Id = new UnitId(ObjectId.GenerateNewId()),
            Name = name,
            Members = []
        };
    }
}