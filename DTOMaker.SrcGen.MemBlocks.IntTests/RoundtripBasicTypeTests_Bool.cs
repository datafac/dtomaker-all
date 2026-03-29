using DTOMaker.Models;
using DTOMaker.SrcGen.MemBlocks.IntTests.MemBlocks;
using Shouldly;
using System;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace DTOMaker.SrcGen.MemBlocks.IntTests;

[Entity(9)]
public interface ISimpleDTO_Bool : IEntityBase
{
    [Member(1)] bool Field1 { get; set; }
    [Member(2)] bool? Field2 { get; set; }
}

public class RoundtripBasicTypeTests_Bool
{

    public async Task<string> Roundtrip_BoolAsync(bool reqValue, bool? optValue)
    {
        using var dataStore = new DataFac.Storage.Testing.TestDataStore();
		var orig = new SimpleDTO_Bool { Field1 = reqValue, Field2 = optValue };
        await orig.Pack(dataStore);
        orig.Field1.ShouldBe(reqValue);
        orig.Field2.ShouldBe(optValue);
        var buffers = orig.GetContent();
        var copy = new SimpleDTO_Bool(buffers);
        copy.ShouldNotBeNull();
        copy.ShouldBe(orig);
        copy.Equals(orig).ShouldBeTrue();
        copy.Field1.ShouldBe(reqValue);
        copy.Field2.ShouldBe(optValue);
        return buffers.ToDisplay();
    }

    [Fact] public async Task Roundtrip_Bool_Defaults() => await Verifier.Verify(await Roundtrip_BoolAsync(default, null));
    [Fact] public async Task Roundtrip_Bool_MaxValue() => await Verifier.Verify(await Roundtrip_BoolAsync(true, true));
    [Fact] public async Task Roundtrip_Bool_MinValue() => await Verifier.Verify(await Roundtrip_BoolAsync(false, false));

}
