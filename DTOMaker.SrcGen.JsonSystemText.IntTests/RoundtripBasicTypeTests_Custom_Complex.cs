using DTOMaker.Models;
using DTOMaker.Runtime.JsonSystemText;
using DTOMaker.SrcGen.JsonSystemText.IntTests.JsonSystemText;
using Shouldly;
using System;
using System.Numerics;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace DTOMaker.SrcGen.JsonSystemText.IntTests;

[Entity(42, LayoutMethod.Linear)]
public interface ISimpleDTO_Complex : IEntityBase { [Member(1, NativeType.PairOfInt64, typeof(DTOMaker.Models.ComplexConverter))] Complex Value { get; } }

public class RoundtripBasicTypeTests_Custom_Complex
{
    public string Roundtrip_Complex(Complex reqValue)
    {
        var orig = new SimpleDTO_Complex { Value = reqValue };
        orig.Freeze();
        orig.Value.ShouldBe(reqValue);
        var json = orig.SerializeToJson();
        var copy = json.DeserializeFromJson<SimpleDTO_Complex>();
        copy.ShouldNotBeNull();
        copy.ShouldBe(orig);
        copy.Value.ShouldBe(reqValue);
        return json;
    }

    [Fact] public async Task Roundtrip_Complex_Defaults() => await Verifier.Verify(Roundtrip_Complex(default));
    [Fact] public async Task Roundtrip_Complex_OneValue() => await Verifier.Verify(Roundtrip_Complex(Complex.One));
    [Fact] public async Task Roundtrip_Complex_OthValue() => await Verifier.Verify(Roundtrip_Complex(Complex.ImaginaryOne));

}

[Entity(44, LayoutMethod.Linear)]
public interface ISimpleDTO_DateTime : IEntityBase { [Member(1, NativeType.Int64, typeof(DTOMaker.Models.DateTimeConverter))] DateTime Value { get; } }

public class RoundtripBasicTypeTests_Custom_DateTime
{
    public string Roundtrip_DateTime(DateTime reqValue)
    {
        var orig = new SimpleDTO_DateTime { Value = reqValue };
        orig.Freeze();
        orig.Value.ShouldBe(reqValue);
        var json = orig.SerializeToJson();
        var copy = json.DeserializeFromJson<SimpleDTO_DateTime>();
        copy.ShouldNotBeNull();
        copy.ShouldBe(orig);
        copy.Value.ShouldBe(reqValue);
        return json;
    }

    [Fact] public async Task Roundtrip_DateTime_Default() => await Verifier.Verify(Roundtrip_DateTime(DateTime.MinValue));
    [Fact] public async Task Roundtrip_DateTime_OneTick() => await Verifier.Verify(Roundtrip_DateTime(DateTime.FromBinary(1)));
    [Fact] public async Task Roundtrip_DateTime_MaxTick() => await Verifier.Verify(Roundtrip_DateTime(DateTime.MaxValue));

}

[Entity(45, LayoutMethod.Linear)]
public interface ISimpleDTO_TimeSpan : IEntityBase { [Member(1, NativeType.Int64, typeof(DTOMaker.Models.TimeSpanConverter))] TimeSpan Value { get; } }

public class RoundtripBasicTypeTests_Custom_TimeSpan
{
    public string Roundtrip_TimeSpan(TimeSpan reqValue)
    {
        var orig = new SimpleDTO_TimeSpan { Value = reqValue };
        orig.Freeze();
        orig.Value.ShouldBe(reqValue);
        var json = orig.SerializeToJson();
        var copy = json.DeserializeFromJson<SimpleDTO_TimeSpan>();
        copy.ShouldNotBeNull();
        copy.ShouldBe(orig);
        copy.Value.ShouldBe(reqValue);
        return json;
    }

    [Fact] public async Task Roundtrip_TimeSpan_MinTick() => await Verifier.Verify(Roundtrip_TimeSpan(TimeSpan.MinValue));
    [Fact] public async Task Roundtrip_TimeSpan_Default() => await Verifier.Verify(Roundtrip_TimeSpan(TimeSpan.Zero));
    [Fact] public async Task Roundtrip_TimeSpan_OneTick() => await Verifier.Verify(Roundtrip_TimeSpan(TimeSpan.FromTicks(1)));
    [Fact] public async Task Roundtrip_TimeSpan_MaxTick() => await Verifier.Verify(Roundtrip_TimeSpan(TimeSpan.MaxValue));

}

[Entity(46, LayoutMethod.Linear)]
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

    private static readonly DateTime _locDate = new DateTime(2026, 1, 19, 22, 0, 0, DateTimeKind.Local);
    private static readonly DateTime _utcDate = new DateTime(2026, 1, 19, 22, 0, 0, DateTimeKind.Utc);
    [Fact] public async Task Roundtrip_DateTimeOffset_Default() => await Verifier.Verify(Roundtrip_DateTimeOffset(DateTimeOffset.MinValue));
    [Fact] public async Task Roundtrip_DateTimeOffset_LocDate() => await Verifier.Verify(Roundtrip_DateTimeOffset(new DateTimeOffset(_locDate, TimeSpan.FromHours(10))));
    [Fact] public async Task Roundtrip_DateTimeOffset_UtcDate() => await Verifier.Verify(Roundtrip_DateTimeOffset(new DateTimeOffset(_utcDate, TimeSpan.Zero)));
    [Fact] public async Task Roundtrip_DateTimeOffset_MaxTick() => await Verifier.Verify(Roundtrip_DateTimeOffset(DateTimeOffset.MaxValue));

}

