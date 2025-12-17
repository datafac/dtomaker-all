using DataFac.Memory;
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
[Id(16)]
public interface ISimpleDTO_PairOfInt16 : IEntityBase
{
    [Member(1)] PairOfInt16 Field1 { get; set; }
    [Member(2)] PairOfInt16? Field2 { get; set; }
}

public class RoundtripBasicTypeTests_PairOfInt16
{
    public string Roundtrip_PairOfInt16(PairOfInt16 reqValue, PairOfInt16? optValue)
    {
        var orig = new SimpleDTO_PairOfInt16 { Field1 = reqValue, Field2 = optValue };
        orig.Freeze();
        orig.Field1.ShouldBe(reqValue);
        orig.Field2.ShouldBe(optValue);
        var json = orig.SerializeToJson();
        var copy = json.DeserializeFromJson<SimpleDTO_PairOfInt16>();
        copy.ShouldNotBeNull();
        copy.ShouldBe(orig);
        copy.Field1.ShouldBe(reqValue);
        return json;
    }

    [Fact] public async Task Roundtrip_PairOfInt16_Defaults() => await Verifier.Verify(Roundtrip_PairOfInt16(default, default));
    [Fact]
    public async Task Roundtrip_PairOfInt16_Maximums()
        => await Verifier.Verify(Roundtrip_PairOfInt16(
            new PairOfInt16(Int16.MaxValue, Int16.MaxValue),
            new PairOfInt16(Int16.MinValue, Int16.MinValue)));
}
