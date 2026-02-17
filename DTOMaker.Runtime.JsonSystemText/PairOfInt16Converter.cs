using DataFac.Memory;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DTOMaker.Runtime.JsonSystemText;

/// <summary>
/// Converts between JSON and instances of the PairOfInt16 type using the System.Text.Json serialization framework.
/// </summary>
/// <remarks>This converter serializes PairOfInt16 objects as JSON objects with properties "a" and "b"
/// representing the two Int16 values. During deserialization, it expects a JSON object with these properties
/// (case-insensitive). An exception is thrown if the JSON structure does not match the expected format or contains
/// unexpected property names.</remarks>
public class PairOfInt16Converter : JsonConverter<PairOfInt16>
{
    /// <summary>
    /// Reads a JSON object and converts it to a PairOfInt16 instance containing two 16-bit integer values.
    /// </summary>
    /// <remarks>The JSON object must contain properties named 'A' and 'B' (case-insensitive), each
    /// representing a 16-bit integer value.</remarks>
    /// <param name="reader">The Utf8JsonReader positioned at the start of the JSON object to read.</param>
    /// <param name="typeToConvert">The type of the object to convert. This parameter is not used.</param>
    /// <param name="options">Options to control the behavior of the JSON deserialization.</param>
    /// <returns>A PairOfInt16 instance populated with values from the 'A' and 'B' properties of the JSON object.</returns>
    /// <exception cref="JsonException">Thrown if the JSON does not start with an object, contains unexpected property names, or is missing required
    /// properties.</exception>
    public override PairOfInt16 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException("Expected StartObject token");

        Int16 a = default;
        Int16 b = default;

        // get pair of values
        while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
        {
            if (reader.TokenType != JsonTokenType.PropertyName) throw new JsonException("Expected PropertyName token");

            string propertyName = reader.GetString()!;
            reader.Read();
            Int16 value = reader.GetInt16();
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

        return new PairOfInt16(a, b);
    }

    /// <summary>
    /// Writes the specified PairOfInt16 value as a JSON object using the provided Utf8JsonWriter.
    /// </summary>
    /// <remarks>The resulting JSON object contains two properties: "a" and "b", corresponding to the A and B
    /// values of the PairOfInt16 instance.</remarks>
    /// <param name="writer">The Utf8JsonWriter to which the JSON representation of the PairOfInt16 value is written. Must not be null.</param>
    /// <param name="value">The PairOfInt16 value to serialize as a JSON object.</param>
    /// <param name="options">The options to use for serialization customization. This parameter can be used to influence serialization
    /// behavior.</param>
    public override void Write(Utf8JsonWriter writer, PairOfInt16 value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteNumber("a", value.A);
        writer.WriteNumber("b", value.B);
        writer.WriteEndObject();
    }
}
