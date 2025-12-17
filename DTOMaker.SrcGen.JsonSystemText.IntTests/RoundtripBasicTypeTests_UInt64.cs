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
[Id(6)]
public interface ISimpleDTO_UInt64 : IEntityBase
{
    [Member(1)] UInt64 Field1 { get; set; }
    [Member(2)] UInt64? Field2 { get; set; }
}

public class RoundtripBasicTypeTests_UInt64
{
    public string Roundtrip_UInt64(UInt64 reqValue, UInt64? optValue)
    {
        var orig = new SimpleDTO_UInt64 { Field1 = reqValue, Field2 = optValue };
        orig.Freeze();
        orig.Field1.ShouldBe(reqValue);
        orig.Field2.ShouldBe(optValue);
        var json = orig.SerializeToJson();
        var copy = json.DeserializeFromJson<SimpleDTO_UInt64>();
        copy.ShouldNotBeNull();
        copy.ShouldBe(orig);
        copy.Field1.ShouldBe(reqValue);
        return json;
    }

    [Fact] public async Task Roundtrip_UInt64_Defaults() => await Verifier.Verify(Roundtrip_UInt64(default, default));
    [Fact] public async Task Roundtrip_UInt64_MaxValue() => await Verifier.Verify(Roundtrip_UInt64(UInt64.MaxValue, UInt64.MaxValue));
    [Fact] public async Task Roundtrip_UInt64_MinValue() => await Verifier.Verify(Roundtrip_UInt64(UInt64.MinValue, UInt64.MinValue));
    [Fact] public async Task Roundtrip_UInt64_UnitVals() => await Verifier.Verify(Roundtrip_UInt64(1, 1));

}
