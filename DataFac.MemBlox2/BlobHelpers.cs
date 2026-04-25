using DataFac.Compression;
using DataFac.Hashing;
using System;

namespace DataFac.MemBlox2;

public static class BlobHelpers
{
    public static (bool embedded, ReadOnlyMemory<byte> compressedData) CompressData(ReadOnlyMemory<byte> uncompressed, Span<byte> idSpan)
    {
        // Snappier compression and hashing
        var compressResult = SnappyCompressor.CompressData(uncompressed, idSpan.Slice(32, 32), BlobIdV1.MaxEmbeddedSize);

        // embed compressed if small engough
        if (compressResult.Output.Length <= BlobIdV1.MaxEmbeddedSize)
        {
            BlobIdV1.WriteEmbedded(idSpan, compressResult.CompAlgo, compressResult.Output);
            return (true, ReadOnlyMemory<byte>.Empty);
        }
        else
        {
            BlobIdV1.WriteSansHash(idSpan, compressResult.InputSize, compressResult.CompAlgo, BlobHashAlgo.Sha256);
            return (false, compressResult.Output);
        }
    }

    public static (bool embedded, ReadOnlyMemory<byte> compressedData) CompressText(string text, Span<byte> idSpan)
    {
        // Snappier compression and hashing
        var compressResult = SnappyCompressor.CompressText(text, idSpan.Slice(32, 32), BlobIdV1.MaxEmbeddedSize);

        // embed compressed if small engough
        if (compressResult.Output.Length <= BlobIdV1.MaxEmbeddedSize)
        {
            BlobIdV1.WriteEmbedded(idSpan, compressResult.CompAlgo, compressResult.Output);
            return (true, ReadOnlyMemory<byte>.Empty);
        }
        else
        {
            BlobIdV1.WriteSansHash(idSpan, compressResult.InputSize, compressResult.CompAlgo, BlobHashAlgo.Sha256);
            return (false, compressResult.Output);
        }
    }

    public static (bool embedded, ReadOnlyMemory<byte>? decompressed) TryGetEmbedded(ReadOnlySpan<byte> blobId)
    {
        return BlobIdV1.TryReadEmbedded(blobId);
    }

    public static ReadOnlyMemory<byte> DecompressData(ReadOnlySpan<byte> idSpan, ReadOnlyMemory<byte> blobData)
    {
        var (_, _, compAlgo, _, _) = BlobIdV1.ReadNonEmbedded(idSpan);
        switch (compAlgo)
        {
            case BlobCompAlgo.UnComp:
                return blobData;
            case BlobCompAlgo.Snappy:
                return SnappyCompressor.Decompress(blobData.Span);
            default:
                throw new NotSupportedException($"Compression algorithm {compAlgo} not supported.");
        }
    }
}
