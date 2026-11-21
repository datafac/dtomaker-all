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
        [Fact] public async ValueTask Roundtrip_Simple_JsonSystemText() => _ = await new Roundtrip_Simple(true).JsonSystemText();
        [Fact] public async ValueTask Roundtrip_Simple_JsonNewtonSoft() => _ = await new Roundtrip_Simple(true).JsonNewtonSoft();

        [Fact] public async ValueTask Roundtrip_PolyLeaf_MemoryPack() => _ = await new Roundtrip_PolyLeaf(true).MemoryPack();
        [Fact] public async ValueTask Roundtrip_PolyLeaf_MsgPack2() => _ = await new Roundtrip_PolyLeaf(true).MsgPack2();
        [Fact] public async ValueTask Roundtrip_PolyLeaf_MemBlocks() => _ = await new Roundtrip_PolyLeaf(true).MemBlocks();
        [Fact] public async ValueTask Roundtrip_PolyLeaf_NetStrux() => _ = await new Roundtrip_PolyLeaf(true).NetStrux();
        [Fact] public async ValueTask Roundtrip_PolyLeaf_JsonSystemText() => _ = await new Roundtrip_PolyLeaf(true).JsonSystemText();
        [Fact] public async ValueTask Roundtrip_PolyLeaf_JsonNewtonSoft() => _ = await new Roundtrip_PolyLeaf(true).JsonNewtonSoft();

        [Fact] public async ValueTask Roundtrip_PolyBase_MemoryPack() => _ = await new Roundtrip_PolyBase(true).MemoryPack();
        [Fact] public async ValueTask Roundtrip_PolyBase_MsgPack2() => _ = await new Roundtrip_PolyBase(true).MsgPack2();
        [Fact] public async ValueTask Roundtrip_PolyBase_MemBlocks() => _ = await new Roundtrip_PolyBase(true).MemBlocks();
        [Fact] public async ValueTask Roundtrip_PolyBase_NetStrux() => _ = await new Roundtrip_PolyBase(true).NetStrux();
        [Fact] public async ValueTask Roundtrip_PolyBase_JsonSystemText() => _ = await new Roundtrip_PolyBase(true).JsonSystemText();
        [Fact] public async ValueTask Roundtrip_PolyBase_JsonNewtonSoft() => _ = await new Roundtrip_PolyBase(true).JsonNewtonSoft();

        [Fact] public async ValueTask Roundtrip_Generic_MemoryPack() => _ = await new Roundtrip_Generic(true).MemoryPack();
    }
}