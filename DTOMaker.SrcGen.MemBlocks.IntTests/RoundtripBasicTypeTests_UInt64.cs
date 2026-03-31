using DTOMaker.Models;
using DTOMaker.SrcGen.MemBlocks.IntTests.MemBlocks;
using Shouldly;
using System;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace DTOMaker.SrcGen.MemBlocks.IntTests;

[Entity(6)]
public interface ISimpleDTO_UInt64 : IEntityBase
{
    [Member(1)] UInt64 Field1 { get; set; }
    [Member(2)] UInt64? Field2 { get; set; }
}

public class RoundtripBasicTypeTests_UInt64
{
    public async Task<string> Roundtrip_UInt64Async(UInt64 reqValue, UInt64? optValue)
    {
        using var dataStore = new DataFac.Storage.Testing.TestDataStore();
        var orig = new SimpleDTO_UInt64 { Field1 = reqValue, Field2 = optValue };
        await orig.Pack(dataStore);
        orig.Field1.ShouldBe(reqValue);
        orig.Field2.ShouldBe(optValue);
        var buffer = orig.GetBuffer();
        var copy = new SimpleDTO_UInt64(buffer);
        copy.ShouldNotBeNull();
        copy.ShouldBe(orig);
        copy.Field1.ShouldBe(reqValue);
        copy.Field2.ShouldBe(optValue);
        return buffer.ToDisplay();
    }

    [Fact] public async Task Roundtrip_UInt64_Defaults() => await Verifier.Verify(await Roundtrip_UInt64Async(default, null));
    [Fact] public async Task Roundtrip_UInt64_MaxValue() => await Verifier.Verify(await Roundtrip_UInt64Async(UInt64.MaxValue, UInt64.MaxValue));
    [Fact] public async Task Roundtrip_UInt64_MinValue() => await Verifier.Verify(await Roundtrip_UInt64Async(UInt64.MinValue, UInt64.MinValue));
    [Fact] public async Task Roundtrip_UInt64_UnitVals() => await Verifier.Verify(await Roundtrip_UInt64Async(1, 1));

}
