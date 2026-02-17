using DataFac.Memory;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DTOMaker.Runtime.JsonSystemText;

/// <summary>
/// Converts instances of the PairOfInt64 type to and from JSON using the System.Text.Json serialization framework.
/// </summary>
/// <remarks>This converter serializes PairOfInt64 objects as JSON objects with numeric properties named "a" and
/// "b". During deserialization, it accepts property names "A" or "a" for the first value and "B" or "b" for the second
/// value. The converter expects both properties to be present and represented as numbers in the JSON object.</remarks>
public class PairOfInt64Converter : JsonConverter<PairOfInt64>
{
    /// <summary>
    /// Reads a JSON object and converts it to a PairOfInt64 instance containing two 64-bit integer values.
    /// </summary>
    /// <remarks>Property names 'A' and 'B' are matched case-insensitively. Both properties must be present in
    /// the JSON object.</remarks>
    /// <param name="reader">The Utf8JsonReader positioned at the start of the JSON object to read.</param>
    /// <param name="typeToConvert">The type of the object to convert. This parameter is not used.</param>
    /// <param name="options">Options to control the behavior of the JSON deserialization.</param>
    /// <returns>A PairOfInt64 instance with values populated from the 'A' and 'B' properties of the JSON object.</returns>
    /// <exception cref="JsonException">Thrown if the JSON does not start with an object, contains unexpected property names, or is missing required
    /// properties.</exception>
    public override PairOfInt64 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException("Expected StartObject token");

        long a = default;
        long b = default;

        // get pair of values
        while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
        {
            if (reader.TokenType != JsonTokenType.PropertyName) throw new JsonException("Expected PropertyName token");

            string propertyName = reader.GetString()!;
            reader.Read();
            Int64 value = reader.GetInt64();
            switch (propertyName)
            {
                case "A":
                case "a":
                    a = value;
                    break;
                case "B":
                case "b":
                    b = value;
                    break;
                default:
                    throw new JsonException($"Unexpected property name: {propertyName}");
            }
        }

        return new PairOfInt64(a, b);
    }

    /// <summary>
    /// Writes the specified PairOfInt64 value as a JSON object using the provided Utf8JsonWriter.
    /// </summary>
    /// <remarks>The resulting JSON object contains two number properties, 'a' and 'b', corresponding to the A
    /// and B values of the PairOfInt64 instance.</remarks>
    /// <param name="writer">The Utf8JsonWriter to which the JSON representation of the PairOfInt64 value is written. Cannot be null.</param>
    /// <param name="value">The PairOfInt64 instance containing the values to serialize as JSON properties.</param>
    /// <param name="options">The JsonSerializerOptions to use when writing the JSON. This parameter can be used to customize serialization
    /// behavior.</param>
    public override void Write(Utf8JsonWriter writer, PairOfInt64 value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteNumber("a", value.A);
        writer.WriteNumber("b", value.B);
        writer.WriteEndObject();
    }
}
