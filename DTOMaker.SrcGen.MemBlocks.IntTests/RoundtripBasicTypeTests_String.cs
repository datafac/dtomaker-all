using DTOMaker.Models;
using DTOMaker.SrcGen.MemBlocks.IntTests.MemBlocks;
using Shouldly;
using System;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace DTOMaker.SrcGen.MemBlocks.IntTests;

[Entity(11, LayoutMethod.Linear)]
public interface ISimpleDTO_String : IEntityBase
{
    [Member(1)] String Field1 { get; set; }
    [Member(2)] String? Field2 { get; set; }
}

public class RoundtripBasicTypeTests_String
{
    public async Task<string> Roundtrip_StringAsync(String reqValue, String? optValue)
    {
        using var dataStore = new DataFac.Storage.Testing.TestDataStore();
        var orig = new SimpleDTO_String { Field1 = reqValue };
        await orig.Pack(dataStore);
        orig.Field1.ShouldBe(reqValue);
        //orig.Field2.ShouldBe(optValue)
        var buffers = orig.GetBuffers();
        var copy = new SimpleDTO_String(buffers);
        copy.ShouldNotBeNull();
        await copy.UnpackAll(dataStore);
        copy.ShouldBe(orig);
        copy.Field1.ShouldBe(reqValue);
        return buffers.ToDisplay();
    }

    [Fact] public async Task Roundtrip_String_Defaults() => await Verifier.Verify(await Roundtrip_StringAsync(string.Empty, null));
    [Fact] public async Task Roundtrip_String_UnitVals() => await Verifier.Verify(await Roundtrip_StringAsync("abc", "def"));

}
