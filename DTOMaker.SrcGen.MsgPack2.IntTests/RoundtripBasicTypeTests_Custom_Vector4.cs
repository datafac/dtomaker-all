using DTOMaker.Converters.Numerics;
using DTOMaker.Models;
using DTOMaker.Runtime.MsgPack2;
using DTOMaker.SrcGen.MsgPack2.IntTests.MsgPack2;
using Shouldly;
using System;
using System.Numerics;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace DTOMaker.SrcGen.MsgPack2.IntTests;

[Entity(53, LayoutMethod.Compact)]
public interface ISimpleDTO_Vector4 : IEntityBase { [Member(1, NativeType.QuadOfInt32, typeof(Vector4Converter))] Vector4 Value { get; } }

public class RoundtripBasicTypeTests_Custom_Vector4
{
    public string Roundtrip_Vector4(Vector4 reqValue)
    {
        var orig = new SimpleDTO_Vector4 { Value = reqValue };
        orig.Freeze();
        orig.Value.ShouldBe(reqValue);
        ReadOnlyMemory<byte> buffer = orig.SerializeToMessagePack();
        var copy = buffer.DeserializeFromMessagePack<SimpleDTO_Vector4>();
        copy.ShouldNotBeNull();
        copy.ShouldBe(orig);
        copy.Value.ShouldBe(reqValue);
        return buffer.Span.ToDisplay();
    }

    [Fact] public async Task Roundtrip_Vector4_Defaults() => await Verifier.Verify(Roundtrip_Vector4(default));
    [Fact] public async Task Roundtrip_Vector4_Value001() => await Verifier.Verify(Roundtrip_Vector4(Vector4.UnitX));
    [Fact] public async Task Roundtrip_Vector4_Value002() => await Verifier.Verify(Roundtrip_Vector4(Vector4.UnitY));
    [Fact] public async Task Roundtrip_Vector4_Value003() => await Verifier.Verify(Roundtrip_Vector4(Vector4.UnitZ));
    [Fact] public async Task Roundtrip_Vector4_Value004() => await Verifier.Verify(Roundtrip_Vector4(Vector4.UnitW));

}

