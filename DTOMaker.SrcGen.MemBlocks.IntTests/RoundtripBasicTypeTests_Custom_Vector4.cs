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
public interface ISimpleDTO_Vector4 : IEntityBase { [Member(1, NativeType.QuadOfInt32, typeof(Vector4Converter))] Vector4 Value { get; } }

public class RoundtripBasicTypeTests_Custom_Vector4
{
    public async Task<string> Roundtrip_Vector4Async(Vector4 reqValue)
    {
        using var dataStore = new DataFac.Storage.Testing.TestDataStore();
        var orig = new SimpleDTO_Vector4 { Value = reqValue };
        await orig.Pack(dataStore);
        orig.Value.ShouldBe(reqValue);
        var buffers = orig.GetBuffers();
        var copy = new SimpleDTO_Vector4(buffers);
        copy.ShouldNotBeNull();
        copy.ShouldBe(orig);
        copy.Value.ShouldBe(reqValue);
        return buffers.ToDisplay();
    }

    [Fact] public async Task Roundtrip_Vector4_Defaults() => await Verifier.Verify(Roundtrip_Vector4Async(default));
    [Fact] public async Task Roundtrip_Vector4_Value001() => await Verifier.Verify(Roundtrip_Vector4Async(Vector4.UnitX));
    [Fact] public async Task Roundtrip_Vector4_Value002() => await Verifier.Verify(Roundtrip_Vector4Async(Vector4.UnitY));
    [Fact] public async Task Roundtrip_Vector4_Value003() => await Verifier.Verify(Roundtrip_Vector4Async(Vector4.UnitZ));
    [Fact] public async Task Roundtrip_Vector4_Value004() => await Verifier.Verify(Roundtrip_Vector4Async(Vector4.UnitW));
}

