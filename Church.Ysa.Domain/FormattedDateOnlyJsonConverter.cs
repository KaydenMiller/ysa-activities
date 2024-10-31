using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Church.Ysa.Domain;

public class FormattedDateOnlyJsonConverter : JsonConverter<DateOnly>
{
    public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return DateOnly.ParseExact(reader.GetString()!.Trim(), "d MMM yyyy", CultureInfo.InvariantCulture);
    }

    public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
    {
        throw new NotImplementedException("We don't do this!");
    }
}