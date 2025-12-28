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

[Entity(10, LayoutMethod.Linear)]
public interface ISimpleDTO_Char : IEntityBase
{
    [Member(1)] Char Field1 { get; set; }
    // todo [Member(2)] Char? Field2 { get; set; }
}

public class RoundtripBasicTypeTests_Char
{
    public async Task<string> Roundtrip_CharAsync(Char reqValue, Char? optValue)
    {
        using var dataStore = new DataFac.Storage.Testing.TestDataStore();
        var orig = new SimpleDTO_Char { Field1 = reqValue };
        await orig.Pack(dataStore);
        orig.Field1.ShouldBe(reqValue);
        //orig.Field2.ShouldBe(optValue)
        var buffers = orig.GetBuffers();
        var copy = new SimpleDTO_Char(buffers);
        copy.ShouldNotBeNull();
        copy.ShouldBe(orig);
        copy.Field1.ShouldBe(reqValue);
        return buffers.ToDisplay();
    }

    [Fact] public async Task Roundtrip_Char_Defaults() => await Verifier.Verify(await Roundtrip_CharAsync(default, default));
    [Fact] public async Task Roundtrip_Char_MaxValue() => await Verifier.Verify(await Roundtrip_CharAsync(Char.MaxValue, Char.MaxValue));
    [Fact] public async Task Roundtrip_Char_MinValue() => await Verifier.Verify(await Roundtrip_CharAsync(Char.MinValue, Char.MinValue));
    [Fact] public async Task Roundtrip_Char_UnitVals() => await Verifier.Verify(await Roundtrip_CharAsync('A', 'z'));

}
