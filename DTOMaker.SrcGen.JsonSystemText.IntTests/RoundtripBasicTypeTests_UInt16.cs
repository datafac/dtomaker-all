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
[Id(7)]
public interface ISimpleDTO_UInt16 : IEntityBase
{
    [Member(1)] UInt16 Field1 { get; set; }
    [Member(2)] UInt16? Field2 { get; set; }
}

public class RoundtripBasicTypeTests_UInt16
{
    public string Roundtrip_UInt16(UInt16 reqValue, UInt16? optValue)
    {
        var orig = new SimpleDTO_UInt16 { Field1 = reqValue, Field2 = optValue };
        orig.Freeze();
        orig.Field1.ShouldBe(reqValue);
        orig.Field2.ShouldBe(optValue);
        var json = orig.SerializeToJson();
        var copy = json.DeserializeFromJson<SimpleDTO_UInt16>();
        copy.ShouldNotBeNull();
        copy.ShouldBe(orig);
        copy.Field1.ShouldBe(reqValue);
        return json;
    }

    [Fact] public async Task Roundtrip_UInt16_Defaults() => await Verifier.Verify(Roundtrip_UInt16(default, default));
    [Fact] public async Task Roundtrip_UInt16_MaxValue() => await Verifier.Verify(Roundtrip_UInt16(UInt16.MaxValue, UInt16.MaxValue));
    [Fact] public async Task Roundtrip_UInt16_MinValue() => await Verifier.Verify(Roundtrip_UInt16(UInt16.MinValue, UInt16.MinValue));
    [Fact] public async Task Roundtrip_UInt16_UnitVals() => await Verifier.Verify(Roundtrip_UInt16(1, 1));

}
