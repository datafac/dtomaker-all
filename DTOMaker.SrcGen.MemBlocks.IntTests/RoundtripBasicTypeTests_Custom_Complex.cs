using DataFac.Storage;
using DTOMaker.Models;
using DTOMaker.Runtime;
using DTOMaker.SrcGen.MemBlocks.IntTests.MemBlocks;
using Shouldly;
using System;
using System.Numerics;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace DTOMaker.SrcGen.MemBlocks.IntTests;

[Entity(42, LayoutMethod.Linear)]
public interface ISimpleDTO_Complex : IEntityBase { [Member(1, NativeType.PairOfInt64, "DTOMaker.Runtime.Converters.ComplexConverter")] Complex Value { get; } }

public class RoundtripBasicTypeTests_Custom_Complex
{
    public async Task<string> Roundtrip_ComplexAsync(Complex reqValue)
    {
        using var dataStore = new DataFac.Storage.Testing.TestDataStore();
        var orig = new SimpleDTO_Complex { Value = reqValue };
        await orig.Pack(dataStore);
        orig.Value.ShouldBe(reqValue);
        var buffers = orig.GetBuffers();
        var copy = new SimpleDTO_Complex(buffers);
        copy.ShouldNotBeNull();
        copy.ShouldBe(orig);
        copy.Value.ShouldBe(reqValue);
        return buffers.ToDisplay();
    }

    [Fact] public async Task Roundtrip_Complex_Defaults() => await Verifier.Verify(await Roundtrip_ComplexAsync(default));
    [Fact] public async Task Roundtrip_Complex_OneValue() => await Verifier.Verify(await Roundtrip_ComplexAsync(Complex.One));
    [Fact] public async Task Roundtrip_Complex_OthValue() => await Verifier.Verify(await Roundtrip_ComplexAsync(Complex.ImaginaryOne));

}
