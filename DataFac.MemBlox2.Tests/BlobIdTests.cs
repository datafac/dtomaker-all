using DataFac.Compression;
using DataFac.Hashing;
using Microsoft.Testing.Platform.Extensions.Messages;
using Shouldly;
using System;
using System.Linq;
using Xunit;

namespace DataFac.MemBlox2.Tests;

public class BlobIdRegressionTests
{
    [Fact]
    public void BlobId01CreateWithZeroData()
    {
        ReadOnlySpan<byte> input = stackalloc byte[BlobIdV1.Size];
        BlobIdV1.ToDisplayString(input).ShouldBe("");
    }

    [Fact]
    public void BlobId02CreateWithInvalidSize()
    {
        var ex = Assert.ThrowsAny<ArgumentException>(() =>
        {
            ReadOnlySpan<byte> input = stackalloc byte[10];
            BlobIdV1.ReadNonEmbedded(input);
        });
        ex.Message.ShouldStartWith("Length must be 64 bytes");
    }

    [Fact]
    public void BlobId03CreateWithInvalidData()
    {
        // todo this should fail CRC checks
        ReadOnlySpan<byte> input = Enumerable.Range(0, BlobIdV1.Size).Select(i => (byte)i).ToArray();
        BlobIdV1.ToDisplayString(input).ShouldBe("V2.3:185207048:U:N:ICEiIyQlJicoKSorLC0uLzAxMjM0NTY3ODk6Ozw9Pj8=");
    }

    [Fact]
    public void BlobId04CreateEmbeddedUncompressed()
    {
        ReadOnlyMemory<byte> data = Enumerable.Range(0, 62).Select(i => (byte)i).ToArray();
        Memory<byte> idMemory = new byte[BlobIdV1.Size];
        (bool embedded1, var compressed) = BlobHelpers.CompressData(data, idMemory.Span);
        embedded1.ShouldBeTrue();

        (bool embedded2, _) = BlobHelpers.TryGetEmbedded(idMemory.Span);
        embedded2.ShouldBeTrue();

        BlobIdV1.ToDisplayString(idMemory.Span).ShouldBe("U:62:AAECAwQFBgcICQoLDA0ODxAREhMUFRYXGBkaGxwdHh8gISIjJCUmJygpKissLS4vMDEyMzQ1Njc4OTo7PD0=");
    }

    [Fact]
    public void BlobId05CreateEmbeddedCompressed()
    {
        string text = new string('z', 100);
        ReadOnlyMemory<byte> data = System.Text.Encoding.UTF8.GetBytes(text);
        Memory<byte> idMemory = new byte[BlobIdV1.Size];
        (bool embedded1, var compressed) = BlobHelpers.CompressData(data, idMemory.Span);
        embedded1.ShouldBeTrue();
        ReadOnlySpan<byte> idSpan = idMemory.Span;

        (bool embedded2, _) = BlobHelpers.TryGetEmbedded(idMemory.Span);
        embedded2.ShouldBeTrue();

        BlobIdV1.ToDisplayString(idSpan).ShouldBe("S:9:ZAB6/gEAigEA");
    }

    [Fact]
    public void BlobId06CompressedIsShorterThanOriginal()
    {
        string text =
            "The quick brown fox jumps over the lazy dog. " +
            "The quick brown dog jumps over the lazy fox. " +
            "The quick brown fox jumps over the lazy dog. ";
        ReadOnlyMemory<byte> data = System.Text.Encoding.UTF8.GetBytes(text);
        Memory<byte> idMemory = new byte[BlobIdV1.Size];
        (bool embedded, var compressed) = BlobHelpers.CompressData(data, idMemory.Span);
        embedded.ShouldBeFalse();
        BlobIdV1.ToDisplayString(idMemory.Span).ShouldBe("V1.0:135:S:S:Xz72lITGg+TNe3L7H60LLQXBRykLMJJiQLykcEVH94k=");
    }

    [Fact]
    public void BlobId07CompressedIsLongerThanOriginal()
    {
        ReadOnlyMemory<byte> data = Enumerable.Range(0, 256).Select(i => (byte)i).ToArray();
        Memory<byte> idMemory = new byte[BlobIdV1.Size];
        (bool embedded1, var compressed) = BlobHelpers.CompressData(data, idMemory.Span);
        embedded1.ShouldBeFalse();
        ReadOnlySpan<byte> idSpan = idMemory.Span;

        (bool embedded2, _) = BlobHelpers.TryGetEmbedded(idSpan);
        embedded2.ShouldBeFalse();

        (int majorVer, int minorVer, var compAlgo, var hashAlgo, int blobSize) = BlobIdV1.ReadNonEmbedded(idSpan);
        majorVer.ShouldBe(1);
        minorVer.ShouldBe(0);
        hashAlgo.ShouldBe(BlobHashAlgo.Sha256);
        compAlgo.ShouldBe(BlobCompAlgo.UnComp);
        blobSize.ShouldBe(256);
        BlobIdV1.ToDisplayString(idSpan).ShouldBe("V1.0:256:U:S:QK/y6dLYki5Hr9RkjmlnSXFYeF+9Hahw5xECZr+USIA=");
    }

    [Fact]
    public void BlobId08CreateFromSpanWithValidData()
    {
        string inputStr =
            "7C-5F-01-00-00-01-00-00-00-01-00-00-00-00-00-00-" +
            "00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-" +
            "40-AF-F2-E9-D2-D8-92-2E-47-AF-D4-64-8E-69-67-49-" +
            "71-58-78-5F-BD-1D-A8-70-E7-11-02-66-BF-94-48-80";

        ReadOnlySpan<byte> input = inputStr.Split('-').Select(s => (byte)int.Parse(s, System.Globalization.NumberStyles.HexNumber)).ToArray();

        (bool embedded, _) = BlobHelpers.TryGetEmbedded(input);
        embedded.ShouldBeFalse();

        (int majorVer, int minorVer, var compAlgo, var hashAlgo, int blobSize) = BlobIdV1.ReadNonEmbedded(input);
        majorVer.ShouldBe(1);
        minorVer.ShouldBe(0);
        hashAlgo.ShouldBe(BlobHashAlgo.Sha256);
        compAlgo.ShouldBe(BlobCompAlgo.UnComp);
        blobSize.ShouldBe(256);

        BlobIdV1.ToDisplayString(input).ShouldBe("V1.0:256:U:S:QK/y6dLYki5Hr9RkjmlnSXFYeF+9Hahw5xECZr+USIA=");
    }

}

