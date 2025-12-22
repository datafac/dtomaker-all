using MessagePack;
using MessagePack.Resolvers;
using System;

namespace DTOMaker.Runtime.MessagePack
{
    public static class SerializationHelpers
    {
        private static readonly IFormatterResolver _resolver = CompositeResolver.Create(
                // resolve custom types first
                CustomResolver.Instance,
                // then use standard resolver
                StandardResolver.Instance
            );

        private static readonly MessagePackSerializerOptions _options = MessagePackSerializerOptions.Standard.WithResolver(_resolver);

        public static ReadOnlyMemory<byte> SerializeToMessagePack<T>(this T value)
        {
            return MessagePackSerializer.Serialize<T>(value, _options);
        }

        public static T DeserializeFromMessagePack<T>(this ReadOnlyMemory<byte> buffer)
        {
            return MessagePackSerializer.Deserialize<T>(buffer, _options);
        }
    }
}
