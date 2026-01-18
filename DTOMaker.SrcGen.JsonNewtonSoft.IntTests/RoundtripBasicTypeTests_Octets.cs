using DataFac.Memory;
using DTOMaker.Models;
using DTOMaker.Runtime.JsonNewtonSoft;
using DTOMaker.SrcGen.JsonNewtonSoft.IntTests.JsonNewtonSoft;
using Shouldly;
using System;
using System.Text;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace DTOMaker.SrcGen.JsonNewtonSoft.IntTests;

[Entity(12)]
public interface ISimpleDTO_Octets : IEntityBase
{
    [Member(1)] Octets Field1 { get; set; }
    [Member(2)] Octets? Field2 { get; set; }
}

public class RoundtripBasicTypeTests_Octets
{
    public string Roundtrip_Octets(Octets reqValue, Octets? optValue)
    {
        var orig = new SimpleDTO_Octets { Field1 = reqValue, Field2 = optValue };
        orig.Freeze();
        orig.Field1.ShouldBe(reqValue);
        orig.Field2.ShouldBe(optValue);
        var json = orig.SerializeToJson();
        var copy = json.DeserializeFromJson<SimpleDTO_Octets>();
        copy.ShouldNotBeNull();
        copy.ShouldBe(orig);
        copy.Field1.ShouldBe(reqValue);
        return json;
    }

    [Fact] 
    public async Task Roundtrip_Octets_Defaults() => await Verifier.Verify(Roundtrip_Octets(Octets.Empty, null));

    [Fact]
    public async Task Roundtrip_Octets_UnitVals() => await Verifier.Verify(
        Roundtrip_Octets(
            new Octets(Encoding.UTF8.GetBytes("abc")),
            new Octets(Encoding.UTF8.GetBytes("def"))));

}
