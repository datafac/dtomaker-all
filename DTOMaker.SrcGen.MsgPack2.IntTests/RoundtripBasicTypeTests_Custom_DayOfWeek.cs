using DTOMaker.Models;
using DTOMaker.Runtime;
using DTOMaker.Runtime.MsgPack2;
using DTOMaker.SrcGen.MsgPack2.IntTests.MsgPack2;
using Shouldly;
using System;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace DTOMaker.SrcGen.MsgPack2.IntTests;

[Entity(41, LayoutMethod.Linear)]
public interface ISimpleDTO_DayOfWeek : IEntityBase { [Member(1, NativeType.Int32, typeof(DTOMaker.Runtime.Converters.DayOfWeekConverter))] DayOfWeek Value { get; } }

public class RoundtripBasicTypeTests_Custom_DayOfWeek
{
    public string Roundtrip_DayOfWeek(DayOfWeek reqValue)
    {
        var orig = new SimpleDTO_DayOfWeek { Value = reqValue };
        orig.Freeze();
        orig.Value.ShouldBe(reqValue);
        ReadOnlyMemory<byte> buffer = orig.SerializeToMessagePack();
        var copy = buffer.DeserializeFromMessagePack<SimpleDTO_DayOfWeek>();
        copy.ShouldNotBeNull();
        copy.ShouldBe(orig);
        copy.Value.ShouldBe(reqValue);
        return buffer.Span.ToDisplay();
    }

    [Fact] public async Task Roundtrip_DayOfWeek_Defaults() => await Verifier.Verify(Roundtrip_DayOfWeek(default));
    [Fact] public async Task Roundtrip_DayOfWeek_OneValue() => await Verifier.Verify(Roundtrip_DayOfWeek(DayOfWeek.Monday));
    [Fact] public async Task Roundtrip_DayOfWeek_MaxValue() => await Verifier.Verify(Roundtrip_DayOfWeek(DayOfWeek.Saturday));

}

