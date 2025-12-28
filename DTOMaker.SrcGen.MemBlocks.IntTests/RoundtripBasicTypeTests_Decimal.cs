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

[Entity(19, LayoutMethod.Linear)]
public interface ISimpleDTO_Decimal : IEntityBase
{
    [Member(1)] Decimal Field1 { get; set; }
    // todo [Member(2)] Decimal? Field2 { get; set; }
}

public class RoundtripBasicTypeTests_Decimal
{
    public async Task<string> Roundtrip_DecimalAsync(Decimal reqValue, Decimal? optValue)
    {
        using var dataStore = new DataFac.Storage.Testing.TestDataStore();
        var orig = new SimpleDTO_Decimal { Field1 = reqValue };
        await orig.Pack(dataStore);
        orig.Field1.ShouldBe(reqValue);
        //orig.Field2.ShouldBe(optValue)
        var buffers = orig.GetBuffers();
        var copy = new SimpleDTO_Decimal(buffers);
        copy.ShouldNotBeNull();
        copy.ShouldBe(orig);
        copy.Field1.ShouldBe(reqValue);
        return buffers.ToDisplay();
    }

    [Fact] public async Task Roundtrip_Decimal_Defaults() => await Verifier.Verify(await Roundtrip_DecimalAsync(default, default));
    [Fact] public async Task Roundtrip_Decimal_Maximums() => await Verifier.Verify(await Roundtrip_DecimalAsync(Decimal.MaxValue, Decimal.MinValue));
    [Fact] public async Task Roundtrip_Decimal_UnitVals() => await Verifier.Verify(await Roundtrip_DecimalAsync(Decimal.One, Decimal.MinusOne));

}
