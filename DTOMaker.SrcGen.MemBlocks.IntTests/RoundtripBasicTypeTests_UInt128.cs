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

#if NET7_0_OR_GREATER
[Entity(22, LayoutMethod.Linear)]
public interface ISimpleDTO_UInt128 : IEntityBase
{
    [Member(1)] UInt128 Field1 { get; set; }
    // todo [Member(2)] UInt128? Field2 { get; set; }
}

public class RoundtripBasicTypeTests_UInt128
{
    public async Task<string> Roundtrip_UInt128Async(UInt128 reqValue, UInt128? optValue)
    {
        using var dataStore = new DataFac.Storage.Testing.TestDataStore();
        var orig = new SimpleDTO_UInt128 { Field1 = reqValue };
        await orig.Pack(dataStore);
        orig.Field1.ShouldBe(reqValue);
        //orig.Field2.ShouldBe(optValue)
        var buffers = orig.GetBuffers();
        var copy = new SimpleDTO_UInt128(buffers);
        copy.ShouldNotBeNull();
        copy.ShouldBe(orig);
        copy.Field1.ShouldBe(reqValue);
        //copy.Field2.ShouldBe(optValue)
        return buffers.ToDisplay();
    }

    [Fact] public async Task Roundtrip_UInt128_Defaults() => await Verifier.Verify(await Roundtrip_UInt128Async(default, default));
    [Fact] public async Task Roundtrip_UInt128_Maximums() => await Verifier.Verify(await Roundtrip_UInt128Async(UInt128.MaxValue, UInt128.MinValue));
    [Fact] public async Task Roundtrip_UInt128_UnitVals() => await Verifier.Verify(await Roundtrip_UInt128Async(UInt128.One, null));
    [Fact] public async Task Roundtrip_UInt128_ZeroVals() => await Verifier.Verify(await Roundtrip_UInt128Async(UInt128.Zero, UInt128.Zero));

}
#endif
