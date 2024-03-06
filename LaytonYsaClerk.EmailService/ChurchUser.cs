using System.Text.Json.Serialization;

namespace LaytonYsaClerk.EmailService;

public class ChurchUser
{
    [JsonPropertyName("id")]
    public long MemberId { get; set; }
    
    [JsonPropertyName("textAddress")]
    public string Address { get; set; } = default!;

    [JsonPropertyName("addressUnknown")]
    public bool AddressUnknown { get; set; } = false;
    
    [JsonPropertyName("age")]
    public int Age { get; set; }

    [JsonPropertyName("birthdateCalc")]
    public DateOnly Birthday { get; set; }

    [JsonPropertyName("Gender")]
    public string Gender { get; set; } = default!;

    [JsonPropertyName("householdPosition")]
    public string HouseholdPosistion { get; set; } = default!;

    [JsonPropertyName("moveDateCalc")]
    public DateOnly MoveInDate { get; set; }

    [JsonPropertyName("phone")]
    public string Phone { get; set; } = default!;

    [JsonPropertyName("name")]
    public string FullName { get; set; } = default!;

    [JsonPropertyName("priorUnitName")]
    public string PriorUnit { get; set; } = default!;

    [JsonPropertyName("priorUnitNumber")]
    public string PriorUnitNumber { get; set; } = default!;

    [JsonPropertyName("unitDetails")]
    public UnitDetails UnitDetails { get; set; } = default!;
}