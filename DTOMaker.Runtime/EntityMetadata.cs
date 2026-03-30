using DataFac.Memory;
using DataFac.Storage;
using System;
using System.Collections.Immutable;
using System.IO;
using System.Threading.Tasks;

namespace DTOMaker.Runtime;

/// <summary>
/// Information about block-based entities.
/// </summary>
public readonly struct EntityMetadata : IEquatable<EntityMetadata>
{
    public const int HeaderSize = 16;
    public const int SignatureV21 = 0x01025f7c; // 1,2,_,|

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
    /// The offset in bytes, of the local block within the total block.
    /// </summary>
    public readonly int LocalBlockOffset;

    /// <summary>
    /// The length in bytes of the local block.
    /// </summary>
    public readonly int LocalBlockLength;

    /// <summary>
    /// Combined length of all memory blocks of the entity, including the header.
    /// </summary>
    public readonly int TotalLength;

    private static readonly ReadOnlyMemory<int> _blockSizes
        = new ReadOnlyMemory<int>([0, 1, 2, 4, 8, 16, 32, 64, 128, 256, 512, 1024 * 1, 1024 * 2, 1024 * 4, 1024 * 8, 1024 * 16]);
    private static int GetBlockSize(int blockSizeCode) => _blockSizes.Span[blockSizeCode];

    public EntityMetadata(int entityId, long structureBits) : this()
    {
        SignatureBits = SignatureV21;
        EntityId = entityId;
        StructureBits = structureBits;
        long bits = StructureBits;
        int classHeight = (int)(bits & 0x0F);
        int totalLength = EntityMetadata.HeaderSize;
        for (int h = 0; h < classHeight && h < 15; h++)
        {
            bits = bits >> 4;
            int blockSizeCode = (int)(bits & 0x0F);
            int blockLength = GetBlockSize(blockSizeCode);
            LocalBlockOffset = totalLength;
            totalLength += blockLength;
            LocalBlockLength = blockLength;
        }
        TotalLength = totalLength;
    }

    public EntityMetadata(ReadOnlyMemory<byte> buffer) : this()
    {
        ReadOnlyMemory<byte> header = buffer.Slice(0, HeaderSize);
        BlockB016 block = default;
        if (!block.TryRead(header.Span)) throw new InvalidDataException($"Source buffer too small, expected at least {HeaderSize} bytes but received {buffer.Length} bytes.");
        //int signature = block.A.A.Int32ValueLE; todo? check signature?
        SignatureBits = SignatureV21;
        EntityId = block.A.B.Int32ValueLE;
        StructureBits = block.B.Int64ValueLE;
        // todo? integrity check of header? CRC32?
        long bits = StructureBits;
        int classHeight = (int)(bits & 0x0F);
        int totalLength = EntityMetadata.HeaderSize;
        for (int h = 0; h < classHeight && h < 15; h++)
        {
            bits = bits >> 4;
            int blockSizeCode = (int)(bits & 0x0F);
            int blockLength = GetBlockSize(blockSizeCode);
            LocalBlockOffset = totalLength;
            totalLength += blockLength;
            LocalBlockLength = blockLength;
        }
        TotalLength = totalLength;
    }

    public void WriteTo(Span<byte> target)
    {
        BlockB016 block = default;
        block.A.A.Int32ValueLE = SignatureV21;
        block.A.B.Int32ValueLE = EntityId;
        block.B.Int64ValueLE = StructureBits;
        block.WriteTo(target);
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


