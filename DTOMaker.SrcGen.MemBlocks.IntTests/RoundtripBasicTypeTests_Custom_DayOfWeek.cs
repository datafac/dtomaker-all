using DTOMaker.Models;
using DTOMaker.SrcGen.MemBlocks.IntTests.MemBlocks;
using Shouldly;
using System;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace DTOMaker.SrcGen.MemBlocks.IntTests;

[Entity(41)]
public interface ISimpleDTO_DayOfWeek : IEntityBase
{
    [Member(1, NativeType.Byte, typeof(DTOMaker.Models.DayOfWeekConverter))] DayOfWeek Field1 { get; }
    [Member(2, NativeType.Byte, typeof(DTOMaker.Models.DayOfWeekConverter))] DayOfWeek? Field2 { get; }
}

public class RoundtripBasicTypeTests_Custom_DayOfWeek
{
    public async Task<string> Roundtrip_DayOfWeekAsync(DayOfWeek reqValue, DayOfWeek? optValue)
    {
        using var dataStore = new DataFac.Storage.Testing.TestDataStore();
        var orig = new SimpleDTO_DayOfWeek { Field1 = reqValue, Field2 = optValue };
        await orig.Pack(dataStore);
        orig.Field1.ShouldBe(reqValue);
        orig.Field2.ShouldBe(optValue);
        var buffers = orig.GetBuffers();
        var copy = new SimpleDTO_DayOfWeek(buffers);
        copy.ShouldNotBeNull();
        copy.ShouldBe(orig);
        copy.Equals(orig).ShouldBeTrue();
        copy.Field1.ShouldBe(reqValue);
        copy.Field2.ShouldBe(optValue);
        return buffers.ToDisplay();
    }

    [Fact] public async Task Roundtrip_DayOfWeek_Defaults() => await Verifier.Verify(await Roundtrip_DayOfWeekAsync(default, null));
    [Fact] public async Task Roundtrip_DayOfWeek_OneValue() => await Verifier.Verify(await Roundtrip_DayOfWeekAsync(DayOfWeek.Monday, DayOfWeek.Tuesday));
    [Fact] public async Task Roundtrip_DayOfWeek_MaxValue() => await Verifier.Verify(await Roundtrip_DayOfWeekAsync(DayOfWeek.Saturday, DayOfWeek.Sunday));

}
