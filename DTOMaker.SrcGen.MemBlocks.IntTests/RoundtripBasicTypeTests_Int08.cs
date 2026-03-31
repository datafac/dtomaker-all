using DTOMaker.Models;
using DTOMaker.SrcGen.MemBlocks.IntTests.MemBlocks;
using Shouldly;
using System;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace DTOMaker.SrcGen.MemBlocks.IntTests;

[Entity(4)]
public interface ISimpleDTO_Int08 : IEntityBase
{
    [Member(1)] SByte Field1 { get; set; }
    [Member(2)] SByte? Field2 { get; set; }
}

public class RoundtripBasicTypeTests_Int08
{
    public async Task<string> Roundtrip_Int08Async(SByte reqValue, SByte? optValue)
    {
        using var dataStore = new DataFac.Storage.Testing.TestDataStore();
        var orig = new SimpleDTO_Int08 { Field1 = reqValue, Field2 = optValue };
        await orig.Pack(dataStore);
        orig.Field1.ShouldBe(reqValue);
        orig.Field2.ShouldBe(optValue);
        var buffer = orig.GetBuffer();
        var copy = new SimpleDTO_Int08(buffer);
        copy.ShouldNotBeNull();
        copy.ShouldBe(orig);
        copy.Field1.ShouldBe(reqValue);
        copy.Field2.ShouldBe(optValue);
        return buffer.ToDisplay();
    }

    [Fact] public async Task Roundtrip_Int08_Defaults() => await Verifier.Verify(await Roundtrip_Int08Async(default, null));
    [Fact] public async Task Roundtrip_Int08_MaxValue() => await Verifier.Verify(await Roundtrip_Int08Async(SByte.MaxValue, SByte.MaxValue));
    [Fact] public async Task Roundtrip_Int08_MinValue() => await Verifier.Verify(await Roundtrip_Int08Async(SByte.MinValue, SByte.MinValue));
    [Fact] public async Task Roundtrip_Int08_UnitVals() => await Verifier.Verify(await Roundtrip_Int08Async(1, -1));

}