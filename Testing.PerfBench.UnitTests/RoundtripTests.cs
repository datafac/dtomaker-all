using System.Threading.Tasks;
using Xunit;

namespace Testing.PerfBench.UnitTests
{
    public class RoundtripTests
    {
        [Fact] public async ValueTask Roundtrip_Int64_MemoryPack() => _ = await new Roundtrip_Int64(true).MemoryPack();
        [Fact] public async ValueTask Roundtrip_Int64_MsgPack2() => _ = await new Roundtrip_Int64(true).MsgPack2();
        [Fact] public async ValueTask Roundtrip_Int64_MemBlocks() => _ = await new Roundtrip_Int64(true).MemBlocks();
        [Fact] public async ValueTask Roundtrip_Int64_JsonSystemText() => _ = await new Roundtrip_Int64(true).JsonSystemText();
        [Fact] public async ValueTask Roundtrip_Int64_JsonNewtonSoft() => _ = await new Roundtrip_Int64(true).JsonNewtonSoft();

        [Theory]
        [InlineData(ValueKind.StrZero)]
        [InlineData(ValueKind.StrB064)]
        [InlineData(ValueKind.StrK002)]
        public async ValueTask Roundtrip_String_MemoryPack(ValueKind valueKind) => _ = await new Roundtrip_String(true, valueKind).MemoryPack();

        [Theory]
        [InlineData(ValueKind.StrZero)]
        [InlineData(ValueKind.StrB064)]
        [InlineData(ValueKind.StrK002)]
        public async ValueTask Roundtrip_String_MsgPack2(ValueKind valueKind) => _ = await new Roundtrip_String(true, valueKind).MsgPack2();

        [Theory]
        [InlineData(ValueKind.StrZero)]
        [InlineData(ValueKind.StrB064)]
        [InlineData(ValueKind.StrK002)]
        public async ValueTask Roundtrip_String_MemBlocks(ValueKind valueKind) => _ = await new Roundtrip_String(true, valueKind).MemBlocks();

        [Theory]
        [InlineData(ValueKind.StrZero)]
        [InlineData(ValueKind.StrB064)]
        [InlineData(ValueKind.StrK002)]
        public async ValueTask Roundtrip_String_JsonSystemText(ValueKind valueKind) => _ = await new Roundtrip_String(true, valueKind).JsonSystemText();

        [Theory]
        [InlineData(ValueKind.StrZero)]
        [InlineData(ValueKind.StrB064)]
        [InlineData(ValueKind.StrK002)]
        public async ValueTask Roundtrip_String_JsonNewtonSoft(ValueKind valueKind) => _ = await new Roundtrip_String(true, valueKind).JsonNewtonSoft();

    }
}
