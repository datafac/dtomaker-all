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

