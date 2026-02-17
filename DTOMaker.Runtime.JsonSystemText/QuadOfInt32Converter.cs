using DataFac.Memory;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DTOMaker.Runtime.JsonSystemText;

/// <summary>
/// Converts instances of the QuadOfInt32 type to and from JSON using the System.Text.Json serialization framework.
/// </summary>
/// <remarks>This converter serializes QuadOfInt32 objects as JSON objects with properties "a", "b", "c" and "d"
/// representing the four integer values. During deserialization, it expects a JSON object containing these properties
/// (case-insensitive). An exception is thrown if the JSON structure does not match the expected format.</remarks>
public class QuadOfInt32Converter : JsonConverter<QuadOfInt32>
{
    /// <summary>
    /// Reads a JSON object and converts it into a QuadOfInt32 instance containing four integer values, A, B, C and D.
    /// </summary>
    /// <param name="reader">The Utf8JsonReader positioned at the start of the JSON object to read. The reader is advanced as the object is
    /// parsed.</param>
    /// <param name="typeToConvert">The type of the object to convert. This parameter is not used in this implementation.</param>
    /// <param name="options">Options to control the behavior of the JSON deserialization process.</param>
    /// <returns>A QuadOfInt32 instance populated with the values extracted from the JSON object.</returns>
    /// <exception cref="JsonException">Thrown if the JSON does not start with an object, if a property name is unexpected, or if the expected
    /// properties are not found.</exception>
    public override QuadOfInt32 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException("Expected StartObject token");

        Int32 a = default;
        Int32 b = default;
        Int32 c = default;
        Int32 d = default;

        // get Quad of values
        while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
        {
            if (reader.TokenType != JsonTokenType.PropertyName) throw new JsonException("Expected PropertyName token");

            string propertyName = reader.GetString()!;
            reader.Read();
            Int32 value = reader.GetInt32();
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
                case "C":
                case "c":
                    c = value;
                    break;
                case "D":
                case "d":
                    d = value;
                    break;
                default:
                    throw new JsonException($"Unexpected property name: {propertyName}");
            }
        }

        return new QuadOfInt32(a, b, c, d);
    }

    /// <summary>
    /// Writes the specified QuadOfInt32 value as a JSON object using the provided Utf8JsonWriter.
    /// </summary>
    /// <remarks>The resulting JSON object contains four properties corresponding to the values of the QuadOfInt32 instance.</remarks>
    /// <param name="writer">The Utf8JsonWriter to which the JSON representation of the value is written. Cannot be null.</param>
    /// <param name="value">The QuadOfInt32 value to write as a JSON object.</param>
    /// <param name="options">The options to use when writing the JSON. This parameter can be used to customize serialization behavior.</param>
    public override void Write(Utf8JsonWriter writer, QuadOfInt32 value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteNumber("a", value.A);
        writer.WriteNumber("b", value.B);
        writer.WriteNumber("c", value.C);
        writer.WriteNumber("d", value.D);
        writer.WriteEndObject();
    }
}
