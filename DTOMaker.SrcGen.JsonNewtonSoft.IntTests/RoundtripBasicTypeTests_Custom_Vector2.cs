using DTOMaker.Converters.Numerics;
using DTOMaker.Models;
using DTOMaker.Runtime.JsonNewtonSoft;
using DTOMaker.SrcGen.JsonNewtonSoft.IntTests.JsonNewtonSoft;
using Shouldly;
using System.Numerics;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace DTOMaker.SrcGen.JsonNewtonSoft.IntTests;

[Entity(51, LayoutMethod.Linear)]
public interface ISimpleDTO_Vector2 : IEntityBase { [Member(1, NativeType.PairOfInt32, typeof(Vector2Converter))] Vector2 Value { get; } }

public class RoundtripBasicTypeTests_Custom_Vector2
{
    public string Roundtrip_Vector2(Vector2 reqValue)
    {
        var orig = new SimpleDTO_Vector2 { Value = reqValue };
        orig.Freeze();
        orig.Value.ShouldBe(reqValue);
        var json = orig.SerializeToJson();
        var copy = json.DeserializeFromJson<SimpleDTO_Vector2>();
        copy.ShouldNotBeNull();
        copy.ShouldBe(orig);
        copy.Value.ShouldBe(reqValue);
        return json;
    }

    [Fact] public async Task Roundtrip_Vector2_Defaults() => await Verifier.Verify(Roundtrip_Vector2(default));
    [Fact] public async Task Roundtrip_Vector2_OneValue() => await Verifier.Verify(Roundtrip_Vector2(Vector2.UnitX));
    [Fact] public async Task Roundtrip_Vector2_OthValue() => await Verifier.Verify(Roundtrip_Vector2(Vector2.UnitY));

}

