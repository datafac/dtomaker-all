using DTOMaker.Models;
using DTOMaker.Runtime;
using DTOMaker.Runtime.JsonSystemText;
using DTOMaker.SrcGen.JsonSystemText.IntTests.JsonSystemText;
using Shouldly;
using System;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace DTOMaker.SrcGen.JsonSystemText.IntTests;

[Entity]
[Id(8)]
public interface ISimpleDTO_UInt08 : IEntityBase
{
    [Member(1)] Byte Field1 { get; set; }
    [Member(2)] Byte? Field2 { get; set; }
}

public class RoundtripBasicTypeTests_UInt08
{
    public string Roundtrip_UInt08(Byte reqValue, Byte? optValue)
    {
        var orig = new SimpleDTO_UInt08 { Field1 = reqValue, Field2 = optValue };
        orig.Freeze();
        orig.Field1.ShouldBe(reqValue);
        orig.Field2.ShouldBe(optValue);
        var json = orig.SerializeToJson();
        var copy = json.DeserializeFromJson<SimpleDTO_UInt08>();
        copy.ShouldNotBeNull();
        copy.ShouldBe(orig);
        copy.Field1.ShouldBe(reqValue);
        return json;
    }

    [Fact] public async Task Roundtrip_UInt08_Defaults() => await Verifier.Verify(Roundtrip_UInt08(default, default));
    [Fact] public async Task Roundtrip_UInt08_MaxValue() => await Verifier.Verify(Roundtrip_UInt08(Byte.MaxValue, Byte.MaxValue));
    [Fact] public async Task Roundtrip_UInt08_MinValue() => await Verifier.Verify(Roundtrip_UInt08(Byte.MinValue, Byte.MinValue));
    [Fact] public async Task Roundtrip_UInt08_UnitVals() => await Verifier.Verify(Roundtrip_UInt08(1, 1));

}
