using DTOMaker.Models;
using DTOMaker.SrcGen.MemBlocks.IntTests.MemBlocks;
using Shouldly;
using System;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace DTOMaker.SrcGen.MemBlocks.IntTests;

[Entity(1)]
public interface ISimpleDTO_Int32 : IEntityBase
{
    [Member(1)] Int32 Field1 { get; set; }
    [Member(2)] Int32? Field2 { get; set; }
}

public class RoundtripBasicTypeTests_Int32
{
    public async Task<string> Roundtrip_Int32Async(Int32 reqValue, Int32? optValue)
    {
        using var dataStore = new DataFac.Storage.Testing.TestDataStore();
        var orig = new SimpleDTO_Int32 { Field1 = reqValue, Field2 = optValue };
        await orig.Pack(dataStore);
        orig.Field1.ShouldBe(reqValue);
        orig.Field2.ShouldBe(optValue);
        var buffer = orig.GetBuffer();
        var copy = new SimpleDTO_Int32(buffer);
        copy.ShouldNotBeNull();
        copy.ShouldBe(orig);
        copy.Field1.ShouldBe(reqValue);
        copy.Field2.ShouldBe(optValue);
        return buffer.ToDisplay();
    }

    [Fact] public async Task Roundtrip_Int32_Defaults() => await Verifier.Verify(await Roundtrip_Int32Async(default, null));
    [Fact] public async Task Roundtrip_Int32_MaxValue() => await Verifier.Verify(await Roundtrip_Int32Async(Int32.MaxValue, Int32.MaxValue));
    [Fact] public async Task Roundtrip_Int32_MinValue() => await Verifier.Verify(await Roundtrip_Int32Async(Int32.MinValue, Int32.MinValue));
    [Fact] public async Task Roundtrip_Int32_UnitVals() => await Verifier.Verify(await Roundtrip_Int32Async(1, -1));

}
