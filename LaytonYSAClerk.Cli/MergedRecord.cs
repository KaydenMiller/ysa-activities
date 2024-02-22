using System.Text.Json.Serialization;

namespace LaytonYSAClerk.Cli;

public class MergedRecord
{
    [JsonPropertyName("fullname")]
    public string Fullname { get; set; }
    
    [JsonPropertyName("pulled")]
    public bool? Pulled { get; set; }
    
    [JsonPropertyName("pulledTimestamp")]
    public DateTime? PulledTimestamp { get; set; }
    
    [JsonPropertyName("birthday")]
    public DateTime? Birthday { get; set; }
    
    [JsonPropertyName("email")]
    public string? Email { get; set; }
    
    [JsonPropertyName("phone")]
    public string? Phone { get; set; }
    
    [JsonPropertyName("address")]
    public string? Address { get; set; }
    
    [JsonPropertyName("apartmentNumber")]
    public string? ApartmentNumber { get; set; }
    
    [JsonPropertyName("city")]
    public string? City { get; set; }
    
    [JsonPropertyName("zip")]
    public string? Zip { get; set; }

    [JsonPropertyName("photoDriveUrl")]
    public string? PhotoDriveUrl { get; set; }
    
    [JsonPropertyName("met")]
    public bool? MetWith { get; set; }
    
    [JsonPropertyName("calling")]
    public bool? HasCalling { get; set; }
    
    [JsonPropertyName("pendingReview")]
    public bool? IsPendingReview { get; set; }
    
    [JsonPropertyName("datePulled")]
    public DateTime? DatePulled { get; set; }
    
    [JsonPropertyName("hasPhotoOnWall")]
    public bool? HasPhotoOnWall { get; set; }
}