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
[Id(5)]
public interface ISimpleDTO_UInt32 : IEntityBase
{
    [Member(1)] UInt32 Field1 { get; set; }
    [Member(2)] UInt32? Field2 { get; set; }
}

public class RoundtripBasicTypeTests_UInt32
{
    public string Roundtrip_UInt32(UInt32 reqValue, UInt32? optValue)
    {
        var orig = new SimpleDTO_UInt32 { Field1 = reqValue, Field2 = optValue };
        orig.Freeze();
        orig.Field1.ShouldBe(reqValue);
        orig.Field2.ShouldBe(optValue);
        var json = orig.SerializeToJson();
        var copy = json.DeserializeFromJson<SimpleDTO_UInt32>();
        copy.ShouldNotBeNull();
        copy.ShouldBe(orig);
        copy.Field1.ShouldBe(reqValue);
        return json;
    }

    [Fact] public async Task Roundtrip_UInt32_Defaults() => await Verifier.Verify(Roundtrip_UInt32(default, default));
    [Fact] public async Task Roundtrip_UInt32_MaxValue() => await Verifier.Verify(Roundtrip_UInt32(UInt32.MaxValue, UInt32.MaxValue));
    [Fact] public async Task Roundtrip_UInt32_MinValue() => await Verifier.Verify(Roundtrip_UInt32(UInt32.MinValue, UInt32.MinValue));
    [Fact] public async Task Roundtrip_UInt32_UnitVals() => await Verifier.Verify(Roundtrip_UInt32(1, 1));

}
