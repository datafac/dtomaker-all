using DTOMaker.Converters.Numerics;
using DTOMaker.Models;
using DTOMaker.SrcGen.MemBlocks.IntTests.MemBlocks;
using Shouldly;
using System.Numerics;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace DTOMaker.SrcGen.MemBlocks.IntTests;

[Entity(54, LayoutMethod.Linear)]
public interface ISimpleDTO_Plane : IEntityBase { [Member(1, NativeType.QuadOfInt32, typeof(PlaneConverter))] Plane Value { get; } }

public class RoundtripBasicTypeTests_Custom_Plane
{
    public async Task<string> Roundtrip_PlaneAsync(Plane reqValue)
    {
        using var dataStore = new DataFac.Storage.Testing.TestDataStore();
        var orig = new SimpleDTO_Plane { Value = reqValue };
        await orig.Pack(dataStore);
        orig.Value.ShouldBe(reqValue);
        var buffers = orig.GetBuffers();
        var copy = new SimpleDTO_Plane(buffers);
        copy.ShouldNotBeNull();
        copy.ShouldBe(orig);
        copy.Value.ShouldBe(reqValue);
        return buffers.ToDisplay();
    }

    [Fact] public async Task Roundtrip_Plane_Defaults() => await Verifier.Verify(Roundtrip_PlaneAsync(default));
    [Fact] public async Task Roundtrip_Plane_Value001() => await Verifier.Verify(Roundtrip_PlaneAsync(new Plane(Vector4.UnitX)));
    [Fact] public async Task Roundtrip_Plane_Value002() => await Verifier.Verify(Roundtrip_PlaneAsync(new Plane(Vector4.UnitY)));
    [Fact] public async Task Roundtrip_Plane_Value003() => await Verifier.Verify(Roundtrip_PlaneAsync(new Plane(Vector4.UnitZ)));
    [Fact] public async Task Roundtrip_Plane_Value004() => await Verifier.Verify(Roundtrip_PlaneAsync(new Plane(Vector4.UnitW)));
}

