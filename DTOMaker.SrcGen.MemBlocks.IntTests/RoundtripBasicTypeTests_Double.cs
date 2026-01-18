using DTOMaker.Models;
using DTOMaker.SrcGen.MemBlocks.IntTests.MemBlocks;
using Shouldly;
using System;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace DTOMaker.SrcGen.MemBlocks.IntTests;

[Entity(13, LayoutMethod.Linear)]
public interface ISimpleDTO_Double : IEntityBase
{
    [Member(1)] Double Field1 { get; set; }
    // todo [Member(2)] Double? Field2 { get; set; }
}

public class RoundtripBasicTypeTests_Double
{
    public async Task<string> Roundtrip_DoubleAsync(Double reqValue, Double? optValue)
    {
        using var dataStore = new DataFac.Storage.Testing.TestDataStore();
        var orig = new SimpleDTO_Double { Field1 = reqValue };
        await orig.Pack(dataStore);
        orig.Field1.ShouldBe(reqValue);
        //orig.Field2.ShouldBe(optValue)
        var buffers = orig.GetBuffers();
        var copy = new SimpleDTO_Double(buffers);
        copy.ShouldNotBeNull();
        if (Double.IsNaN(reqValue))
        {
            Double.IsNaN(copy.Field1).ShouldBeTrue();
        }
        else
        {
            copy.ShouldBe(orig);
            copy.Field1.ShouldBe(reqValue);
        }
        //copy.Field2.ShouldBe(optValue)
        return buffers.ToDisplay();
    }

    [Fact] public async Task Roundtrip_Double_Defaults() => await Verifier.Verify(await Roundtrip_DoubleAsync(default, default));
    [Fact] public async Task Roundtrip_Double_Maximums() => await Verifier.Verify(await Roundtrip_DoubleAsync(Double.MaxValue, Double.MinValue));
    [Fact] public async Task Roundtrip_Double_Infinite() => await Verifier.Verify(await Roundtrip_DoubleAsync(Double.PositiveInfinity, Double.NegativeInfinity));
    [Fact] public async Task Roundtrip_Double_UnitVals() => await Verifier.Verify(await Roundtrip_DoubleAsync(1, -1));
#if NET7_0_OR_GREATER
    [Fact] public async Task Roundtrip_Double_NaNEpsil_Net70() => await Verifier.Verify(await Roundtrip_DoubleAsync(Double.NaN, Double.Epsilon));
#else
    [Fact] public async Task Roundtrip_Double_NaNEpsil_Net48() => await Verifier.Verify(await Roundtrip_DoubleAsync(Double.NaN, Double.Epsilon));
#endif

}
