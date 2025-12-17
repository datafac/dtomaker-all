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
[Id(21)]
public interface ISimpleDTO_Int128 : IEntityBase
{
    [Member(1)] Int128 Field1 { get; set; }
    [Member(2)] Int128? Field2 { get; set; }
}

public class RoundtripBasicTypeTests_Int128
{
    public string Roundtrip_Int128(Int128 reqValue, Int128? optValue)
    {
        var orig = new SimpleDTO_Int128 { Field1 = reqValue, Field2 = optValue };
        orig.Freeze();
        orig.Field1.ShouldBe(reqValue);
        orig.Field2.ShouldBe(optValue);
        var json = orig.SerializeToJson();
        var copy = json.DeserializeFromJson<SimpleDTO_Int128>();
        copy.ShouldNotBeNull();
        copy.ShouldBe(orig);
        copy.Field1.ShouldBe(reqValue);
        return json;
    }

    [Fact] public async Task Roundtrip_Int128_Defaults() => await Verifier.Verify(Roundtrip_Int128(default, default));
    [Fact] public async Task Roundtrip_Int128_Maximums() => await Verifier.Verify(Roundtrip_Int128(Int128.MaxValue, Int128.MinValue));
    [Fact] public async Task Roundtrip_Int128_UnitVals() => await Verifier.Verify(Roundtrip_Int128(Int128.One, Int128.NegativeOne));

}
#endif
