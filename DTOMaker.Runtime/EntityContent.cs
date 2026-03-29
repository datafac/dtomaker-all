using DataFac.Memory;
using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;

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

    /// <summary>
    /// The total length of the contained memory blocks.
    /// </summary>
    public readonly int Length;

    /// <summary>
    /// True if the buffers are slices of a single contiguous memory block.
    /// </summary>
    private readonly bool _hasBackingBuffer;

    /// <summary>
    /// The single contiguous memory block backing the buffers.
    /// </summary>
    private readonly ReadOnlyMemory<byte> _backingBuffer;

    private EntityContent(EntityMetadata metadata, ImmutableArray<ReadOnlyMemory<byte>> buffers, ReadOnlyMemory<byte> backingBuffer) : this()
    {
        SignatureBits = metadata.SignatureBits;
        EntityId = metadata.EntityId;
        StructureBits = metadata.StructureBits;
        Buffers = buffers;
        Length = buffers.Sum(b => b.Length);
        _hasBackingBuffer = true;
        _backingBuffer = backingBuffer;
    }

    public EntityContent(EntityMetadata metadata, ImmutableArray<ReadOnlyMemory<byte>> buffers) : this()
    {
        SignatureBits = metadata.SignatureBits;
        EntityId = metadata.EntityId;
        StructureBits = metadata.StructureBits;
        Buffers = buffers;
        Length = buffers.Sum(b => b.Length);
        _hasBackingBuffer = false;
        _backingBuffer = ReadOnlyMemory<byte>.Empty;
    }

    public ReadOnlyMemory<byte> ToSingleBuffer()
    {
        if (_hasBackingBuffer) return _backingBuffer;
        if (Buffers.Length == 0) return ReadOnlyMemory<byte>.Empty;
        if (Buffers.Length == 1) return Buffers[0];
        var buffers = Buffers.AsSpan();
        int totalLength = 0;
        for (int i = 0; i < buffers.Length; i++)
        {
            totalLength += buffers[i].Length;
        }
        Memory<byte> memory = new byte[totalLength];
        var span = memory.Span;
        int offset = 0;
        for (int i = 0; i < buffers.Length; i++)
        {
            var buffer = buffers[i].Span;
            buffer.CopyTo(span.Slice(offset));
            offset += buffer.Length;
        }
        return memory;
    }

    private static readonly ReadOnlyMemory<int> _blockSizes
        = new ReadOnlyMemory<int>([0, 1, 2, 4, 8, 16, 32, 64, 128, 256, 512, 1024 * 1, 1024 * 2, 1024 * 4, 1024 * 8, 1024 * 16]);
    private static int GetBlockSize(int blockSizeCode) => _blockSizes.Span[blockSizeCode];

    public static EntityContent FromSingleBuffer(EntityMetadata expectedMetadata, ReadOnlyMemory<byte> buffer)
    {
        // get incoming header and compare
        EntityMetadata receivedMetadata = EntityMetadata.Decode(buffer);
        if (receivedMetadata != expectedMetadata) throw new InvalidDataException($"Expected metadata [{expectedMetadata}] but received [{receivedMetadata}]");

        // get remaining blocks
        long bits = expectedMetadata.StructureBits;
        int classHeight = (int)(bits & 0x0F);
        var builder = ImmutableArray.CreateBuilder<ReadOnlyMemory<byte>>(classHeight + 1);
        builder.Add(receivedMetadata.Memory);
        int startPosition = EntityMetadata.HeaderSize;
        for (int h = 0; h < classHeight && h < 15; h++)
        {
            bits = bits >> 4;
            int blockSizeCode = (int)(bits & 0x0F);
            int blockLength = GetBlockSize(blockSizeCode);
            var block = buffer.Slice(startPosition, blockLength);
            builder.Add(block);
            startPosition += blockLength;
        }

        return new EntityContent(receivedMetadata, builder.ToImmutable(), buffer);
    }
}


