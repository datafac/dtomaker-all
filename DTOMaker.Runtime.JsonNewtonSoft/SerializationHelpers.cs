using Newtonsoft.Json;
using System.Globalization;
using System.IO;
using System.Text;

namespace DTOMaker.Runtime.JsonNewtonSoft
{
    /// <summary>
    /// Provides helper methods for serializing and deserializing objects to and from JSON using predefined settings.
    /// </summary>
    /// <remarks>This class uses Newtonsoft.Json with settings that include indented formatting, automatic
    /// type name handling, and ignoring default values. The methods are intended to simplify common serialization and
    /// deserialization scenarios with consistent configuration.
    /// Todo: Use entity id as type discriminator when serializing/deserializing polymorphic types.
    /// </remarks>
    public static class SerializationHelpers
    {
        private static readonly JsonSerializerSettings _settings = new JsonSerializerSettings()
        {
            Formatting = Formatting.Indented,
            DefaultValueHandling = DefaultValueHandling.Ignore,
            TypeNameHandling = TypeNameHandling.Auto,
        };

        private static readonly JsonSerializer _serializer = JsonSerializer.Create(_settings);

        /// <summary>
        /// Serializes the specified value to its JSON string representation.
        /// </summary>
        /// <remarks>The serialization uses the default settings of the underlying JSON serializer. If the
        /// value contains properties that cannot be serialized, an exception may be thrown.</remarks>
        /// <typeparam name="T">The type of the object to serialize.</typeparam>
        /// <param name="value">The value to serialize to JSON. Can be any serializable object.</param>
        /// <returns>A string containing the JSON representation of the specified value.</returns>
        public static string SerializeToJson<T>(this T value)
        {
            using var sw = new StringWriter(new StringBuilder(256), CultureInfo.InvariantCulture);
            using var jw = new JsonTextWriter(sw);
            jw.Formatting = _serializer.Formatting;
            _serializer.Serialize(jw, value, typeof(T));
            return sw.ToString();
        }

        /// <summary>
        /// Deserializes the JSON string to an object of the specified type.
        /// </summary>
        /// <remarks>This method uses the default JSON serializer settings. The input string must contain
        /// valid JSON that matches the structure of type T.</remarks>
        /// <typeparam name="T">The type of the object to deserialize to.</typeparam>
        /// <param name="input">The JSON string to deserialize. Cannot be null.</param>
        /// <returns>An instance of type T deserialized from the JSON string, or null if the input is empty or represents a null
        /// value.</returns>
        public static T? DeserializeFromJson<T>(this string input)
        {
            using var sr = new StringReader(input);
            using var jr = new JsonTextReader(sr);
            return _serializer.Deserialize<T>(jr);
        }
    }
}
