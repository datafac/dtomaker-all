using DataFac.Storage;
using System;

namespace DTOMaker.Runtime.MemBlocks;

public static class Codec_BlobId_NE
{
    public static BlobIdV1 ReadFromMemory(ReadOnlyMemory<byte> source) => BlobIdV1.FromSpan(source.Span);
    public static BlobIdV1 ReadFromSpan(ReadOnlySpan<byte> source) => BlobIdV1.FromSpan(source);
    public static void WriteToSpan(Span<byte> target, BlobIdV1 value) => value.WriteTo(target);
}
