using System.Text.Json;
#if HAS_NEWTONSOFT
using JsonSerializer = Newtonsoft.Json.JsonSerializer;
#endif

namespace ImSharp;

#if HAS_NEWTONSOFT
/// <summary> Conversion from and to string. </summary>
public class StringU8ConverterNewtonSoft : JsonConverter<StringU8>
{
    /// <inheritdoc/>
    public override void WriteJson(JsonWriter writer, StringU8 value, JsonSerializer serializer)
        => writer.WriteValue(value.ToString());

    /// <inheritdoc/>
    public override StringU8 ReadJson(JsonReader reader, Type objectType, StringU8 existingValue, bool hasExistingValue,
        JsonSerializer serializer)
    {
        var text = reader.ReadAsString();
        if (text is null)
            return hasExistingValue ? existingValue : StringU8.Empty;

        return new StringU8(text);
    }
}
#endif

/// <summary> Conversion from and to string. </summary>
public class StringU8Converter : System.Text.Json.Serialization.JsonConverter<StringU8>
{
    /// <inheritdoc/>
    public override StringU8 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => new(reader.ValueSpan, false);

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, StringU8 value, JsonSerializerOptions options)
        => writer.WriteStringValue(value);
}
