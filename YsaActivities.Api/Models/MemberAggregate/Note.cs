using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using KaydenMiller.ApiTools;
using MongoDB.Bson;

namespace YsaActivities.Api.Models.MemberAggregate;

public class NoteIdConverter : JsonConverter<NoteId>
{
    public override NoteId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return NoteId.Parse(reader.GetString()!, null);
    }

    public override void Write(Utf8JsonWriter writer, NoteId value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}

[JsonConverter(typeof(NoteIdConverter))]
[ValueObject("string")]
public record struct NoteId(ObjectId Id) : IParsable<NoteId>
{
    public static NoteId Empty => new(ObjectId.Empty);
    
    public static NoteId Parse(string value, IFormatProvider? provider)
    {
        return new NoteId(ObjectId.Parse(value));
    }

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out NoteId result)
    {
        if (string.IsNullOrWhiteSpace(s))
        {
            result = Empty;
            return false;
        }
        
        try
        {
            result = Parse(s, provider);
            return true;
        }
        catch
        {
            result = Empty;
            return false;
        }
    }
}

[WithJsonConverter]
public class Note
{
    public NoteId Id { get; set; }
    
    /// <summary>
    /// The member who wrote the note
    /// </summary>
    public MemberId Author { get; set; }
    
    /// <summary>
    /// The time at which the note was created.
    /// </summary>
    public DateTime Created { get; set; }
    
    /// <summary>
    /// The contents of the note to be displayed.
    /// </summary>
    public string Content { get; set; }
}