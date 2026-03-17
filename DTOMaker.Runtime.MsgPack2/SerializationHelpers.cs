using DTOMaker.Models;
using MessagePack;
using MessagePack.Resolvers;
using System;

namespace DTOMaker.Runtime.MsgPack2
{
    /// <summary>
    /// Serialization helpers for MsgPack2 generated entities.
    /// </summary>
    public static class SerializationHelpers
    {
        private static readonly IFormatterResolver _resolver = CompositeResolver.Create(
                // resolve custom types first
                CustomResolver.Instance,
                // then use standard resolver
                StandardResolver.Instance
            );

        private static readonly MessagePackSerializerOptions _options = MessagePackSerializerOptions.Standard.WithResolver(_resolver);

        /// <summary>
        /// Serializes an entity to a buffer using the MessagePack serializer.
        /// </summary>
        public static ReadOnlyMemory<byte> SerializeToMessagePack<T>(this T value)
        {
            return MessagePackSerializer.Serialize<T>(value, _options);
        }

        /// <summary>
        /// Deerializes an entity from a buffer using the MessagePack serializer.
        /// </summary>
        public static T DeserializeFromMessagePack<T>(this ReadOnlyMemory<byte> buffer)
            where T : IEntityBase
        {
            T result = MessagePackSerializer.Deserialize<T>(buffer, _options);
            result.Freeze();
            return result;
        }
    }
}
