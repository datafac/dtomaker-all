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
[Id(17)]
public interface ISimpleDTO_PairOfInt32 : IEntityBase
{
    [Member(1)] PairOfInt32 Field1 { get; set; }
    [Member(2)] PairOfInt32? Field2 { get; set; }
}

public class RoundtripBasicTypeTests_PairOfInt32
{
    public string Roundtrip_PairOfInt32(PairOfInt32 reqValue, PairOfInt32? optValue)
    {
        var orig = new SimpleDTO_PairOfInt32 { Field1 = reqValue, Field2 = optValue };
        orig.Freeze();
        orig.Field1.ShouldBe(reqValue);
        orig.Field2.ShouldBe(optValue);
        var json = orig.SerializeToJson();
        var copy = json.DeserializeFromJson<SimpleDTO_PairOfInt32>();
        copy.ShouldNotBeNull();
        copy.ShouldBe(orig);
        copy.Field1.ShouldBe(reqValue);
        return json;
    }

    [Fact] public async Task Roundtrip_PairOfInt32_Defaults() => await Verifier.Verify(Roundtrip_PairOfInt32(default, default));
    [Fact]
    public async Task Roundtrip_PairOfInt32_Maximums()
        => await Verifier.Verify(Roundtrip_PairOfInt32(
            new PairOfInt32(Int32.MaxValue, Int32.MaxValue),
            new PairOfInt32(Int32.MinValue, Int32.MinValue)));
}
