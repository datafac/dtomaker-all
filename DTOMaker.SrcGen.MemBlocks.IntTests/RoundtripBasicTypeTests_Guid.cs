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

[Entity(20, LayoutMethod.Linear)]
public interface ISimpleDTO_Guid : IEntityBase
{
    [Member(1)] Guid Field1 { get; set; }
    // todo [Member(2)] Guid? Field2 { get; set; }
}

public class RoundtripBasicTypeTests_Guid
{
    public async Task<string> Roundtrip_GuidAsync(Guid reqValue, Guid? optValue)
    {
        using var dataStore = new DataFac.Storage.Testing.TestDataStore();
        var orig = new SimpleDTO_Guid { Field1 = reqValue };
        await orig.Pack(dataStore);
        orig.Field1.ShouldBe(reqValue);
        //orig.Field2.ShouldBe(optValue)
        var buffers = orig.GetBuffers();
        var copy = new SimpleDTO_Guid(buffers);
        copy.ShouldNotBeNull();
        copy.ShouldBe(orig);
        copy.Field1.ShouldBe(reqValue);
        return buffers.ToDisplay();
    }

    private static readonly Guid guidValue = Guid.Parse("12345678-1234-1234-1234-1234567890ab");
    [Fact] public async Task Roundtrip_Guid_Defaults() => await Verifier.Verify(await Roundtrip_GuidAsync(default, default));
    [Fact] public async Task Roundtrip_Guid_UnitVals() => await Verifier.Verify(await Roundtrip_GuidAsync(Guid.Empty, guidValue));

}
