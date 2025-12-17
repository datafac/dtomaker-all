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
[Id(14)]
public interface ISimpleDTO_Single : IEntityBase
{
    [Member(1)] Single Field1 { get; set; }
    [Member(2)] Single? Field2 { get; set; }
}

public class RoundtripBasicTypeTests_Single
{
    public string Roundtrip_Single(Single reqValue, Single? optValue)
    {
        var orig = new SimpleDTO_Single { Field1 = reqValue, Field2 = optValue };
        orig.Freeze();
        orig.Field1.ShouldBe(reqValue);
        orig.Field2.ShouldBe(optValue);
        var json = orig.SerializeToJson();
        var copy = json.DeserializeFromJson<SimpleDTO_Single>();
        copy.ShouldNotBeNull();
        if (Single.IsNaN(reqValue))
        {
            Single.IsNaN(copy.Field1).ShouldBeTrue();
        }
        else
        {
            copy.ShouldBe(orig);
            copy.Field1.ShouldBe(reqValue);
        }
        copy.Field2.ShouldBe(optValue);
        return json;
    }

    [Fact] public async Task Roundtrip_Single_Defaults() => await Verifier.Verify(Roundtrip_Single(default, default));
    [Fact] public async Task Roundtrip_Single_Infinite() => await Verifier.Verify(Roundtrip_Single(Single.PositiveInfinity, Single.NegativeInfinity));
    [Fact] public async Task Roundtrip_Single_UnitVals() => await Verifier.Verify(Roundtrip_Single(1, -1));
#if NET7_0_OR_GREATER
    [Fact] public async Task Roundtrip_Single_Maximums_Net70() => await Verifier.Verify(Roundtrip_Single(Single.MaxValue, Single.MinValue));
    [Fact] public async Task Roundtrip_Single_NaNEpsil_Net70() => await Verifier.Verify(Roundtrip_Single(Single.NaN, Single.Epsilon));
#else
    [Fact] public async Task Roundtrip_Single_Maximums_Net48() => await Verifier.Verify(Roundtrip_Single(Single.MaxValue, Single.MinValue));
    [Fact] public async Task Roundtrip_Single_NaNEpsil_Net48() => await Verifier.Verify(Roundtrip_Single(Single.NaN, Single.Epsilon));
#endif

}
