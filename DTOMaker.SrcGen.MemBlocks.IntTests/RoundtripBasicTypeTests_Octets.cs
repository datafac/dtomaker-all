using DataFac.Memory;
using DTOMaker.Models;
using DTOMaker.SrcGen.MemBlocks.IntTests.MemBlocks;
using Shouldly;
using System;
using System.Text;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace DTOMaker.SrcGen.MemBlocks.IntTests;

[Entity(12)]
public interface ISimpleDTO_Octets : IEntityBase
{
    [Member(1)] Octets Field1 { get; set; }
    [Member(2)] Octets? Field2 { get; set; }
}

public class RoundtripBasicTypeTests_Octets
{
    public async Task<string> Roundtrip_OctetsAsync(Octets reqValue, Octets? optValue)
    {
        using var dataStore = new DataFac.Storage.Testing.TestDataStore();
        var orig = new SimpleDTO_Octets { Field1 = reqValue, Field2 = optValue };
        await orig.Pack(dataStore);
        orig.Field1.ShouldBe(reqValue);
        orig.Field2.ShouldBe(optValue);
        var buffers = orig.GetBuffers();
        var copy = new SimpleDTO_Octets(buffers);
        copy.ShouldNotBeNull();
        await copy.UnpackAll(dataStore);
        copy.ShouldBe(orig);
        copy.Field1.ShouldBe(reqValue);
        copy.Field2.ShouldBe(optValue);
        return buffers.ToDisplay();
    }

    [Fact] public async Task Roundtrip_Octets_Defaults() => await Verifier.Verify(await Roundtrip_OctetsAsync(Octets.Empty, null));
    [Fact] public async Task Roundtrip_Octets_UnitVals() => await Verifier.Verify(await Roundtrip_OctetsAsync(new Octets(Encoding.UTF8.GetBytes("abc")), new Octets(Encoding.UTF8.GetBytes("def"))));

}
