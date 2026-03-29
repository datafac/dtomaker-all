using DTOMaker.Converters.Numerics;
using DTOMaker.Models;
using DTOMaker.SrcGen.MemBlocks.IntTests.MemBlocks;
using Shouldly;
using System.Numerics;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace DTOMaker.SrcGen.MemBlocks.IntTests;

[Entity(55)]
public interface ISimpleDTO_Quaternion : IEntityBase
{
    [Member(1, NativeType.QuadOfInt32, typeof(DTOMaker.Converters.Numerics.QuaternionConverter))] Quaternion Field1 { get; }
    [Member(2, NativeType.QuadOfInt32, typeof(DTOMaker.Converters.Numerics.QuaternionConverter))] Quaternion? Field2 { get; }
}

public class RoundtripBasicTypeTests_Custom_Quaternion
{
    public async Task<string> Roundtrip_QuaternionAsync(Quaternion reqValue, Quaternion? optValue)
    {
        using var dataStore = new DataFac.Storage.Testing.TestDataStore();
        var orig = new SimpleDTO_Quaternion { Field1 = reqValue, Field2 = optValue };
        await orig.Pack(dataStore);
        orig.Field1.ShouldBe(reqValue);
        orig.Field2.ShouldBe(optValue);
        var buffers = orig.GetContent();
        var copy = new SimpleDTO_Quaternion(buffers);
        copy.ShouldNotBeNull();
        copy.ShouldBe(orig);
        copy.Equals(orig).ShouldBeTrue();
        copy.Field1.ShouldBe(reqValue);
        copy.Field2.ShouldBe(optValue);
        return buffers.ToDisplay();
    }

    [Fact] public async Task Roundtrip_Quaternion_Defaults() => await Verifier.Verify(Roundtrip_QuaternionAsync(default, null));
    [Fact] public async Task Roundtrip_Quaternion_Value001() => await Verifier.Verify(Roundtrip_QuaternionAsync(Quaternion.Identity, new Quaternion(Vector3.UnitX, 2.0F)));
    [Fact] public async Task Roundtrip_Quaternion_Value002() => await Verifier.Verify(Roundtrip_QuaternionAsync(new Quaternion(Vector3.UnitY, 3.0F), new Quaternion(Vector3.UnitZ, 4.0F)));
}

