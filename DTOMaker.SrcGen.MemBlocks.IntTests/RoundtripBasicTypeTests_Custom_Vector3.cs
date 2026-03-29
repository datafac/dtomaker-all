using DTOMaker.Converters.Numerics;
using DTOMaker.Models;
using DTOMaker.SrcGen.MemBlocks.IntTests.MemBlocks;
using Shouldly;
using System.Numerics;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace DTOMaker.SrcGen.MemBlocks.IntTests;

[Entity(52)]
public interface ISimpleDTO_Vector3 : IEntityBase
{
    [Member(1, NativeType.QuadOfInt32, typeof(DTOMaker.Converters.Numerics.Vector3Converter))] Vector3 Field1 { get; }
    [Member(2, NativeType.QuadOfInt32, typeof(DTOMaker.Converters.Numerics.Vector3Converter))] Vector3? Field2 { get; }
}

public class RoundtripBasicTypeTests_Custom_Vector3
{
    public async Task<string> Roundtrip_Vector3Async(Vector3 reqValue, Vector3? optValue)
    {
        using var dataStore = new DataFac.Storage.Testing.TestDataStore();
        var orig = new SimpleDTO_Vector3 { Field1 = reqValue, Field2 = optValue };
        await orig.Pack(dataStore);
        orig.Field1.ShouldBe(reqValue);
        orig.Field2.ShouldBe(optValue);
        var buffers = orig.GetContent();
        var copy = new SimpleDTO_Vector3(buffers);
        copy.ShouldNotBeNull();
        copy.ShouldBe(orig);
        copy.Equals(orig).ShouldBeTrue();
        copy.Field1.ShouldBe(reqValue);
        copy.Field2.ShouldBe(optValue);
        return buffers.ToDisplay();
    }

    [Fact] public async Task Roundtrip_Vector3_Defaults() => await Verifier.Verify(Roundtrip_Vector3Async(default, null));
    [Fact] public async Task Roundtrip_Vector3_Value001() => await Verifier.Verify(Roundtrip_Vector3Async(Vector3.UnitX, Vector3.UnitY));
    [Fact] public async Task Roundtrip_Vector3_Value002() => await Verifier.Verify(Roundtrip_Vector3Async(Vector3.UnitZ, Vector3.Zero));
}
