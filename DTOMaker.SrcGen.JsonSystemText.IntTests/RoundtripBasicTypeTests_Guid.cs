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
[Id(20)]
public interface ISimpleDTO_Guid : IEntityBase
{
    [Member(1)] Guid Field1 { get; set; }
    [Member(2)] Guid? Field2 { get; set; }
}

public class RoundtripBasicTypeTests_Guid
{
    public string Roundtrip_Guid(Guid reqValue, Guid? optValue)
    {
        var orig = new SimpleDTO_Guid { Field1 = reqValue, Field2 = optValue };
        orig.Freeze();
        orig.Field1.ShouldBe(reqValue);
        orig.Field2.ShouldBe(optValue);
        var json = orig.SerializeToJson();
        var copy = json.DeserializeFromJson<SimpleDTO_Guid>();
        copy.ShouldNotBeNull();
        copy.ShouldBe(orig);
        copy.Field1.ShouldBe(reqValue);
        return json;
    }

    private static readonly Guid guidValue = Guid.Parse("12345678-1234-1234-1234-1234567890ab");
    [Fact] public async Task Roundtrip_Guid_Defaults() => await Verifier.Verify(Roundtrip_Guid(default, default));
    [Fact] public async Task Roundtrip_Guid_UnitVals() => await Verifier.Verify(Roundtrip_Guid(Guid.Empty, guidValue));

}
