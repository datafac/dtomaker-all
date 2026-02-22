using DataFac.Storage;
using DTOMaker.Converters.Numerics;
using DTOMaker.Models;
using DTOMaker.SrcGen.MemBlocks.IntTests.MemBlocks;
using Shouldly;
using System.Numerics;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace DTOMaker.SrcGen.MemBlocks.IntTests;

[Entity(51, LayoutMethod.Compact)]
public interface ISimpleDTO_Vector2 : IEntityBase { [Member(1, NativeType.PairOfInt32, typeof(Vector2Converter))] Vector2 Value { get; } }

public class RoundtripBasicTypeTests_Custom_Vector2
{
    public async Task<string> Roundtrip_Vector2Async(Vector2 reqValue)
    {
        using var dataStore = new DataFac.Storage.Testing.TestDataStore();
        var orig = new SimpleDTO_Vector2 { Value = reqValue };
        await orig.Pack(dataStore);
        orig.Value.ShouldBe(reqValue);
        var buffers = orig.GetBuffers();
        var copy = new SimpleDTO_Vector2(buffers);
        copy.ShouldNotBeNull();
        copy.ShouldBe(orig);
        copy.Value.ShouldBe(reqValue);
        return buffers.ToDisplay();
    }

    [Fact] public async Task Roundtrip_Vector2_Defaults() => await Verifier.Verify(Roundtrip_Vector2Async(default));
    [Fact] public async Task Roundtrip_Vector2_OneValue() => await Verifier.Verify(Roundtrip_Vector2Async(Vector2.UnitX));
    [Fact] public async Task Roundtrip_Vector2_OthValue() => await Verifier.Verify(Roundtrip_Vector2Async(Vector2.UnitY));

}
