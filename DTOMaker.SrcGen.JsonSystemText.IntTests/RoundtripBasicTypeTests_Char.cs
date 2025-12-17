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
[Id(10)]
public interface ISimpleDTO_Char : IEntityBase
{
    [Member(1)] Char Field1 { get; set; }
    [Member(2)] Char? Field2 { get; set; }
}

public class RoundtripBasicTypeTests_Char
{
    public string Roundtrip_Char(Char reqValue, Char? optValue)
    {
        var orig = new SimpleDTO_Char { Field1 = reqValue, Field2 = optValue };
        orig.Freeze();
        orig.Field1.ShouldBe(reqValue);
        orig.Field2.ShouldBe(optValue);
        var json = orig.SerializeToJson();
        var copy = json.DeserializeFromJson<SimpleDTO_Char>();
        copy.ShouldNotBeNull();
        copy.ShouldBe(orig);
        copy.Field1.ShouldBe(reqValue);
        return json;
    }

    [Fact] public async Task Roundtrip_Char_Defaults() => await Verifier.Verify(Roundtrip_Char(default, default));
    [Fact] public async Task Roundtrip_Char_MaxValue() => await Verifier.Verify(Roundtrip_Char(Char.MaxValue, Char.MaxValue));
    [Fact] public async Task Roundtrip_Char_MinValue() => await Verifier.Verify(Roundtrip_Char(Char.MinValue, Char.MinValue));
    [Fact] public async Task Roundtrip_Char_UnitVals() => await Verifier.Verify(Roundtrip_Char('A', 'z'));

}
