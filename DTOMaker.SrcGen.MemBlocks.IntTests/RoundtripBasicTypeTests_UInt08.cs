using DTOMaker.Models;
using DTOMaker.SrcGen.MemBlocks.IntTests.MemBlocks;
using Shouldly;
using System;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace DTOMaker.SrcGen.MemBlocks.IntTests;

[Entity(8)]
public interface ISimpleDTO_UInt08 : IEntityBase
{
    [Member(1)] Byte Field1 { get; set; }
    [Member(2)] Byte? Field2 { get; set; }
}

public class RoundtripBasicTypeTests_UInt08
{
    public async Task<string> Roundtrip_UInt08Async(Byte reqValue, Byte? optValue)
    {
        using var dataStore = new DataFac.Storage.Testing.TestDataStore();
        var orig = new SimpleDTO_UInt08 { Field1 = reqValue, Field2 = optValue };
        await orig.Pack(dataStore);
        orig.Field1.ShouldBe(reqValue);
        orig.Field2.ShouldBe(optValue);
        var buffer = orig.GetBuffer();
        var copy = new SimpleDTO_UInt08(buffer);
        copy.ShouldNotBeNull();
        copy.ShouldBe(orig);
        copy.Field1.ShouldBe(reqValue);
        copy.Field2.ShouldBe(optValue);
        return buffer.ToDisplay();
    }

    [Fact] public async Task Roundtrip_UInt08_Defaults() => await Verifier.Verify(await Roundtrip_UInt08Async(default, null));
    [Fact] public async Task Roundtrip_UInt08_MaxValue() => await Verifier.Verify(await Roundtrip_UInt08Async(Byte.MaxValue, Byte.MaxValue));
    [Fact] public async Task Roundtrip_UInt08_MinValue() => await Verifier.Verify(await Roundtrip_UInt08Async(Byte.MinValue, Byte.MinValue));
    [Fact] public async Task Roundtrip_UInt08_UnitVals() => await Verifier.Verify(await Roundtrip_UInt08Async(1, 1));

}
