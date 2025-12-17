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
[Id(9)]
public interface ISimpleDTO_Bool : IEntityBase
{
    [Member(1)] bool Field1 { get; set; }
    [Member(2)] bool? Field2 { get; set; }
}

public class RoundtripBasicTypeTests_Bool
{
    public string Roundtrip_Bool(bool reqValue, bool? optValue)
    {
        var orig = new SimpleDTO_Bool { Field1 = reqValue, Field2 = optValue };
        orig.Freeze();
        orig.Field1.ShouldBe(reqValue);
        orig.Field2.ShouldBe(optValue);
        var json = orig.SerializeToJson();
        var copy = json.DeserializeFromJson<SimpleDTO_Bool>();
        copy.ShouldNotBeNull();
        copy.ShouldBe(orig);
        copy.Field1.ShouldBe(reqValue);
        return json;
    }

    [Fact] public async Task Roundtrip_Bool_Defaults() => await Verifier.Verify(Roundtrip_Bool(default, default));
    [Fact] public async Task Roundtrip_Bool_MaxValue() => await Verifier.Verify(Roundtrip_Bool(true, true));
    [Fact] public async Task Roundtrip_Bool_MinValue() => await Verifier.Verify(Roundtrip_Bool(false, false));

}