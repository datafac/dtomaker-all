using DTOMaker.Models;
using DTOMaker.Runtime.MsgPack2;
using DTOMaker.SrcGen.MsgPack2.IntTests.MsgPack2;
using Shouldly;
using System;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace DTOMaker.SrcGen.MsgPack2.IntTests;

[Entity(4)]
public interface ISimpleDTO_Int08 : IEntityBase
{
    [Member(1)] SByte Field1 { get; set; }
    [Member(2)] SByte? Field2 { get; set; }
}

public class RoundtripBasicTypeTests_Int08
{
    public string Roundtrip_Int08(SByte reqValue, SByte? optValue)
    {
        var orig = new SimpleDTO_Int08 { Field1 = reqValue, Field2 = optValue };
        orig.Freeze();
        orig.Field1.ShouldBe(reqValue);
        orig.Field2.ShouldBe(optValue);
        ReadOnlyMemory<byte> buffer = orig.SerializeToMessagePack();
        var copy = buffer.DeserializeFromMessagePack<SimpleDTO_Int08>();
        copy.ShouldNotBeNull();
        copy.ShouldBe(orig);
        copy.Field1.ShouldBe(reqValue);
        return buffer.Span.ToDisplay();
    }

    [Fact] public async Task Roundtrip_Int08_Defaults() => await Verifier.Verify(Roundtrip_Int08(default, default));
    [Fact] public async Task Roundtrip_Int08_MaxValue() => await Verifier.Verify(Roundtrip_Int08(SByte.MaxValue, SByte.MaxValue));
    [Fact] public async Task Roundtrip_Int08_MinValue() => await Verifier.Verify(Roundtrip_Int08(SByte.MinValue, SByte.MinValue));
    [Fact] public async Task Roundtrip_Int08_UnitVals() => await Verifier.Verify(Roundtrip_Int08(1, -1));

}