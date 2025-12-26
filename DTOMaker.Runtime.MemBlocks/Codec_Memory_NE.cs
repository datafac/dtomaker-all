using System;
using System.Buffers.Binary;

namespace DTOMaker.Runtime.MemBlocks;

public static class Codec_Memory_NE
{
    /// <summary>
    /// Zero-allocation read from memory block.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static ReadOnlyMemory<byte>? ReadFromMemory(ReadOnlyMemory<byte> source)
    {
        // null check
        ReadOnlySpan<byte> sourceSpan = source.Span;
        ushort header0 = BinaryPrimitives.ReadUInt16LittleEndian(sourceSpan.Slice(0, 2));
        ushort header1 = BinaryPrimitives.ReadUInt16LittleEndian(sourceSpan.Slice(2, 2));
        if (header0 == 0x0000 && header1 == 0xFFFF) return null;

        int sourceLen = sourceSpan.Length;
        if (sourceLen <= 256)
        {
            int length = sourceSpan[0];
            return length == 0
                ? ReadOnlyMemory<byte>.Empty
                : source.Slice(1, length);
        }
        else
        {
            int length = header0;
            return length == 0
                ? ReadOnlyMemory<byte>.Empty
                : source.Slice(2, length);
        }
    }

    public static void WriteToSpan(Memory<byte> target, ReadOnlyMemory<byte>? source)
    {
        Span<byte> targetSpan = target.Span;
        targetSpan.Clear();
        if (source is null)
        {
            BinaryPrimitives.WriteUInt16LittleEndian(targetSpan.Slice(2, 2), ushort.MaxValue);
            return;
        }

        var sourceSpan = source.Value.Span;
        if (targetSpan.Length <= 256)
        {
            sourceSpan.CopyTo(targetSpan.Slice(1));
            targetSpan[0] = (byte)sourceSpan.Length;
        }
        else
        {
            sourceSpan.CopyTo(targetSpan.Slice(2));
            BinaryPrimitives.WriteUInt16LittleEndian(targetSpan.Slice(0, 2), (ushort)sourceSpan.Length);
        }
    }
}
