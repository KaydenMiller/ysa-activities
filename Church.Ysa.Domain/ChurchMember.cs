using System.Text.Json.Serialization;
using MongoDB.Bson;

namespace Church.Ysa.Domain;

public class SimpleMember
{
    public string Name { get; set; }
    public string Address { get; set; }
    public Gender Gender { get; set; }
}

public class ChurchMember
{
    [JsonIgnore]
    public ObjectId Id { get; set; }
    [JsonPropertyName("id")]
    public long ChruchMemberId { get; set; }
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonIgnore]
    public string FullName => string.Join(' ', Name.Split(',').Reverse());
    
    [JsonPropertyName("spokenName")]
    public string SpokenName { get; set; }
    [JsonPropertyName("nameOrder")]
    public int NameOrder { get; set; }
    [JsonPropertyName("birthDateFormated")]
    [JsonConverter(typeof(FormattedDateOnlyJsonConverter))]
    public DateOnly BirthDate { get; set; }
    [JsonPropertyName("genderCode")]
    public Gender Gender { get; set; }
    [JsonPropertyName("mrn")]
    public string MembershipRecordNumber { get; set; }
    [JsonPropertyName("email")]
    public string? Email { get; set; }
    [JsonPropertyName("phone")]
    public string? Phone { get; set; }
    [JsonPropertyName("unitNumber")]
    public int UnitNumber { get; set; }
    [JsonPropertyName("unitName")]
    public string UnitName { get; set; } = "";
    [JsonPropertyName("priesthoodCode")]
    public Priesthood? Priesthood { get; set; }
    [JsonPropertyName("nonMember")]
    public bool IsNonMember { get; set; }
    [JsonPropertyName("formattedConfirmationDate")]
    [JsonConverter(typeof(FormattedDateOnlyJsonConverter))]
    public DateOnly ConfirmationDate { get; set; }
    [JsonPropertyName("setApart")]
    public bool IsSetApart { get; set; }
    [JsonPropertyName("sustainedDate")]
    [JsonConverter(typeof(FormattedDateOnlyJsonConverter))]
    public DateOnly? SustainedDate { get; set; } = null;
    [JsonPropertyName("accountable")]
    public bool Endowed { get; set; }
    [JsonPropertyName("image")]
    public MemberImage? Image { get; set; }
}