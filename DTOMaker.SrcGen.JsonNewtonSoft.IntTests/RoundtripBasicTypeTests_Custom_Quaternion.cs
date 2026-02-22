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

[Entity(55, LayoutMethod.Compact)]
public interface ISimpleDTO_Quaternion : IEntityBase { [Member(1, NativeType.QuadOfInt32, typeof(QuaternionConverter))] Quaternion Value { get; } }

public class RoundtripBasicTypeTests_Custom_Quaternion
{
    public string Roundtrip_Quaternion(Quaternion reqValue)
    {
        var orig = new SimpleDTO_Quaternion { Value = reqValue };
        orig.Freeze();
        orig.Value.ShouldBe(reqValue);
        var json = orig.SerializeToJson();
        var copy = json.DeserializeFromJson<SimpleDTO_Quaternion>();
        copy.ShouldNotBeNull();
        copy.ShouldBe(orig);
        copy.Value.ShouldBe(reqValue);
        return json;
    }

    [Fact] public async Task Roundtrip_Quaternion_Defaults() => await Verifier.Verify(Roundtrip_Quaternion(default));
    [Fact] public async Task Roundtrip_Quaternion_Value001() => await Verifier.Verify(Roundtrip_Quaternion(Quaternion.Identity));
    [Fact] public async Task Roundtrip_Quaternion_Value002() => await Verifier.Verify(Roundtrip_Quaternion(new Quaternion(Vector3.UnitX, 2.0F)));
    [Fact] public async Task Roundtrip_Quaternion_Value003() => await Verifier.Verify(Roundtrip_Quaternion(new Quaternion(Vector3.UnitY, 3.0F)));
    [Fact] public async Task Roundtrip_Quaternion_Value004() => await Verifier.Verify(Roundtrip_Quaternion(new Quaternion(Vector3.UnitZ, 4.0F)));

}

