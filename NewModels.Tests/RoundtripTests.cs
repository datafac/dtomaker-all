using Shouldly;
using System;
using System.Threading.Tasks;
using Xunit;

namespace NewModels.Tests
{

    public class RoundtripTests
    {
        [Fact]
        public async Task RoundtripNewModel()
        {
            using var dataStore = new DataFac.Storage.Testing.TestDataStore();
            var orig = new NewModels.Records.VarString() { Value = "The quick brown fox jumps over the lazy dog." };
            var send = new NewModels.MsgPack3.VarString(orig);
            await send.Pack(dataStore);
            var buffer = send.GetPacked();
            var recd = NewModels.MsgPack3.VarString.Deserialize(buffer);
            var copy = new NewModels.Records.VarString(recd);
            copy.ShouldBe(orig);
        }
    }
}
