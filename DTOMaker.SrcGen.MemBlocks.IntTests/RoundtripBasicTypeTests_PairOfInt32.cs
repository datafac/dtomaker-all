using DataFac.Memory;
using DTOMaker.Models;
using DTOMaker.SrcGen.MemBlocks.IntTests.MemBlocks;
using Shouldly;
using System;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace DTOMaker.SrcGen.MemBlocks.IntTests;

[Entity(17, LayoutMethod.Linear)]
public interface ISimpleDTO_PairOfInt32 : IEntityBase
{
    [Member(1)] PairOfInt32 Field1 { get; set; }
    // todo [Member(2)] PairOfInt32? Field2 { get; set; }
}

public class RoundtripBasicTypeTests_PairOfInt32
{
    public async Task<string> Roundtrip_PairOfInt32Async(PairOfInt32 reqValue, PairOfInt32? optValue)
    {
        using var dataStore = new DataFac.Storage.Testing.TestDataStore();
        var orig = new SimpleDTO_PairOfInt32 { Field1 = reqValue };
        await orig.Pack(dataStore);
        orig.Field1.ShouldBe(reqValue);
        //orig.Field2.ShouldBe(optValue)
        var buffers = orig.GetBuffers();
        var copy = new SimpleDTO_PairOfInt32(buffers);
        copy.ShouldNotBeNull();
        copy.ShouldBe(orig);
        copy.Field1.ShouldBe(reqValue);
        return buffers.ToDisplay();
    }

    [Fact] public async Task Roundtrip_PairOfInt32_Defaults() => await Verifier.Verify(await Roundtrip_PairOfInt32Async(default, default));
    [Fact]
    public async Task Roundtrip_PairOfInt32_Maximums()
        => await Verifier.Verify(await Roundtrip_PairOfInt32Async(
            new PairOfInt32(Int32.MaxValue, Int32.MaxValue),
            new PairOfInt32(Int32.MinValue, Int32.MinValue)));
}
