using DataFac.Memory;
using System;

namespace DTOMaker.Runtime.MemBlocks;

public readonly struct BlockHeader
{
    public const int HeaderSize = 16;
    private const int SignatureV11 = 0x01015f7c; // 1,1,_,|

    public static BlockHeader CreateNew(int entityId, long structureBits)
    {
        Memory<byte> memory = new byte[HeaderSize];
        Span<byte> headerSpan = memory.Span;
        Codec_Int32_LE.WriteToSpan(headerSpan.Slice(0, 4), SignatureV11);
        Codec_Int32_LE.WriteToSpan(headerSpan.Slice(4, 4), entityId);
        Codec_Int64_LE.WriteToSpan(headerSpan.Slice(8, 8), structureBits);
        return new BlockHeader(SignatureV11, entityId, structureBits, memory);
    }

    public static BlockHeader ParseFrom(ReadOnlyMemory<byte> buffer)
    {
        var header = buffer.Slice(0, HeaderSize);
        int signature = Codec_Int32_LE.ReadFromSpan(header.Span.Slice(0, 4));
        int entityId = Codec_Int32_LE.ReadFromSpan(header.Span.Slice(4, 4));
        long structureBits = Codec_Int64_LE.ReadFromSpan(header.Span.Slice(8, 8));
        return new BlockHeader(signature, entityId, structureBits, header);
    }

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

    public readonly ReadOnlyMemory<byte> Memory;

    private BlockHeader(int signatureBits, int entityId, long structureBits, ReadOnlyMemory<byte> memory)
    {
        SignatureBits = signatureBits;
        EntityId = entityId;
        StructureBits = structureBits;
        Memory = memory;
    }

    public bool Equals(BlockHeader other)
    {
        if (other.SignatureBits != SignatureBits) return false;
        if (other.EntityId != EntityId) return false;
        if (other.StructureBits != StructureBits) return false;
        return true;
    }

    public override bool Equals(object? obj) => obj is BlockHeader other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(SignatureBits, EntityId, StructureBits);
    public static bool operator ==(BlockHeader left, BlockHeader right) => left.Equals(right);
    public static bool operator !=(BlockHeader left, BlockHeader right) => !left.Equals(right);
}
