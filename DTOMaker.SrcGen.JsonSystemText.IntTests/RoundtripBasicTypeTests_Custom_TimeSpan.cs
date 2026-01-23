using DTOMaker.Models;
using DTOMaker.Runtime.JsonSystemText;
using DTOMaker.SrcGen.JsonSystemText.IntTests.JsonSystemText;
using Shouldly;
using System;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace DTOMaker.SrcGen.JsonSystemText.IntTests;

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

