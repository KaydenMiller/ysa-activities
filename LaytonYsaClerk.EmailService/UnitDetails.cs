using System.Text.Json.Serialization;

namespace LaytonYsaClerk.EmailService;

public class UnitDetails
{
    [JsonPropertyName("unitNumber")]
    public int UnitNumber { get; set; }
    
    [JsonPropertyName("title")]
    public string UnitTitle { get; set; } = default!;
    
    [JsonPropertyName("leaderName")]
    public string LeaderName { get; set; } = default!;
    
    [JsonPropertyName("leaderCellPhone")]
    public string LeaderCellPhone { get; set; } = default!;

    [JsonPropertyName("leaderEmail")]
    public string LeaderEmail { get; set; } = default!;

    [JsonPropertyName("positionName")]
    public string PositionName { get; set; } = default!;
}