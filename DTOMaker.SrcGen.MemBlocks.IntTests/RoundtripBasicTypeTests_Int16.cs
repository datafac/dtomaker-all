using DTOMaker.Models;
using DTOMaker.SrcGen.MemBlocks.IntTests.MemBlocks;
using Shouldly;
using System;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace DTOMaker.SrcGen.MemBlocks.IntTests;

[Entity(3, LayoutMethod.Linear)]
public interface ISimpleDTO_Int16 : IEntityBase
{
    [Member(1)] Int16 Field1 { get; set; }
    // todo [Member(2)] Int16? Field2 { get; set; }
}

public class RoundtripBasicTypeTests_Int16
{
    public async Task<string> Roundtrip_Int16Async(Int16 reqValue, Int16? optValue)
    {
        using var dataStore = new DataFac.Storage.Testing.TestDataStore();
        var orig = new SimpleDTO_Int16 { Field1 = reqValue };
        await orig.Pack(dataStore);
        orig.Field1.ShouldBe(reqValue);
        //orig.Field2.ShouldBe(optValue)
        var buffers = orig.GetBuffers();
        var copy = new SimpleDTO_Int16(buffers);
        copy.ShouldNotBeNull();
        copy.ShouldBe(orig);
        copy.Field1.ShouldBe(reqValue);
        return buffers.ToDisplay();
    }

    [Fact] public async Task Roundtrip_Int16_Defaults() => await Verifier.Verify(await Roundtrip_Int16Async(default, default));
    [Fact] public async Task Roundtrip_Int16_MaxValue() => await Verifier.Verify(await Roundtrip_Int16Async(Int16.MaxValue, Int16.MaxValue));
    [Fact] public async Task Roundtrip_Int16_MinValue() => await Verifier.Verify(await Roundtrip_Int16Async(Int16.MinValue, Int16.MinValue));
    [Fact] public async Task Roundtrip_Int16_UnitVals() => await Verifier.Verify(await Roundtrip_Int16Async(1, -1));

}