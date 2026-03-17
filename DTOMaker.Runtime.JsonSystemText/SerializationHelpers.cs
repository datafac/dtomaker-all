using DTOMaker.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DTOMaker.Runtime.JsonSystemText
{
    public static class SerializationHelpers
    {
        private static readonly JsonSerializerOptions _options = new JsonSerializerOptions(JsonSerializerDefaults.Web)
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
            NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
            Converters =
            {
                new PairOfInt16Converter(),
                new PairOfInt32Converter(),
                new PairOfInt64Converter(),
                new QuadOfInt32Converter(),
            }
        };

        public static string SerializeToJson<T>(this T value) => JsonSerializer.Serialize<T>(value, _options);
        public static T? DeserializeFromJson<T>(this string input)
            where T: IEntityBase
        {
            T? result = JsonSerializer.Deserialize<T>(input, _options);
            result?.Freeze();
            return result;
        }
    }
}
