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

[Entity(53, LayoutMethod.Linear)]
public interface ISimpleDTO_Vector4 : IEntityBase { [Member(1, NativeType.QuadOfInt32, typeof(Vector4Converter))] Vector4 Value { get; } }

public class RoundtripBasicTypeTests_Custom_Vector4
{
    public string Roundtrip_Vector4(Vector4 reqValue)
    {
        var orig = new SimpleDTO_Vector4 { Value = reqValue };
        orig.Freeze();
        orig.Value.ShouldBe(reqValue);
        var json = orig.SerializeToJson();
        var copy = json.DeserializeFromJson<SimpleDTO_Vector4>();
        copy.ShouldNotBeNull();
        copy.ShouldBe(orig);
        copy.Value.ShouldBe(reqValue);
        return json;
    }

    [Fact] public async Task Roundtrip_Vector4_Defaults() => await Verifier.Verify(Roundtrip_Vector4(default));
    [Fact] public async Task Roundtrip_Vector4_Value001() => await Verifier.Verify(Roundtrip_Vector4(Vector4.UnitX));
    [Fact] public async Task Roundtrip_Vector4_Value002() => await Verifier.Verify(Roundtrip_Vector4(Vector4.UnitY));
    [Fact] public async Task Roundtrip_Vector4_Value003() => await Verifier.Verify(Roundtrip_Vector4(Vector4.UnitZ));
    [Fact] public async Task Roundtrip_Vector4_Value004() => await Verifier.Verify(Roundtrip_Vector4(Vector4.UnitW));

}

