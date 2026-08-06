using DTOMaker.Models;
using DTOMaker.SrcGen.MemBlocks.IntTests.MemBlocks;
using Shouldly;
using System;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace DTOMaker.SrcGen.MemBlocks.IntTests;

[Entity(11)]
public interface ISimpleDTO_String : IEntityBase
{
    [Member(1)] String Field1 { get; set; }
    [Member(2)] String? Field2 { get; set; }
}

public class RoundtripBasicTypeTests_String
{
    public async Task<string> Roundtrip_StringAsync(String reqValue, String? optValue)
    {
        var cancellation = TestContext.Current.CancellationToken;
        using var blobStore = new DataFac.Storage.Testing.TestBlobStore();
        var orig = new SimpleDTO_String { Field1 = reqValue, Field2 = optValue };
        await orig.Pack(blobStore, cancellation);
        orig.Field1.ShouldBe(reqValue);
        orig.Field2.ShouldBe(optValue);
        var buffer = orig.Serialize(cancellation);
        var copy = new SimpleDTO_String(buffer);
        copy.ShouldNotBeNull();
        await copy.UnpackAll(blobStore, cancellation);
        copy.ShouldBe(orig);
        copy.Field1.ShouldBe(reqValue);
        copy.Field2.ShouldBe(optValue);
        return buffer.ToDisplay();
    }

    [Fact] public async Task Roundtrip_String_Defaults() => await Verifier.Verify(await Roundtrip_StringAsync(string.Empty, null));
    [Fact] public async Task Roundtrip_String_UnitVals() => await Verifier.Verify(await Roundtrip_StringAsync("abc", "def"));

}
