using DataFac.Memory;
using System;
using System.Buffers;

namespace DTOMaker.Runtime.MemBlocks;

public readonly struct SourceBlocks
{
    public readonly BlockHeader Header;
    public readonly int ClassHeight;
    public readonly ReadOnlyMemory<ReadOnlyMemory<byte>> Blocks;

    private SourceBlocks(BlockHeader header, int classHeight, ReadOnlyMemory<ReadOnlyMemory<byte>> blocks)
    {
        Header = header;
        ClassHeight = classHeight;
        Blocks = blocks;
    }

    private static readonly ReadOnlyMemory<int> _blockSizes
        = new ReadOnlyMemory<int>([0, 1, 2, 4, 8, 16, 32, 64, 128, 256, 512, 1024 * 1, 1024 * 2, 1024 * 4, 1024 * 8, 1024 * 16]);
    private static int GetBlockSize(int blockSizeCode) => _blockSizes.Span[blockSizeCode];

    public static SourceBlocks ParseFrom(ReadOnlySequence<byte> buffers)
    {
        int startPosition = 0;
        ReadOnlyMemory<byte> headerMemory = buffers.Slice(startPosition, BlockHeader.HeaderSize).Compact();
        startPosition += BlockHeader.HeaderSize;

        // parse header
        BlockHeader header = BlockHeader.ParseFrom(headerMemory);

        // get remaining blocks
        //ReadOnlySpan<int> blockSizes = _blockSizes.AsSpan();
        // if the source is a single element or the source elements match the target
        // structure, then the slice compactions will not allocate new memory.
        long bits = header.StructureBits;
        int classHeight = (int)(bits & 0x0F);
        Memory<ReadOnlyMemory<byte>> blocks = new ReadOnlyMemory<byte>[classHeight + 1];
        var blockSpan = blocks.Span;
        blockSpan[0] = headerMemory;
        for (int h = 0; h < classHeight && h < 15; h++)
        {
            bits = bits >> 4;
            int blockSizeCode = (int)(bits & 0x0F);
            int blockLength = GetBlockSize(blockSizeCode);
            ReadOnlyMemory<byte> block = buffers.Slice(startPosition, blockLength).Compact();
            startPosition += blockLength;
            blockSpan[h + 1] = block;
        }

        return new SourceBlocks(header, classHeight, blocks);
    }
}
