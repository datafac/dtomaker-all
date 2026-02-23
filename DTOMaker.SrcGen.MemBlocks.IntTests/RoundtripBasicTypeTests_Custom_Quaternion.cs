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
public interface ISimpleDTO_Quaternion : IEntityBase { [Member(1, NativeType.QuadOfInt32, typeof(QuaternionConverter))] Quaternion Value { get; } }

public class RoundtripBasicTypeTests_Custom_Quaternion
{
    public async Task<string> Roundtrip_QuaternionAsync(Quaternion reqValue)
    {
        using var dataStore = new DataFac.Storage.Testing.TestDataStore();
        var orig = new SimpleDTO_Quaternion { Value = reqValue };
        await orig.Pack(dataStore);
        orig.Value.ShouldBe(reqValue);
        var buffers = orig.GetBuffers();
        var copy = new SimpleDTO_Quaternion(buffers);
        copy.ShouldNotBeNull();
        copy.ShouldBe(orig);
        copy.Value.ShouldBe(reqValue);
        return buffers.ToDisplay();
    }

    [Fact] public async Task Roundtrip_Quaternion_Defaults() => await Verifier.Verify(Roundtrip_QuaternionAsync(default));
    [Fact] public async Task Roundtrip_Quaternion_Value001() => await Verifier.Verify(Roundtrip_QuaternionAsync(Quaternion.Identity));
    [Fact] public async Task Roundtrip_Quaternion_Value002() => await Verifier.Verify(Roundtrip_QuaternionAsync(new Quaternion(Vector3.UnitX, 2.0F)));
    [Fact] public async Task Roundtrip_Quaternion_Value003() => await Verifier.Verify(Roundtrip_QuaternionAsync(new Quaternion(Vector3.UnitY, 3.0F)));
    [Fact] public async Task Roundtrip_Quaternion_Value004() => await Verifier.Verify(Roundtrip_QuaternionAsync(new Quaternion(Vector3.UnitZ, 4.0F)));
}

