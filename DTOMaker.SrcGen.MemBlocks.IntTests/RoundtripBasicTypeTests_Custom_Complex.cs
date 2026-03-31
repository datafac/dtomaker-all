using DTOMaker.Converters.Numerics;
using DTOMaker.Models;
using DTOMaker.SrcGen.MemBlocks.IntTests.MemBlocks;
using Shouldly;
using System;
using System.Numerics;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace DTOMaker.SrcGen.MemBlocks.IntTests;

[Entity(50)]
public interface ISimpleDTO_Complex : IEntityBase
{
    [Member(1, NativeType.PairOfInt64, typeof(ComplexConverter))] Complex Field1 { get; }
    [Member(2, NativeType.PairOfInt64, typeof(ComplexConverter))] Complex? Field2 { get; }
}

public class RoundtripBasicTypeTests_Custom_Complex
{
    public async Task<string> Roundtrip_ComplexAsync(Complex reqValue, Complex? optValue)
    {
        using var dataStore = new DataFac.Storage.Testing.TestDataStore();
        var orig = new SimpleDTO_Complex { Field1 = reqValue, Field2 = optValue };
        await orig.Pack(dataStore);
        orig.Field1.ShouldBe(reqValue);
        orig.Field2.ShouldBe(optValue);
        var buffer = orig.GetBuffer();
        var copy = new SimpleDTO_Complex(buffer);
        copy.ShouldNotBeNull();
        copy.ShouldBe(orig);
        copy.Equals(orig).ShouldBeTrue();
        copy.Field1.ShouldBe(reqValue);
        copy.Field2.ShouldBe(optValue);
        return buffer.ToDisplay();
    }

    [Fact] public async Task Roundtrip_Complex_Defaults() => await Verifier.Verify(await Roundtrip_ComplexAsync(default, null));
    [Fact] public async Task Roundtrip_Complex_OneValue() => await Verifier.Verify(await Roundtrip_ComplexAsync(Complex.One, Complex.Zero));
    [Fact] public async Task Roundtrip_Complex_OthValue() => await Verifier.Verify(await Roundtrip_ComplexAsync(Complex.ImaginaryOne, Complex.Zero));

}

