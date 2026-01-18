using DTOMaker.Models;
using DTOMaker.SrcGen.MemBlocks.IntTests.MemBlocks;
using Shouldly;
using System;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace DTOMaker.SrcGen.MemBlocks.IntTests;

#if NET7_0_OR_GREATER
[Entity(21, LayoutMethod.Linear)]
public interface ISimpleDTO_Int128 : IEntityBase
{
    [Member(1)] Int128 Field1 { get; set; }
    // todo [Member(2)] Int128? Field2 { get; set; }
}

public class RoundtripBasicTypeTests_Int128
{
    public async Task<string> Roundtrip_Int128Async(Int128 reqValue, Int128? optValue)
    {
        using var dataStore = new DataFac.Storage.Testing.TestDataStore();
        var orig = new SimpleDTO_Int128 { Field1 = reqValue };
        await orig.Pack(dataStore);
        orig.Field1.ShouldBe(reqValue);
        //orig.Field2.ShouldBe(optValue)
        var buffers = orig.GetBuffers();
        var copy = new SimpleDTO_Int128(buffers);
        copy.ShouldNotBeNull();
        copy.ShouldBe(orig);
        copy.Field1.ShouldBe(reqValue);
        return buffers.ToDisplay();
    }

    [Fact] public async Task Roundtrip_Int128_Defaults() => await Verifier.Verify(await Roundtrip_Int128Async(default, default));
    [Fact] public async Task Roundtrip_Int128_Maximums() => await Verifier.Verify(await Roundtrip_Int128Async(Int128.MaxValue, Int128.MinValue));
    [Fact] public async Task Roundtrip_Int128_UnitVals() => await Verifier.Verify(await Roundtrip_Int128Async(Int128.One, null));
    [Fact] public async Task Roundtrip_Int128_ZeroVals() => await Verifier.Verify(await Roundtrip_Int128Async(Int128.Zero, Int128.Zero));

}
#endif
