using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace RecipeBook.API.Converters;

public partial class StringConverter : JsonConverter<string>
{
    [GeneratedRegex(@"\s+")]
    private static partial Regex WhitespaceReplacer();

    public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString()?.Trim();

        if (value is null) return null;

        WhitespaceReplacer().Replace(value, " ");

        return value;
    }

    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value);
    }
}