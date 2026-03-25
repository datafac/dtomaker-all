using DataFac.Memory;
using DataFac.Storage;
using System;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace DTOMaker.Runtime;

/// <summary>
/// Information about block-based entities.
/// </summary>
public readonly struct EntityMetadata : IEquatable<EntityMetadata>
{
    private const int HeaderSize = 16;
    private const int SignatureV21 = 0x01025f7c; // 1,2,_,|

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
    /// Class height (inheritance depth excluding the entity base type).
    /// </summary>
    public int ClassHeight => (int)(StructureBits & 0x0F);

    /// <summary>
    /// The entity info encoded in 16 bytes.
    /// </summary>
    public readonly ReadOnlyMemory<byte> Memory;

    public EntityMetadata(int entityId, long structureBits) : this()
    {
        SignatureBits = SignatureV21;
        EntityId = entityId;
        StructureBits = structureBits;
        Memory = Encode(entityId, structureBits);
    }

    private static ReadOnlyMemory<byte> Encode(int entityId, long structureBits)
    {
        BlockB016 block = default;
        block.A.A.Int32ValueLE = SignatureV21;
        block.A.B.Int32ValueLE = entityId;
        block.B.Int64ValueLE = structureBits;
        Memory<byte> memory = new byte[HeaderSize];
        block.WriteTo(memory.Span);
        return memory;
    }

    public override string ToString() => $"0x{SignatureBits:X8},{EntityId},0x{StructureBits:X8}";

    public bool Equals(EntityMetadata other)
        => other.SignatureBits == SignatureV21
        && other.EntityId == EntityId
        && other.StructureBits == StructureBits;

    public override bool Equals(object? obj) => obj is EntityMetadata other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(SignatureBits, EntityId, StructureBits);
    public static bool operator ==(EntityMetadata left, EntityMetadata right) => left.Equals(right);
    public static bool operator !=(EntityMetadata left, EntityMetadata right) => !left.Equals(right);
}


