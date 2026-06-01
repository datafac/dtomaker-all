using DTOMaker.Models;
using MessagePack;
using MessagePack.Resolvers;
using System;
using System.Threading;

namespace DTOMaker.Runtime.MsgPack3
{
    /// <summary>
    /// Serialization helpers for MsgPack3 generated entities.
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
        public static ReadOnlyMemory<byte> SerializeToMessagePack<T>(this T entity, CancellationToken cancellationToken)
            where T : IEntityBase
        {
            entity.Freeze();
            return MessagePackSerializer.Serialize<T>(entity, _options, cancellationToken);
        }

        /// <summary>
        /// Deerializes an entity from a buffer using the MessagePack serializer.
        /// </summary>
        public static T DeserializeFromMessagePack<T>(this ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken)
            where T : IEntityBase
        {
            T entity = MessagePackSerializer.Deserialize<T>(buffer, _options, cancellationToken);
            entity.Freeze();
            return entity;
        }
    }
}
