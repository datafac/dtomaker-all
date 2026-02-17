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

[Entity(52, LayoutMethod.Linear)]
public interface ISimpleDTO_Vector3 : IEntityBase { [Member(1, NativeType.QuadOfInt32, typeof(Vector3Converter))] Vector3 Value { get; } }

public class RoundtripBasicTypeTests_Custom_Vector3
{
    public string Roundtrip_Vector3(Vector3 reqValue)
    {
        var orig = new SimpleDTO_Vector3 { Value = reqValue };
        orig.Freeze();
        orig.Value.ShouldBe(reqValue);
        var json = orig.SerializeToJson();
        var copy = json.DeserializeFromJson<SimpleDTO_Vector3>();
        copy.ShouldNotBeNull();
        copy.ShouldBe(orig);
        copy.Value.ShouldBe(reqValue);
        return json;
    }

    [Fact] public async Task Roundtrip_Vector3_Defaults() => await Verifier.Verify(Roundtrip_Vector3(default));
    [Fact] public async Task Roundtrip_Vector3_Value001() => await Verifier.Verify(Roundtrip_Vector3(Vector3.UnitX));
    [Fact] public async Task Roundtrip_Vector3_Value002() => await Verifier.Verify(Roundtrip_Vector3(Vector3.UnitY));
    [Fact] public async Task Roundtrip_Vector3_Value003() => await Verifier.Verify(Roundtrip_Vector3(Vector3.UnitZ));

}

