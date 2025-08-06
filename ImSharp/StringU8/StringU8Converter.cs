using System.Text.Json;
using System.Text.Json.Serialization;

namespace ImSharp;

/// <summary> Conversion from and to string. </summary>
public class StringU8Converter : JsonConverter<StringU8>
{
    /// <inheritdoc/>
    public override StringU8 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => new(reader.ValueSpan, false);

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, StringU8 value, JsonSerializerOptions options)
        => writer.WriteStringValue(value);
}
