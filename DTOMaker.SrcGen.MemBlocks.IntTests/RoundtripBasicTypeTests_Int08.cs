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

[Entity(4, LayoutMethod.Linear)]
public interface ISimpleDTO_Int08 : IEntityBase
{
    [Member(1)] SByte Field1 { get; set; }
    // todo [Member(2)] SByte? Field2 { get; set; }
}

public class RoundtripBasicTypeTests_Int08
{
    public async Task<string> Roundtrip_Int08Async(SByte reqValue, SByte? optValue)
    {
        using var dataStore = new DataFac.Storage.Testing.TestDataStore();
        var orig = new SimpleDTO_Int08 { Field1 = reqValue };
        await orig.Pack(dataStore);
        orig.Field1.ShouldBe(reqValue);
        //orig.Field2.ShouldBe(optValue)
        var buffers = orig.GetBuffers();
        var copy = new SimpleDTO_Int08(buffers);
        copy.ShouldNotBeNull();
        copy.ShouldBe(orig);
        copy.Field1.ShouldBe(reqValue);
        return buffers.ToDisplay();
    }

    [Fact] public async Task Roundtrip_Int08_Defaults() => await Verifier.Verify(await Roundtrip_Int08Async(default, default));
    [Fact] public async Task Roundtrip_Int08_MaxValue() => await Verifier.Verify(await Roundtrip_Int08Async(SByte.MaxValue, SByte.MaxValue));
    [Fact] public async Task Roundtrip_Int08_MinValue() => await Verifier.Verify(await Roundtrip_Int08Async(SByte.MinValue, SByte.MinValue));
    [Fact] public async Task Roundtrip_Int08_UnitVals() => await Verifier.Verify(await Roundtrip_Int08Async(1, -1));

}