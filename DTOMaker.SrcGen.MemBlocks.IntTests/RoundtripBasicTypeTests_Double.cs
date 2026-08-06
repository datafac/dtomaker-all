using DTOMaker.Models;
using DTOMaker.SrcGen.MemBlocks.IntTests.MemBlocks;
using Shouldly;
using System;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace DTOMaker.SrcGen.MemBlocks.IntTests;

[Entity(13)]
public interface ISimpleDTO_Double : IEntityBase
{
    [Member(1)] Double Field1 { get; set; }
    [Member(2)] Double? Field2 { get; set; }
}

public class RoundtripBasicTypeTests_Double
{
    public async Task<string> Roundtrip_DoubleAsync(Double reqValue, Double? optValue)
    {
        var cancellation = TestContext.Current.CancellationToken;
        using var blobStore = new DataFac.Storage.Testing.TestBlobStore();
        var orig = new SimpleDTO_Double { Field1 = reqValue, Field2 = optValue };
        await orig.Pack(blobStore, cancellation);
        orig.Field1.ShouldBe(reqValue);
        orig.Field2.ShouldBe(optValue);
        var buffer = orig.Serialize(cancellation);
        var copy = new SimpleDTO_Double(buffer);
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
        copy.Field2.ShouldBe(optValue);
        return buffer.ToDisplay();
    }

    [Fact] public async Task Roundtrip_Double_Defaults() => await Verifier.Verify(await Roundtrip_DoubleAsync(default, null));
    [Fact] public async Task Roundtrip_Double_Maximums() => await Verifier.Verify(await Roundtrip_DoubleAsync(Double.MaxValue, Double.MinValue));
    [Fact] public async Task Roundtrip_Double_Infinite() => await Verifier.Verify(await Roundtrip_DoubleAsync(Double.PositiveInfinity, Double.NegativeInfinity));
    [Fact] public async Task Roundtrip_Double_UnitVals() => await Verifier.Verify(await Roundtrip_DoubleAsync(1, -1));
#if NET7_0_OR_GREATER
    [Fact] public async Task Roundtrip_Double_NaNEpsil_Net70() => await Verifier.Verify(await Roundtrip_DoubleAsync(Double.NaN, Double.Epsilon));
#else
    [Fact] public async Task Roundtrip_Double_NaNEpsil_Net48() => await Verifier.Verify(await Roundtrip_DoubleAsync(Double.NaN, Double.Epsilon));
#endif

}
