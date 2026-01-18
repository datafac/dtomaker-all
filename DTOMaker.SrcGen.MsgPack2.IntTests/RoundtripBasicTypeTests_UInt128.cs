using DTOMaker.Models;
using DTOMaker.Runtime.MsgPack2;
using DTOMaker.SrcGen.MsgPack2.IntTests.MsgPack2;
using Shouldly;
using System;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace DTOMaker.SrcGen.MsgPack2.IntTests;

#if NET7_0_OR_GREATER
[Entity(22)]
public interface ISimpleDTO_UInt128 : IEntityBase
{
    [Member(1)] UInt128 Field1 { get; set; }
    [Member(2)] UInt128? Field2 { get; set; }
}

public class RoundtripBasicTypeTests_UInt128
{
    public string Roundtrip_UInt128(UInt128 reqValue, UInt128? optValue)
    {
        var orig = new SimpleDTO_UInt128 { Field1 = reqValue, Field2 = optValue };
        orig.Freeze();
        orig.Field1.ShouldBe(reqValue);
        orig.Field2.ShouldBe(optValue);
        ReadOnlyMemory<byte> buffer = orig.SerializeToMessagePack();
        var copy = buffer.DeserializeFromMessagePack<SimpleDTO_UInt128>();
        copy.ShouldNotBeNull();
        copy.ShouldBe(orig);
        copy.Field1.ShouldBe(reqValue);
        copy.Field2.ShouldBe(optValue);
        return buffer.Span.ToDisplay();
    }

    [Fact] public async Task Roundtrip_UInt128_Defaults() => await Verifier.Verify(Roundtrip_UInt128(default, default));
    [Fact] public async Task Roundtrip_UInt128_Maximums() => await Verifier.Verify(Roundtrip_UInt128(UInt128.MaxValue, UInt128.MinValue));
    [Fact] public async Task Roundtrip_UInt128_UnitVals() => await Verifier.Verify(Roundtrip_UInt128(UInt128.One, null));
    [Fact] public async Task Roundtrip_UInt128_ZeroVals() => await Verifier.Verify(Roundtrip_UInt128(UInt128.Zero, UInt128.Zero));

}
#endif
