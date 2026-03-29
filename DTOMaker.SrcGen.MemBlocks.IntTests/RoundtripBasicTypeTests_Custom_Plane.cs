using DTOMaker.Converters.Numerics;
using DTOMaker.Models;
using DTOMaker.SrcGen.MemBlocks.IntTests.MemBlocks;
using Shouldly;
using System;
using System.Numerics;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace DTOMaker.SrcGen.MemBlocks.IntTests;

[Entity(54)]
public interface ISimpleDTO_Plane : IEntityBase
{
    [Member(1, NativeType.QuadOfInt32, typeof(DTOMaker.Converters.Numerics.PlaneConverter))] Plane Field1 { get; }
    [Member(2, NativeType.QuadOfInt32, typeof(DTOMaker.Converters.Numerics.PlaneConverter))] Plane? Field2 { get; }
}

public class RoundtripBasicTypeTests_Custom_Plane
{
    public async Task<string> Roundtrip_PlaneAsync(Plane reqValue, Plane? optValue)
    {
        using var dataStore = new DataFac.Storage.Testing.TestDataStore();
        var orig = new SimpleDTO_Plane { Field1 = reqValue, Field2 = optValue };
        await orig.Pack(dataStore);
        orig.Field1.ShouldBe(reqValue);
        orig.Field2.ShouldBe(optValue);
        var buffers = orig.GetContent();
        var copy = new SimpleDTO_Plane(buffers);
        copy.ShouldNotBeNull();
        copy.ShouldBe(orig);
        copy.Equals(orig).ShouldBeTrue();
        copy.Field1.ShouldBe(reqValue);
        copy.Field2.ShouldBe(optValue);
        return buffers.ToDisplay();
    }

    [Fact] public async Task Roundtrip_Plane_Defaults() => await Verifier.Verify(Roundtrip_PlaneAsync(default, null));
    [Fact] public async Task Roundtrip_Plane_Value001() => await Verifier.Verify(Roundtrip_PlaneAsync(new Plane(Vector4.UnitX), new Plane(Vector4.UnitY)));
    [Fact] public async Task Roundtrip_Plane_Value002() => await Verifier.Verify(Roundtrip_PlaneAsync(new Plane(Vector4.UnitZ), new Plane(Vector4.UnitW)));
}

