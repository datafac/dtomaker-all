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

[Entity(55, LayoutMethod.Linear)]
public interface ISimpleDTO_Quaternion : IEntityBase { [Member(1, NativeType.QuadOfInt32, typeof(QuaternionConverter))] Quaternion Value { get; } }

public class RoundtripBasicTypeTests_Custom_Quaternion
{
    public string Roundtrip_Quaternion(Quaternion reqValue)
    {
        var orig = new SimpleDTO_Quaternion { Value = reqValue };
        orig.Freeze();
        orig.Value.ShouldBe(reqValue);
        ReadOnlyMemory<byte> buffer = orig.SerializeToMessagePack();
        var copy = buffer.DeserializeFromMessagePack<SimpleDTO_Quaternion>();
        copy.ShouldNotBeNull();
        copy.ShouldBe(orig);
        copy.Value.ShouldBe(reqValue);
        return buffer.Span.ToDisplay();
    }

    [Fact] public async Task Roundtrip_Quaternion_Defaults() => await Verifier.Verify(Roundtrip_Quaternion(default));
    [Fact] public async Task Roundtrip_Quaternion_Value001() => await Verifier.Verify(Roundtrip_Quaternion(Quaternion.Identity));
    [Fact] public async Task Roundtrip_Quaternion_Value002() => await Verifier.Verify(Roundtrip_Quaternion(new Quaternion(Vector3.UnitX, 2.0F)));
    [Fact] public async Task Roundtrip_Quaternion_Value003() => await Verifier.Verify(Roundtrip_Quaternion(new Quaternion(Vector3.UnitY, 3.0F)));
    [Fact] public async Task Roundtrip_Quaternion_Value004() => await Verifier.Verify(Roundtrip_Quaternion(new Quaternion(Vector3.UnitZ, 4.0F)));

}

