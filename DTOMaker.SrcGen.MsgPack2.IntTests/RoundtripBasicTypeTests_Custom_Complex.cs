using DTOMaker.Models;
using DTOMaker.Runtime;
using DTOMaker.Runtime.MsgPack2;
using DTOMaker.SrcGen.MsgPack2.IntTests.MsgPack2;
using Shouldly;
using System;
using System.Numerics;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace DTOMaker.SrcGen.MsgPack2.IntTests;

[Entity(42, LayoutMethod.Linear)]
public interface ISimpleDTO_Complex : IEntityBase { [Member(1, NativeType.PairOfInt64, "DTOMaker.Runtime.Converters.ComplexConverter")] Complex Value { get; } }

public class RoundtripBasicTypeTests_Custom_Complex
{
    public string Roundtrip_Complex(Complex reqValue)
    {
        var orig = new SimpleDTO_Complex { Value = reqValue };
        orig.Freeze();
        orig.Value.ShouldBe(reqValue);
        ReadOnlyMemory<byte> buffer = orig.SerializeToMessagePack();
        var copy = buffer.DeserializeFromMessagePack<SimpleDTO_Complex>();
        copy.ShouldNotBeNull();
        copy.ShouldBe(orig);
        copy.Value.ShouldBe(reqValue);
        return buffer.Span.ToDisplay();
    }

    [Fact] public async Task Roundtrip_Complex_Defaults() => await Verifier.Verify(Roundtrip_Complex(default));
    [Fact] public async Task Roundtrip_Complex_OneValue() => await Verifier.Verify(Roundtrip_Complex(Complex.One));
    [Fact] public async Task Roundtrip_Complex_OthValue() => await Verifier.Verify(Roundtrip_Complex(Complex.ImaginaryOne));

}

