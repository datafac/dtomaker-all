using System.Threading.Tasks;
using Xunit;

namespace Testing.PerfBench.UnitTests
{
    public class CloningTests
    {
        [Fact] public async ValueTask Cloning_Simple_MemoryPack() => _ = await new Cloning_Simple(true).MemoryPack();
        [Fact] public async ValueTask Cloning_Simple_MsgPack2() => _ = await new Cloning_Simple(true).MsgPack2();
        [Fact] public async ValueTask Cloning_Simple_MemBlocks() => _ = await new Cloning_Simple(true).MemBlocks();
        [Fact] public async ValueTask Cloning_Simple_NetStrux() => _ = await new Cloning_Simple(true).NetStrux();
        [Fact] public async ValueTask Cloning_Simple_HyperSer() => _ = await new Cloning_Simple(true).HyperSer();
        [Fact] public async ValueTask Cloning_Simple_JsonSystemText() => _ = await new Cloning_Simple(true).JsonSystemText();
        [Fact] public async ValueTask Cloning_Simple_JsonNewtonSoft() => _ = await new Cloning_Simple(true).JsonNewtonSoft();
        // todo Nino is buggy - try V4 again after release
        //[Fact] public async ValueTask Cloning_Simple_NinoSer() => _ = await new Cloning_Simple(true).NinoSer();
    }
}
