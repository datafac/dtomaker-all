using DataFac.Memory;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DTOMaker.Runtime.JsonSystemText
{
    public class PairOfInt64Converter : JsonConverter<PairOfInt64>
    {
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

        public override void Write(Utf8JsonWriter writer, PairOfInt64 value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteNumber("a", value.A);
            writer.WriteNumber("b", value.B);
            writer.WriteEndObject();
        }
    }
}
