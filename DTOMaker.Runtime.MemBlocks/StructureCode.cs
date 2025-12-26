using System;

namespace DTOMaker.Runtime.MemBlocks;

internal readonly struct StructureCode
{
    // indexes:                                  0, 1, 2, 3, 4,  5,  6,  7,   8,   9,  10,       11,       12,       13,       14,        15
    private static readonly int[] _blockSizes = [0, 1, 2, 4, 8, 16, 32, 64, 128, 256, 512, 1024 * 1, 1024 * 2, 1024 * 4, 1024 * 8, 1024 * 16];
    private static int GetBlockSizeCode(int blockLength)
    {
        ReadOnlySpan<int> blockSizes = _blockSizes;
        for (byte i = 0; i < blockSizes.Length; i++)
        {
            int blockSize = blockSizes[i];
            if (blockLength <= blockSize)
                return i;
        }

        // unsupported large block size - todo error handling
        return 15; // 16K
    }
    private readonly long _bits;
    private StructureCode(long bits)
    {
        _bits = bits;
    }

    public long Bits => _bits;
    public StructureCode(int classHeight, int outerBlockLength)
    {
        // todo check class height
        int blockSizeCode = GetBlockSizeCode(outerBlockLength);
        long init = (long)classHeight & 0x0F;
        long bits = (long)blockSizeCode << (classHeight * 4);
        _bits = (init | bits);
    }

    public StructureCode AddInnerBlock(int innerHeight, int innerBlockLength)
    {
        int blockSizeCode = GetBlockSizeCode(innerBlockLength);
        long bits = (long)blockSizeCode << (innerHeight * 4);
        return new StructureCode(_bits | bits);
    }

    public static int GetBlockSize(int blockSizeCode) => _blockSizes.AsSpan()[blockSizeCode];
}
