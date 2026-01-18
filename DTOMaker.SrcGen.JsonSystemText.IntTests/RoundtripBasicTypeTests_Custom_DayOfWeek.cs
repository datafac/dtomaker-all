using DTOMaker.Models;
using DTOMaker.Runtime.JsonSystemText;
using DTOMaker.SrcGen.JsonSystemText.IntTests.JsonSystemText;
using Shouldly;
using System;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace DTOMaker.SrcGen.JsonSystemText.IntTests;

[Entity(41, LayoutMethod.Linear)]
public interface ISimpleDTO_DayOfWeek : IEntityBase { [Member(1, NativeType.Int32, typeof(DTOMaker.Models.DayOfWeekConverter))] DayOfWeek Value { get; } }

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
