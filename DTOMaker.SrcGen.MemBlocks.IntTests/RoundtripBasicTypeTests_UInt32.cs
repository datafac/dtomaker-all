using DTOMaker.Models;
using DTOMaker.SrcGen.MemBlocks.IntTests.MemBlocks;
using Shouldly;
using System;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace DTOMaker.SrcGen.MemBlocks.IntTests;

[Entity(5, LayoutMethod.Linear)]
public interface ISimpleDTO_UInt32 : IEntityBase
{
    [Member(1)] UInt32 Field1 { get; set; }
    // todo [Member(2)] UInt32? Field2 { get; set; }
}

public class RoundtripBasicTypeTests_UInt32
{
    public async Task<string> Roundtrip_UInt32Async(UInt32 reqValue, UInt32? optValue)
    {
        using var dataStore = new DataFac.Storage.Testing.TestDataStore();
        var orig = new SimpleDTO_UInt32 { Field1 = reqValue };
        await orig.Pack(dataStore);
        orig.Field1.ShouldBe(reqValue);
        //orig.Field2.ShouldBe(optValue)
        var buffers = orig.GetBuffers();
        var copy = new SimpleDTO_UInt32(buffers);
        copy.ShouldNotBeNull();
        copy.ShouldBe(orig);
        copy.Field1.ShouldBe(reqValue);
        return buffers.ToDisplay();
    }

    [Fact] public async Task Roundtrip_UInt32_Defaults() => await Verifier.Verify(await Roundtrip_UInt32Async(default, default));
    [Fact] public async Task Roundtrip_UInt32_MaxValue() => await Verifier.Verify(await Roundtrip_UInt32Async(UInt32.MaxValue, UInt32.MaxValue));
    [Fact] public async Task Roundtrip_UInt32_MinValue() => await Verifier.Verify(await Roundtrip_UInt32Async(UInt32.MinValue, UInt32.MinValue));
    [Fact] public async Task Roundtrip_UInt32_UnitVals() => await Verifier.Verify(await Roundtrip_UInt32Async(1, 1));

}
