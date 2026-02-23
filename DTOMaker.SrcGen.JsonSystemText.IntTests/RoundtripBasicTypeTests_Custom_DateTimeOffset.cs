using DTOMaker.Models;
using DTOMaker.Runtime.JsonSystemText;
using DTOMaker.SrcGen.JsonSystemText.IntTests.JsonSystemText;
using Shouldly;
using System;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace DTOMaker.SrcGen.JsonSystemText.IntTests;

[Entity(46)]
public interface ISimpleDTO_DateTimeOffset : IEntityBase { [Member(1, NativeType.PairOfInt64, typeof(DTOMaker.Models.DateTimeOffsetConverter))] DateTimeOffset Value { get; } }

public class RoundtripBasicTypeTests_Custom_DateTimeOffset
{
    public string Roundtrip_DateTimeOffset(DateTimeOffset reqValue)
    {
        var orig = new SimpleDTO_DateTimeOffset { Value = reqValue };
        orig.Freeze();
        orig.Value.ShouldBe(reqValue);
        var json = orig.SerializeToJson();
        var copy = json.DeserializeFromJson<SimpleDTO_DateTimeOffset>();
        copy.ShouldNotBeNull();
        copy.ShouldBe(orig);
        copy.Value.ShouldBe(reqValue);
        return json;
    }

    private static readonly DateTime _utcDate = new DateTime(2026, 1, 19, 22, 0, 0, DateTimeKind.Utc);
    [Fact] public async Task Roundtrip_DateTimeOffset_Default() => await Verifier.Verify(Roundtrip_DateTimeOffset(DateTimeOffset.MinValue));
    [Fact] public async Task Roundtrip_DateTimeOffset_UtcDate() => await Verifier.Verify(Roundtrip_DateTimeOffset(new DateTimeOffset(_utcDate, TimeSpan.Zero)));
    [Fact] public async Task Roundtrip_DateTimeOffset_MaxTick() => await Verifier.Verify(Roundtrip_DateTimeOffset(DateTimeOffset.MaxValue));

}

