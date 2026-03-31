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

[Entity(51)]
public interface ISimpleDTO_Vector2 : IEntityBase
{
    [Member(1, NativeType.PairOfInt32, typeof(Vector2Converter))] Vector2 Field1 { get; }
    [Member(2, NativeType.PairOfInt32, typeof(Vector2Converter))] Vector2? Field2 { get; }
}

public class RoundtripBasicTypeTests_Custom_Vector2
{
    public async Task<string> Roundtrip_Vector2Async(Vector2 reqValue, Vector2? optValue)
    {
        using var dataStore = new DataFac.Storage.Testing.TestDataStore();
        var orig = new SimpleDTO_Vector2 { Field1 = reqValue, Field2 = optValue };
        await orig.Pack(dataStore);
        orig.Field1.ShouldBe(reqValue);
        orig.Field2.ShouldBe(optValue);
        var buffer = orig.GetBuffer();
        var copy = new SimpleDTO_Vector2(buffer);
        copy.ShouldNotBeNull();
        copy.ShouldBe(orig);
        copy.Field1.ShouldBe(reqValue);
        copy.Field2.ShouldBe(optValue);
        return buffer.ToDisplay();
    }

    [Fact] public async Task Roundtrip_Vector2_Defaults() => await Verifier.Verify(Roundtrip_Vector2Async(default, null));
    [Fact] public async Task Roundtrip_Vector2_Value001() => await Verifier.Verify(Roundtrip_Vector2Async(Vector2.UnitX, Vector2.Zero));
    [Fact] public async Task Roundtrip_Vector2_Value002() => await Verifier.Verify(Roundtrip_Vector2Async(Vector2.UnitY, Vector2.One));

}
