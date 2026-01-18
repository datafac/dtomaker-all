using DTOMaker.Models;
using DTOMaker.SrcGen.MemBlocks.IntTests.MemBlocks;
using Shouldly;
using System;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace DTOMaker.SrcGen.MemBlocks.IntTests;

#if NET7_0_OR_GREATER
[Entity(15, LayoutMethod.Linear)]
public interface ISimpleDTO_Half : IEntityBase
{
    [Member(1)] Half Field1 { get; set; }
    // todo [Member(2)] Half? Field2 { get; set; }
}

public class RoundtripBasicTypeTests_Half
{
    public async Task<string> Roundtrip_HalfAsync(Half reqValue, Half? optValue)
    {
        using var dataStore = new DataFac.Storage.Testing.TestDataStore();
        var orig = new SimpleDTO_Half { Field1 = reqValue };
        await orig.Pack(dataStore);
        orig.Field1.ShouldBe(reqValue);
        //orig.Field2.ShouldBe(optValue)
        var buffers = orig.GetBuffers();
        var copy = new SimpleDTO_Half(buffers);
        copy.ShouldNotBeNull();
        if (Half.IsNaN(reqValue))
        {
            Half.IsNaN(copy.Field1).ShouldBeTrue();
        }
        else
        {
            copy.ShouldBe(orig);
            copy.Field1.ShouldBe(reqValue);
        }
        //copy.Field2.ShouldBe(optValue)
        return buffers.ToDisplay();
    }

    [Fact] public async Task Roundtrip_Half_Defaults() => await Verifier.Verify(await Roundtrip_HalfAsync(default, default));
    [Fact] public async Task Roundtrip_Half_Infinite() => await Verifier.Verify(await Roundtrip_HalfAsync(Half.PositiveInfinity, Half.NegativeInfinity));
    [Fact] public async Task Roundtrip_Half_UnitVals() => await Verifier.Verify(await Roundtrip_HalfAsync(Half.One, Half.NegativeOne));
    [Fact] public async Task Roundtrip_Half_Maximums_Net70() => await Verifier.Verify(await Roundtrip_HalfAsync(Half.MaxValue, Half.MinValue));
    [Fact] public async Task Roundtrip_Half_NaNEpsil_Net70() => await Verifier.Verify(await Roundtrip_HalfAsync(Half.NaN, Half.Epsilon));

}
#endif
