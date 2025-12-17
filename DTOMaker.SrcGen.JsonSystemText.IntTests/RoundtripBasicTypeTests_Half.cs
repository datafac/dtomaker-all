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

#if NET7_0_OR_GREATER
[Entity]
[Id(15)]
public interface ISimpleDTO_Half : IEntityBase
{
    [Member(1)] Half Field1 { get; set; }
    [Member(2)] Half? Field2 { get; set; }
}

public class RoundtripBasicTypeTests_Half
{
    public string Roundtrip_Half(Half reqValue, Half? optValue)
    {
        var orig = new SimpleDTO_Half { Field1 = reqValue, Field2 = optValue };
        orig.Freeze();
        orig.Field1.ShouldBe(reqValue);
        orig.Field2.ShouldBe(optValue);
        var json = orig.SerializeToJson();
        var copy = json.DeserializeFromJson<SimpleDTO_Half>();
        copy.ShouldNotBeNull();
        if (Half.IsNaN(reqValue))
        {
            Half.IsNaN(copy.Field1).ShouldBeTrue();
        }
        else
        {
            copy.ShouldBe(orig);
            copy.Field1.ShouldBe(reqValue);
        }
        copy.Field2.ShouldBe(optValue);
        return json;
    }

    [Fact] public async Task Roundtrip_Half_Defaults() => await Verifier.Verify(Roundtrip_Half(default, default));
    [Fact] public async Task Roundtrip_Half_Infinite() => await Verifier.Verify(Roundtrip_Half(Half.PositiveInfinity, Half.NegativeInfinity));
    [Fact] public async Task Roundtrip_Half_UnitVals() => await Verifier.Verify(Roundtrip_Half(Half.One, Half.NegativeOne));
    [Fact] public async Task Roundtrip_Half_Maximums_Net70() => await Verifier.Verify(Roundtrip_Half(Half.MaxValue, Half.MinValue));
    [Fact] public async Task Roundtrip_Half_NaNEpsil_Net70() => await Verifier.Verify(Roundtrip_Half(Half.NaN, Half.Epsilon));

}
#endif
