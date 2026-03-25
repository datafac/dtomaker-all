using DataFac.Memory;
using System;
using System.Collections.Immutable;
using System.Linq;

namespace DTOMaker.Runtime.MemBlox2;

public readonly struct BlockHeader
{
    public const int HeaderSize = 16;
    private const int SignatureV21 = 0x01025f7c; // 1,2,_,|

    private static ReadOnlyMemory<byte> CreateHeaderBuffer(int entityId, long structureBits)
    {
        Memory<byte> header = new byte[HeaderSize];
        Span<byte> headerSpan = header.Span;
        Codec_Int32_LE.WriteToSpan(headerSpan.Slice(0, 4), SignatureV21);
        Codec_Int32_LE.WriteToSpan(headerSpan.Slice(4, 4), entityId);
        Codec_Int64_LE.WriteToSpan(headerSpan.Slice(8, 8), structureBits);
        return header;
    }

    //public static BlockHeader ParseFrom(ReadOnlyMemory<byte> buffer)
    //{
    //    var header = buffer.Slice(0, HeaderSize);
    //    var headerSpan = header.Span;
    //    int signature = Codec_Int32_LE.ReadFromSpan(headerSpan.Slice(0, 4));
    //    int entityId = Codec_Int32_LE.ReadFromSpan(headerSpan.Slice(4, 4));
    //    long structureBits = Codec_Int64_LE.ReadFromSpan(headerSpan.Slice(8, 8));
    //    return new BlockHeader(signature, entityId, structureBits, header);
    //}

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
    /// Returns the header as a block of bytes
    /// </summary>
    public readonly ReadOnlyMemory<byte> Memory;

    /// <summary>
    /// Returns the header as a readable string.
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"{SignatureBits:X8},{EntityId},0x{StructureBits:X8}";

    private BlockHeader(int signatureBits, int entityId, long structureBits, ReadOnlyMemory<byte> memory)
    {
        SignatureBits = signatureBits;
        EntityId = entityId;
        StructureBits = structureBits;
        Memory = memory;
    }

    public BlockHeader(int entityId, long structureBits)
    {
        SignatureBits = SignatureV21;
        EntityId = entityId;
        StructureBits = structureBits;
        Memory = CreateHeaderBuffer(entityId, structureBits);
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

/// <summary>
/// Wrapper type that contains the readonly buffers of a MemBlox2 entity.
/// </summary>
public readonly struct ReadOnlyBuffers
{
    public readonly BlockHeader Header;
    public readonly ImmutableArray<ReadOnlyMemory<byte>> Buffersqqq;

    public ReadOnlyBuffers(BlockHeader header, ImmutableArray<ReadOnlyMemory<byte>> buffers) : this()
    {
        Header = header;
        Buffersqqq = buffers;
    }
}
