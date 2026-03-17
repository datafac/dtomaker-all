using System.Threading.Tasks;
using Xunit;

namespace Testing.PerfBench.UnitTests
{
    public class RoundtripTests
    {
        [Fact] public async ValueTask Roundtrip_MemoryPack() => _ = await new Roundtrip_Int64(true).MemoryPack();
        [Fact] public async ValueTask Roundtrip_MsgPack2() => _ = await new Roundtrip_Int64(true).MsgPack2();
        [Fact] public async ValueTask Roundtrip_MemBlocks() => _ = await new Roundtrip_Int64(true).MemBlocks();
        [Fact] public async ValueTask Roundtrip_JsonSystemText() => _ = await new Roundtrip_Int64(true).JsonSystemText();
    }
}
