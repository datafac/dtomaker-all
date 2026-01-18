using DataFac.Memory;
using DTOMaker.Models;
using DTOMaker.SrcGen.MemBlocks.IntTests.MemBlocks;
using Shouldly;
using System;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace DTOMaker.SrcGen.MemBlocks.IntTests;

[Entity(16, LayoutMethod.Linear)]
public interface ISimpleDTO_PairOfInt16 : IEntityBase
{
    [Member(1)] PairOfInt16 Field1 { get; set; }
    // todo [Member(2)] PairOfInt16? Field2 { get; set; }
}

public class RoundtripBasicTypeTests_PairOfInt16
{
    public async Task<string> Roundtrip_PairOfInt16Async(PairOfInt16 reqValue, PairOfInt16? optValue)
    {
        using var dataStore = new DataFac.Storage.Testing.TestDataStore();
        var orig = new SimpleDTO_PairOfInt16 { Field1 = reqValue };
        await orig.Pack(dataStore);
        orig.Field1.ShouldBe(reqValue);
        //orig.Field2.ShouldBe(optValue)
        var buffers = orig.GetBuffers();
        var copy = new SimpleDTO_PairOfInt16(buffers);
        copy.ShouldNotBeNull();
        copy.ShouldBe(orig);
        copy.Field1.ShouldBe(reqValue);
        return buffers.ToDisplay();
    }

    [Fact] public async Task Roundtrip_PairOfInt16_Defaults() => await Verifier.Verify(await Roundtrip_PairOfInt16Async(default, default));
    [Fact]
    public async Task Roundtrip_PairOfInt16_Maximums()
        => await Verifier.Verify(await Roundtrip_PairOfInt16Async(
            new PairOfInt16(Int16.MaxValue, Int16.MaxValue),
            new PairOfInt16(Int16.MinValue, Int16.MinValue)));
}
