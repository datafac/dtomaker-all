using System;
using System.Collections.Immutable;

namespace DTOMaker.Runtime;

/// <summary>
/// All metadata and memory buffers containing the immutable state of the entity.
/// </summary>
public readonly struct EntityContent
{
    /// <summary>
    /// Marker and version bytes
    /// </summary>
    public readonly int SignatureBits;

    /// <summary>
    /// Entity identifier
    /// </summary>
    public readonly int EntityId;

    /// <summary>
    /// Class height and block size codes
    /// </summary>
    public readonly long StructureBits;

    /// <summary>
    /// Memory blocks of the frozen entity.
    /// </summary>
    public readonly ImmutableArray<ReadOnlyMemory<byte>> Buffers;

    public EntityContent(EntityMetadata metadata, ImmutableArray<ReadOnlyMemory<byte>> buffers) : this()
    {
        SignatureBits = metadata.SignatureBits;
        EntityId = metadata.EntityId;
        StructureBits = metadata.StructureBits;
        Buffers = buffers;
    }
}


