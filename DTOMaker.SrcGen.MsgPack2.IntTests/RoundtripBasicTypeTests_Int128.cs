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

#if NET7_0_OR_GREATER
[Entity(21)]
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
        ReadOnlyMemory<byte> buffer = orig.SerializeToMessagePack();
        var copy = buffer.DeserializeFromMessagePack<SimpleDTO_Int128>();
        copy.ShouldNotBeNull();
        copy.ShouldBe(orig);
        copy.Field1.ShouldBe(reqValue);
        return buffer.Span.ToDisplay();
    }

    [Fact] public async Task Roundtrip_Int128_Defaults() => await Verifier.Verify(Roundtrip_Int128(default, default));
    [Fact] public async Task Roundtrip_Int128_Maximums() => await Verifier.Verify(Roundtrip_Int128(Int128.MaxValue, Int128.MinValue));
    [Fact] public async Task Roundtrip_Int128_UnitVals() => await Verifier.Verify(Roundtrip_Int128(Int128.One, null));
    [Fact] public async Task Roundtrip_Int128_ZeroVals() => await Verifier.Verify(Roundtrip_Int128(Int128.Zero, Int128.Zero));

}
#endif
