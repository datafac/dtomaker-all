using DTOMaker.Converters.Numerics;
using DTOMaker.Models;
using DTOMaker.Runtime.JsonSystemText;
using DTOMaker.SrcGen.JsonSystemText.IntTests.JsonSystemText;
using Shouldly;
using System.Numerics;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace DTOMaker.SrcGen.JsonSystemText.IntTests;

[Entity(54)]
public interface ISimpleDTO_Plane : IEntityBase { [Member(1, NativeType.QuadOfInt32, typeof(PlaneConverter))] Plane Value { get; } }

public class RoundtripBasicTypeTests_Custom_Plane
{
    public string Roundtrip_Plane(Plane reqValue)
    {
        var orig = new SimpleDTO_Plane { Value = reqValue };
        orig.Freeze();
        orig.Value.ShouldBe(reqValue);
        var json = orig.SerializeToJson();
        var copy = json.DeserializeFromJson<SimpleDTO_Plane>();
        copy.ShouldNotBeNull();
        copy.ShouldBe(orig);
        copy.Value.ShouldBe(reqValue);
        return json;
    }

    [Fact] public async Task Roundtrip_Plane_Defaults() => await Verifier.Verify(Roundtrip_Plane(default));
    [Fact] public async Task Roundtrip_Plane_Value001() => await Verifier.Verify(Roundtrip_Plane(new Plane(Vector4.UnitX)));
    [Fact] public async Task Roundtrip_Plane_Value002() => await Verifier.Verify(Roundtrip_Plane(new Plane(Vector4.UnitY)));
    [Fact] public async Task Roundtrip_Plane_Value003() => await Verifier.Verify(Roundtrip_Plane(new Plane(Vector4.UnitZ)));
    [Fact] public async Task Roundtrip_Plane_Value004() => await Verifier.Verify(Roundtrip_Plane(new Plane(Vector4.UnitW)));

}

