using DataFac.Compression;
using DataFac.Hashing;
using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DataFac.MemBlox2.Tests;

public class EmbeddedIdTests
{
    [Fact]
    public void Embedded01_IdFromEmptyBlob()
    {
        var data = ReadOnlyMemory<byte>.Empty;
        Span<byte> idSpan = stackalloc byte[BlobIdV1.Size];
        (bool embedded, ReadOnlyMemory<byte> packedBuffer) = BlobHelpers.CompressData(data, idSpan);

        BlobIdV1.ToDisplayString(idSpan).ShouldBe("U:0:");
    }

    [Fact]
    public async Task Embedded02_IdFromEmptyText()
    {
        var text = string.Empty;
        Span<byte> idSpan = stackalloc byte[BlobIdV1.Size];
        (bool embedded, ReadOnlyMemory<byte> packedBuffer) = BlobHelpers.CompressText(text, idSpan);

        BlobIdV1.ToDisplayString(idSpan).ShouldBe("U:0:");
    }

    [Fact]
    public async Task Embedded03_IdFromNonEmptyBlob()
    {
        var data = new ReadOnlyMemory<byte>(Enumerable.Range(0, 62).Select(i => (byte)i).ToArray());
        Span<byte> idSpan = stackalloc byte[BlobIdV1.Size];
        (bool embedded, ReadOnlyMemory<byte> packedBuffer) = BlobHelpers.CompressData(data, idSpan);

        BlobIdV1.ToDisplayString(idSpan).ShouldBe("U:62:AAECAwQFBgcICQoLDA0ODxAREhMUFRYXGBkaGxwdHh8gISIjJCUmJygpKissLS4vMDEyMzQ1Njc4OTo7PD0=");
    }

    [Fact]
    public async Task Embedded04_IdFromNonEmptyText()
    {
        var text = new string('Z', 100);
        Span<byte> idSpan = stackalloc byte[BlobIdV1.Size];
        (bool embedded, ReadOnlyMemory<byte> packedBuffer) = BlobHelpers.CompressText(text, idSpan);

        BlobIdV1.ToDisplayString(idSpan).ShouldBe("S:9:ZABa/gEAigEA");
    }

}

