using System.Text.Json.Serialization;

namespace Church.Ysa.Domain;

public class MemberImage
{
    [JsonPropertyName("individualId")]
    public long MemberId { get; set; }
    [JsonPropertyName("unitId")]
    public int UnitId { get; set; }
    [JsonPropertyName("format")]
    public string Format { get; set; }
    [JsonPropertyName("tokenUrl")]
    public string TokenUrl { get; set; }
}