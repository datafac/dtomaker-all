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
[Id(2)]
public interface ISimpleDTO_Int64 : IEntityBase
{
    [Member(1)] Int64 Field1 { get; set; }
    [Member(2)] Int64? Field2 { get; set; }
}

public class RoundtripBasicTypeTests_Int64
{
    public string Roundtrip_Int64(Int64 reqValue, Int64? optValue)
    {
        var orig = new SimpleDTO_Int64 { Field1 = reqValue, Field2 = optValue };
        orig.Freeze();
        orig.Field1.ShouldBe(reqValue);
        orig.Field2.ShouldBe(optValue);
        var json = orig.SerializeToJson();
        var copy = json.DeserializeFromJson<SimpleDTO_Int64>();
        copy.ShouldNotBeNull();
        copy.ShouldBe(orig);
        copy.Field1.ShouldBe(reqValue);
        return json;
    }

    [Fact] public async Task Roundtrip_Int64_Defaults() => await Verifier.Verify(Roundtrip_Int64(default, default));
    [Fact] public async Task Roundtrip_Int64_MaxValue() => await Verifier.Verify(Roundtrip_Int64(Int64.MaxValue, Int64.MaxValue));
    [Fact] public async Task Roundtrip_Int64_MinValue() => await Verifier.Verify(Roundtrip_Int64(Int64.MinValue, Int64.MinValue));
    [Fact] public async Task Roundtrip_Int64_UnitVals() => await Verifier.Verify(Roundtrip_Int64(1, -1));

}