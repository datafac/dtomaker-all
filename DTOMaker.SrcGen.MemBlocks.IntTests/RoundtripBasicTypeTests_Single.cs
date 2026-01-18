using DTOMaker.Models;
using DTOMaker.SrcGen.MemBlocks.IntTests.MemBlocks;
using Shouldly;
using System;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace DTOMaker.SrcGen.MemBlocks.IntTests;

[Entity(14, LayoutMethod.Linear)]
public interface ISimpleDTO_Single : IEntityBase
{
    [Member(1)] Single Field1 { get; set; }
    // todo [Member(2)] Single? Field2 { get; set; }
}

public class RoundtripBasicTypeTests_Single
{
    public async Task<string> Roundtrip_SingleAsync(Single reqValue, Single? optValue)
    {
        using var dataStore = new DataFac.Storage.Testing.TestDataStore();
        var orig = new SimpleDTO_Single { Field1 = reqValue };
        await orig.Pack(dataStore);
        orig.Field1.ShouldBe(reqValue);
        //orig.Field2.ShouldBe(optValue)
        var buffers = orig.GetBuffers();
        var copy = new SimpleDTO_Single(buffers);
        copy.ShouldNotBeNull();
        if (Single.IsNaN(reqValue))
        {
            Single.IsNaN(copy.Field1).ShouldBeTrue();
        }
        else
        {
            copy.ShouldBe(orig);
            copy.Field1.ShouldBe(reqValue);
        }
        //copy.Field2.ShouldBe(optValue)
        return buffers.ToDisplay();
    }

    [Fact] public async Task Roundtrip_Single_Defaults() => await Verifier.Verify(await Roundtrip_SingleAsync(default, default));
    [Fact] public async Task Roundtrip_Single_Infinite() => await Verifier.Verify(await Roundtrip_SingleAsync(Single.PositiveInfinity, Single.NegativeInfinity));
    [Fact] public async Task Roundtrip_Single_UnitVals() => await Verifier.Verify(await Roundtrip_SingleAsync(1, -1));
#if NET7_0_OR_GREATER
    [Fact] public async Task Roundtrip_Single_Maximums_Net70() => await Verifier.Verify(await Roundtrip_SingleAsync(Single.MaxValue, Single.MinValue));
    [Fact] public async Task Roundtrip_Single_NaNEpsil_Net70() => await Verifier.Verify(await Roundtrip_SingleAsync(Single.NaN, Single.Epsilon));
#else
    [Fact] public async Task Roundtrip_Single_Maximums_Net48() => await Verifier.Verify(await Roundtrip_SingleAsync(Single.MaxValue, Single.MinValue));
    [Fact] public async Task Roundtrip_Single_NaNEpsil_Net48() => await Verifier.Verify(await Roundtrip_SingleAsync(Single.NaN, Single.Epsilon));
#endif

}
