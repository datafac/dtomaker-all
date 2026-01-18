using DataFac.Memory;
using DTOMaker.Models;
using DTOMaker.SrcGen.MemBlocks.IntTests.MemBlocks;
using Shouldly;
using System;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace DTOMaker.SrcGen.MemBlocks.IntTests;

[Entity(18, LayoutMethod.Linear)]
public interface ISimpleDTO_PairOfInt64 : IEntityBase
{
    [Member(1)] PairOfInt64 Field1 { get; set; }
    // todo [Member(2)] PairOfInt64? Field2 { get; set; }
}

public class RoundtripBasicTypeTests_PairOfInt64
{
    public async Task<string> Roundtrip_PairOfInt64Async(PairOfInt64 reqValue, PairOfInt64? optValue)
    {
        using var dataStore = new DataFac.Storage.Testing.TestDataStore();
        var orig = new SimpleDTO_PairOfInt64 { Field1 = reqValue };
        await orig.Pack(dataStore);
        orig.Field1.ShouldBe(reqValue);
        //orig.Field2.ShouldBe(optValue)
        var buffers = orig.GetBuffers();
        var copy = new SimpleDTO_PairOfInt64(buffers);
        copy.ShouldNotBeNull();
        copy.ShouldBe(orig);
        copy.Field1.ShouldBe(reqValue);
        return buffers.ToDisplay();
    }

    [Fact] public async Task Roundtrip_PairOfInt64_Defaults() => await Verifier.Verify(await Roundtrip_PairOfInt64Async(default, default));
    [Fact]
    public async Task Roundtrip_PairOfInt64_Maximums()
        => await Verifier.Verify(await Roundtrip_PairOfInt64Async(
            new PairOfInt64(Int64.MaxValue, Int64.MaxValue),
            new PairOfInt64(Int64.MinValue, Int64.MinValue)));
}
