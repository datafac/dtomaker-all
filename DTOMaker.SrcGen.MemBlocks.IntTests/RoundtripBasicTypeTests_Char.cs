using DTOMaker.Models;
using DTOMaker.SrcGen.MemBlocks.IntTests.MemBlocks;
using Shouldly;
using System;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace DTOMaker.SrcGen.MemBlocks.IntTests;

[Entity(10)]
public interface ISimpleDTO_Char : IEntityBase
{
    [Member(1)] Char Field1 { get; set; }
    [Member(2)] Char? Field2 { get; set; }
}

public class RoundtripBasicTypeTests_Char
{
    public async Task<string> Roundtrip_CharAsync(Char reqValue, Char? optValue)
    {
        using var dataStore = new DataFac.Storage.Testing.TestDataStore();
        var orig = new SimpleDTO_Char { Field1 = reqValue, Field2 = optValue };
        await orig.Pack(dataStore);
        orig.Field1.ShouldBe(reqValue);
        orig.Field2.ShouldBe(optValue);
        var buffers = orig.GetContent();
        var copy = new SimpleDTO_Char(buffers);
        copy.ShouldNotBeNull();
        copy.ShouldBe(orig);
        copy.Equals(orig).ShouldBeTrue();
        copy.Field1.ShouldBe(reqValue);
        copy.Field2.ShouldBe(optValue);
        return buffers.ToDisplay();
    }

    [Fact] public async Task Roundtrip_Char_Defaults() => await Verifier.Verify(await Roundtrip_CharAsync(default, null));
    [Fact] public async Task Roundtrip_Char_Value001() => await Verifier.Verify(await Roundtrip_CharAsync(Char.MinValue, Char.MaxValue));
    [Fact] public async Task Roundtrip_Char_Value002() => await Verifier.Verify(await Roundtrip_CharAsync('A', 'z'));

}
