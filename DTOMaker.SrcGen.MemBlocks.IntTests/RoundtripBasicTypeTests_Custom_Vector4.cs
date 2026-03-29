using DTOMaker.Converters.Numerics;
using DTOMaker.Models;
using DTOMaker.SrcGen.MemBlocks.IntTests.MemBlocks;
using Shouldly;
using System.Numerics;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace DTOMaker.SrcGen.MemBlocks.IntTests;

[Entity(53)]
public interface ISimpleDTO_Vector4 : IEntityBase
{
    [Member(1, NativeType.QuadOfInt32, typeof(DTOMaker.Converters.Numerics.Vector4Converter))] Vector4 Field1 { get; }
    [Member(2, NativeType.QuadOfInt32, typeof(DTOMaker.Converters.Numerics.Vector4Converter))] Vector4? Field2 { get; }
}

public class RoundtripBasicTypeTests_Custom_Vector4
{
    public async Task<string> Roundtrip_Vector4Async(Vector4 reqValue, Vector4? optValue)
    {
        using var dataStore = new DataFac.Storage.Testing.TestDataStore();
        var orig = new SimpleDTO_Vector4 { Field1 = reqValue, Field2 = optValue };
        await orig.Pack(dataStore);
        orig.Field1.ShouldBe(reqValue);
        orig.Field2.ShouldBe(optValue);
        var buffers = orig.GetContent();
        var copy = new SimpleDTO_Vector4(buffers);
        copy.ShouldNotBeNull();
        copy.ShouldBe(orig);
        copy.Equals(orig).ShouldBeTrue();
        copy.Field1.ShouldBe(reqValue);
        copy.Field2.ShouldBe(optValue);
        return buffers.ToDisplay();
    }

    [Fact] public async Task Roundtrip_Vector4_Defaults() => await Verifier.Verify(Roundtrip_Vector4Async(default, null));
    [Fact] public async Task Roundtrip_Vector4_Value001() => await Verifier.Verify(Roundtrip_Vector4Async(Vector4.UnitX, Vector4.UnitY));
    [Fact] public async Task Roundtrip_Vector4_Value002() => await Verifier.Verify(Roundtrip_Vector4Async(Vector4.UnitZ, Vector4.UnitW));
}
