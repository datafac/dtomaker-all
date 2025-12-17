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
[Id(13)]
public interface ISimpleDTO_Double : IEntityBase
{
    [Member(1)] Double Field1 { get; set; }
    [Member(2)] Double? Field2 { get; set; }
}

public class RoundtripBasicTypeTests_Double
{
    public string Roundtrip_Double(Double reqValue, Double? optValue)
    {
        var orig = new SimpleDTO_Double { Field1 = reqValue, Field2 = optValue };
        orig.Freeze();
        orig.Field1.ShouldBe(reqValue);
        orig.Field2.ShouldBe(optValue);
        var json = orig.SerializeToJson();
        var copy = json.DeserializeFromJson<SimpleDTO_Double>();
        copy.ShouldNotBeNull();
        if (Double.IsNaN(reqValue))
        {
            Double.IsNaN(copy.Field1).ShouldBeTrue();
        }
        else
        {
            copy.ShouldBe(orig);
            copy.Field1.ShouldBe(reqValue);
        }
        copy.Field2.ShouldBe(optValue);
        return json;
    }

    [Fact] public async Task Roundtrip_Double_Defaults() => await Verifier.Verify(Roundtrip_Double(default, default));
    [Fact] public async Task Roundtrip_Double_Maximums() => await Verifier.Verify(Roundtrip_Double(Double.MaxValue, Double.MinValue));
    [Fact] public async Task Roundtrip_Double_Infinite() => await Verifier.Verify(Roundtrip_Double(Double.PositiveInfinity, Double.NegativeInfinity));
    [Fact] public async Task Roundtrip_Double_UnitVals() => await Verifier.Verify(Roundtrip_Double(1, -1));
#if NET7_0_OR_GREATER
    [Fact] public async Task Roundtrip_Double_NaNEpsil_Net70() => await Verifier.Verify(Roundtrip_Double(Double.NaN, Double.Epsilon));
#else
    [Fact] public async Task Roundtrip_Double_NaNEpsil_Net48() => await Verifier.Verify(Roundtrip_Double(Double.NaN, Double.Epsilon));
#endif

}
