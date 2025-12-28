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

[Entity(2, LayoutMethod.Linear)]
public interface ISimpleDTO_Int64 : IEntityBase
{
    [Member(1)] Int64 Field1 { get; set; }
    // todo [Member(2)] Int64? Field2 { get; set; }
}

public class RoundtripBasicTypeTests_Int64
{
    public async Task<string> Roundtrip_Int64Async(Int64 reqValue, Int64? optValue)
    {
        using var dataStore = new DataFac.Storage.Testing.TestDataStore();
        var orig = new SimpleDTO_Int64 { Field1 = reqValue };
        await orig.Pack(dataStore);
        orig.Field1.ShouldBe(reqValue);
        //orig.Field2.ShouldBe(optValue)
        var buffers = orig.GetBuffers();
        var copy = new SimpleDTO_Int64(buffers);
        copy.ShouldNotBeNull();
        copy.ShouldBe(orig);
        copy.Field1.ShouldBe(reqValue);
        return buffers.ToDisplay();
    }

    [Fact] public async Task Roundtrip_Int64_Defaults() => await Verifier.Verify(await Roundtrip_Int64Async(default, default));
    [Fact] public async Task Roundtrip_Int64_MaxValue() => await Verifier.Verify(await Roundtrip_Int64Async(Int64.MaxValue, Int64.MaxValue));
    [Fact] public async Task Roundtrip_Int64_MinValue() => await Verifier.Verify(await Roundtrip_Int64Async(Int64.MinValue, Int64.MinValue));
    [Fact] public async Task Roundtrip_Int64_UnitVals() => await Verifier.Verify(await Roundtrip_Int64Async(1, -1));

}