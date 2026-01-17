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

[Entity(1)]
public interface ISimpleDTO_Int32 : IEntityBase
{
    [Member(1)] Int32 Field1 { get; set; }
    [Member(2)] Int32? Field2 { get; set; }
}

public class RoundtripBasicTypeTests_Int32
{
    public string Roundtrip_Int32(Int32 reqValue, Int32? optValue)
    {
        var orig = new SimpleDTO_Int32 { Field1 = reqValue, Field2 = optValue };
        orig.Freeze();
        orig.Field1.ShouldBe(reqValue);
        orig.Field2.ShouldBe(optValue);
        var json = orig.SerializeToJson();
        var copy = json.DeserializeFromJson<SimpleDTO_Int32>();
        copy.ShouldNotBeNull();
        copy.ShouldBe(orig);
        copy.Field1.ShouldBe(reqValue);
        return json;
    }

    [Fact] public async Task Roundtrip_Int32_Defaults() => await Verifier.Verify(Roundtrip_Int32(default, default));
    [Fact] public async Task Roundtrip_Int32_MaxValue() => await Verifier.Verify(Roundtrip_Int32(Int32.MaxValue, Int32.MaxValue));
    [Fact] public async Task Roundtrip_Int32_MinValue() => await Verifier.Verify(Roundtrip_Int32(Int32.MinValue, Int32.MinValue));
    [Fact] public async Task Roundtrip_Int32_UnitVals() => await Verifier.Verify(Roundtrip_Int32(1, -1));

}

// ---------- custom structs ----------
[Entity(41, LayoutMethod.Linear)]
public interface ISimpleDTO_DayOfWeek : IEntityBase { [Member(1, NativeType.Int32, "DTOMaker.Runtime.Converters.DayOfWeekConverter")] DayOfWeek Value { get; } }

public class RoundtripBasicTypeTests_Custom_DayOfWeek
{
    public string Roundtrip_DayOfWeek(DayOfWeek reqValue)
    {
        var orig = new SimpleDTO_DayOfWeek { Value = reqValue };
        orig.Freeze();
        orig.Value.ShouldBe(reqValue);
        var json = orig.SerializeToJson();
        var copy = json.DeserializeFromJson<SimpleDTO_DayOfWeek>();
        copy.ShouldNotBeNull();
        copy.ShouldBe(orig);
        copy.Value.ShouldBe(reqValue);
        return json;
    }

    [Fact] public async Task Roundtrip_DayOfWeek_Defaults() => await Verifier.Verify(Roundtrip_DayOfWeek(default));
    [Fact] public async Task Roundtrip_DayOfWeek_OneValue() => await Verifier.Verify(Roundtrip_DayOfWeek(DayOfWeek.Monday));
    [Fact] public async Task Roundtrip_DayOfWeek_MaxValue() => await Verifier.Verify(Roundtrip_DayOfWeek(DayOfWeek.Saturday));

}

