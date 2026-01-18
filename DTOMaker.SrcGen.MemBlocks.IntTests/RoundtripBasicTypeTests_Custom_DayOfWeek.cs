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

[Entity(41, LayoutMethod.Linear)]
public interface ISimpleDTO_DayOfWeek : IEntityBase { [Member(1, NativeType.Int32, typeof(DTOMaker.Runtime.Converters.DayOfWeekConverter))] DayOfWeek Value { get; } }

public class RoundtripBasicTypeTests_Custom_DayOfWeek
{
    public async Task<string> Roundtrip_DayOfWeekAsync(DayOfWeek reqValue)
    {
        using var dataStore = new DataFac.Storage.Testing.TestDataStore();
        var orig = new SimpleDTO_DayOfWeek { Value = reqValue };
        await orig.Pack(dataStore);
        orig.Value.ShouldBe(reqValue);
        var buffers = orig.GetBuffers();
        var copy = new SimpleDTO_DayOfWeek(buffers);
        copy.ShouldNotBeNull();
        copy.ShouldBe(orig);
        copy.Value.ShouldBe(reqValue);
        return buffers.ToDisplay();
    }

    [Fact] public async Task Roundtrip_DayOfWeek_Defaults() => await Verifier.Verify(await Roundtrip_DayOfWeekAsync(default));
    [Fact] public async Task Roundtrip_DayOfWeek_OneValue() => await Verifier.Verify(await Roundtrip_DayOfWeekAsync(DayOfWeek.Monday));
    [Fact] public async Task Roundtrip_DayOfWeek_MaxValue() => await Verifier.Verify(await Roundtrip_DayOfWeekAsync(DayOfWeek.Saturday));

}
