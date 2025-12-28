using DTOMaker.Models;
using DTOMaker.Runtime;
using DTOMaker.Runtime.MemBlocks;
using DTOMaker.SrcGen.MemBlocks.IntTests.MemBlocks;
using Shouldly;
using System;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace DTOMaker.SrcGen.MemBlocks.IntTests;

[Entity(7, LayoutMethod.Linear)]
public interface ISimpleDTO_UInt16 : IEntityBase
{
    [Member(1)] UInt16 Field1 { get; set; }
    // todo [Member(2)] UInt16? Field2 { get; set; }
}

public class RoundtripBasicTypeTests_UInt16
{
    public async Task<string> Roundtrip_UInt16Async(UInt16 reqValue, UInt16? optValue)
    {
        using var dataStore = new DataFac.Storage.Testing.TestDataStore();
        var orig = new SimpleDTO_UInt16 { Field1 = reqValue };
        await orig.Pack(dataStore);
        orig.Field1.ShouldBe(reqValue);
        //orig.Field2.ShouldBe(optValue)
        var buffers = orig.GetBuffers();
        var copy = new SimpleDTO_UInt16(buffers);
        copy.ShouldNotBeNull();
        copy.ShouldBe(orig);
        copy.Field1.ShouldBe(reqValue);
        return buffers.ToDisplay();
    }

    [Fact] public async Task Roundtrip_UInt16_Defaults() => await Verifier.Verify(await Roundtrip_UInt16Async(default, default));
    [Fact] public async Task Roundtrip_UInt16_MaxValue() => await Verifier.Verify(await Roundtrip_UInt16Async(UInt16.MaxValue, UInt16.MaxValue));
    [Fact] public async Task Roundtrip_UInt16_MinValue() => await Verifier.Verify(await Roundtrip_UInt16Async(UInt16.MinValue, UInt16.MinValue));
    [Fact] public async Task Roundtrip_UInt16_UnitVals() => await Verifier.Verify(await Roundtrip_UInt16Async(1, 1));

}
