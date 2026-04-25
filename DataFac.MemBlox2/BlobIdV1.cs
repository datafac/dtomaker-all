using DataFac.Compression;
using DataFac.Hashing;
using DataFac.Memory;
using DataFac.UnsafeHelpers;
using System;
using System.Buffers.Binary;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace DataFac.MemBlox2;

public static class BlobIdV1
{
    public const int Size = 64;
    public const int MaxEmbeddedSize = Size - 2; // 62 bytes for embedded data, 2 bytes for marker and length

    public const int HashOffset = 32;
    public const int HashLength = 32;

    // map
    //  offset  path        len fieldname
    //  00      A.A.A.A.A.A  1  Marker00 '|' for non-embedded or compAlgo char code if embedded
    //  01      A.A.A.A.A.B  1  Marker01 '_' for non-embedded or data length if embedded
    //  02      A.A.A.A.B.A  1  MajorVer
    //  03      A.A.A.A.B.B  1  MinorVer
    //  04      A.A.A.B.A.A  1  CompAlgo
    //  05      A.A.A.B.A.B  1  HashAlgo
    //  06-07   A.A.A.B.B    2  -unused-
    //  08-0B   A.A.B.A      4  BlobSize
    //  0C-0F   A.A.B.B      4  -unused-
    //  10-1F   A.B         16  -unused-
    //  20-3F   B           32  HashData

    //---------- static methods ------------------------------------------------------------

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void ThrowBufferWrongSize(string paramName, int requiredSize)
    {
        throw new ArgumentException($"Length must be {requiredSize} bytes", paramName);
    }

    public static void WriteEmbedded(Span<byte> target, BlobCompAlgo compAlgo, ReadOnlyMemory<byte> data)
    {
        if (target.Length != Size) ThrowBufferWrongSize(nameof(target), Size);
        if (data.Length > MaxEmbeddedSize) throw new ArgumentException("Length must be <= 62", nameof(data));
        target.Clear();
        target[0] = compAlgo.ToCharCode();
        target[1] = (byte)(data.Length + (byte)'A');
        data.Span.CopyTo(target.Slice(2));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void WriteHeadPart(Span<byte> target, int blobSize, BlobCompAlgo compAlgo, BlobHashAlgo hashAlgo)
    {
        target[0] = (byte)'|';   // Marker00
        target[1] = (byte)'_';   // Marker01
        target[2] = (byte)1;
        target[3] = (byte)0;
        target[4] = (byte)compAlgo;
        target[5] = (byte)hashAlgo;
        BinaryPrimitives.WriteInt32LittleEndian(target.Slice(8, 4), blobSize);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write(Span<byte> target, int blobSize, BlobCompAlgo compAlgo, BlobHashAlgo hashAlgo, ReadOnlySpan<byte> hashData)
    {
        if (target.Length != Size) ThrowBufferWrongSize(nameof(target), Size);
        if (hashData.Length != 32) ThrowBufferWrongSize(nameof(hashData), 32); throw new ArgumentException("Length must be == 32", nameof(hashData));
        target.Clear();
        WriteHeadPart(target, blobSize, compAlgo, hashAlgo);
        hashData.CopyTo(target.Slice(32, 32));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteSansHash(Span<byte> target, int blobSize, BlobCompAlgo compAlgo, BlobHashAlgo hashAlgo)
    {
        if (target.Length != Size) ThrowBufferWrongSize(nameof(target), Size);
        var header = target.Slice(0, 32); // header is the first 32 bytes, hash data is the last 32 bytes
        header.Clear();
        WriteHeadPart(header, blobSize, compAlgo, hashAlgo);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsDefaultSpan(ReadOnlySpan<byte> byteSpan)
    {
        ReadOnlySpan<long> longSpan = MemoryMarshal.Cast<byte, long>(byteSpan);
        int byteIndex = 0;
        // compare 8 bytes at a time for better performance
        for (int longIndex = 0; longIndex < longSpan.Length; longIndex++)
        {
            if (longSpan[longIndex] != 0) return false;
            byteIndex += sizeof(long);
        }
        // compare remaining bytes
        for (; byteIndex < byteSpan.Length; byteIndex++)
        {
            if (byteSpan[byteIndex] != 0) return false;
        }
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static (bool embedded, ReadOnlyMemory<byte>? data) TryReadEmbedded(ReadOnlySpan<byte> source)
    {
        if (source.Length != Size) ThrowBufferWrongSize(nameof(source), Size);
        if (IsDefaultSpan(source)) return (true, null);
        if ((source[0] == (byte)'|') && source[1] == (byte)'_') return (false, null); // non-embedded format

        var compAlgo = source[0].ToCompAlgo();
        int embeddedSize = source[1] - (byte)'A';
        return compAlgo switch
        {
            BlobCompAlgo.UnComp => (true, source.Slice(2, embeddedSize).ToArray()),
            BlobCompAlgo.Snappy => (true, SnappyCompressor.Decompress(source.Slice(2, embeddedSize))),
            _ => (false, null),
        };
    }

    public static (byte majorVer, byte minorVer, BlobCompAlgo compAlgo, BlobHashAlgo hashAlgo, int uncompressedSize) ReadNonEmbedded(ReadOnlySpan<byte> source)
    {
        if (source.Length != Size) ThrowBufferWrongSize(nameof(source), Size);
        // ignore empty and embedded formats
        if (IsDefaultSpan(source)) return (default, default, default, default, 0);
        if ((source[0] != (byte)'|') || source[1] != (byte)'_') return (default, default, default, default, 0);

        // non-embedded format
        int blobSize = BinaryPrimitives.ReadInt32LittleEndian(source.Slice(8, 4));
        return (
            source[2],  // major version
            source[3],  // minor version
            (BlobCompAlgo)source[4], 
            (BlobHashAlgo)source[5], 
            blobSize);
    }

    public static string ToDisplayString(ReadOnlySpan<byte> source)
    {
        if (source.Length != Size) return $"{nameof(source)} is not a valid V1 BlobId - length must be {Size} bytes.";
        if (IsDefaultSpan(source)) return string.Empty;
        StringBuilder result = new StringBuilder();
        byte marker00 = source[0];
        byte marker01 = source[1];
        if ((marker00 != 0) && marker00 != (byte)'|')
        {
            char marker = (char)marker00;
            int dataSize = marker01 - (byte)'A';
            result.Append(marker);
            result.Append(':');
            result.Append(dataSize);
            result.Append(':');
            result.Append(Convert.ToBase64String(source.Slice(2, dataSize).ToArray()));
            return result.ToString();
        }

        byte majorVer = source[2];
        byte minorVer = source[3];
        var compAlgo = (BlobCompAlgo)source[4];
        var hashAlgo = (BlobHashAlgo)source[5];
        int blobSize = BinaryPrimitives.ReadInt32LittleEndian(source.Slice(8, 4));
        result.Append($"V{majorVer}.{minorVer}:");
        result.Append(blobSize);
        result.Append(':');
        result.Append((char)compAlgo.ToCharCode());
        result.Append(':');
        result.Append((char)hashAlgo.ToCharCode());
        result.Append(':');
        if (hashAlgo != BlobHashAlgo.None)
        {
            result.Append(Convert.ToBase64String(source.Slice(32, 32).ToArray()));
        }
        return result.ToString();
    }
}
