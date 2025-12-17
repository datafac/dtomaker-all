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
[Id(3)]
public interface ISimpleDTO_Int16 : IEntityBase
{
    [Member(1)] Int16 Field1 { get; set; }
    [Member(2)] Int16? Field2 { get; set; }
}

public class RoundtripBasicTypeTests_Int16
{
    public string Roundtrip_Int16(Int16 reqValue, Int16? optValue)
    {
        var orig = new SimpleDTO_Int16 { Field1 = reqValue, Field2 = optValue };
        orig.Freeze();
        orig.Field1.ShouldBe(reqValue);
        orig.Field2.ShouldBe(optValue);
        var json = orig.SerializeToJson();
        var copy = json.DeserializeFromJson<SimpleDTO_Int16>();
        copy.ShouldNotBeNull();
        copy.ShouldBe(orig);
        copy.Field1.ShouldBe(reqValue);
        return json;
    }

    [Fact] public async Task Roundtrip_Int16_Defaults() => await Verifier.Verify(Roundtrip_Int16(default, default));
    [Fact] public async Task Roundtrip_Int16_MaxValue() => await Verifier.Verify(Roundtrip_Int16(Int16.MaxValue, Int16.MaxValue));
    [Fact] public async Task Roundtrip_Int16_MinValue() => await Verifier.Verify(Roundtrip_Int16(Int16.MinValue, Int16.MinValue));
    [Fact] public async Task Roundtrip_Int16_UnitVals() => await Verifier.Verify(Roundtrip_Int16(1, -1));

}