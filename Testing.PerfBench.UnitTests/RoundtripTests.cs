using System.Threading.Tasks;
using Xunit;

namespace Testing.PerfBench.UnitTests
{
    public class RoundtripTests
    {
        [Fact] public async ValueTask Roundtrip_Simple_MemoryPack() => _ = await new Roundtrip_Simple(true).MemoryPack();
        [Fact] public async ValueTask Roundtrip_Simple_MsgPack2() => _ = await new Roundtrip_Simple(true).MsgPack2();
        [Fact] public async ValueTask Roundtrip_Simple_MemBlocks() => _ = await new Roundtrip_Simple(true).MemBlocks();
        [Fact] public async ValueTask Roundtrip_Simple_NetStrux() => _ = await new Roundtrip_Simple(true).NetStrux();
        [Fact] public async ValueTask Roundtrip_Simple_HyperSer() => _ = await new Roundtrip_Simple(true).HyperSer();
        [Fact] public async ValueTask Roundtrip_Simple_JsonSystemText() => _ = await new Roundtrip_Simple(true).JsonSystemText();
        [Fact] public async ValueTask Roundtrip_Simple_JsonNewtonSoft() => _ = await new Roundtrip_Simple(true).JsonNewtonSoft();
        // todo Nino is buggy - try V4 again after release
        //[Fact] public async ValueTask Roundtrip_Simple_NinoSer() => _ = await new Roundtrip_Simple(true).NinoSer();

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
        //[InlineData(ValueKind.StrK002)] won't fit
        public async ValueTask Roundtrip_String_NetStrux(ValueKind valueKind) => _ = await new Roundtrip_String(true, valueKind).NetStrux();

        [Theory]
        [InlineData(ValueKind.StrZero)]
        [InlineData(ValueKind.StrB064)]
        [InlineData(ValueKind.StrK002)]
        public async ValueTask Roundtrip_String_HyperSer(ValueKind valueKind) => _ = await new Roundtrip_String(true, valueKind).HyperSer();

        // todo Nino is buggy - try V4 again after release
        //[Theory]
        //[InlineData(ValueKind.StrZero)]
        //[InlineData(ValueKind.StrB064)]
        //[InlineData(ValueKind.StrK002)]
        //public async ValueTask Roundtrip_String_NinoSer(ValueKind valueKind) => _ = await new Roundtrip_String(true, valueKind).NinoSer();

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
