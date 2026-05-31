using MessagePack;
using Shouldly;
using System;
using System.Threading.Tasks;
using Xunit;

namespace NewModels.Tests
{
    public class RoundtripTests
    {
        [Fact]
        public async Task RoundtripNewModelAsLeaf()
        {
            using var dataStore = new DataFac.Storage.Testing.TestDataStore();
            var orig = new NewModels.Records.VarString() { Value = "The quick brown fox jumps over the lazy dog." };
            var send = new NewModels.MsgPack3.VarString(orig);
            await send.Pack(dataStore);
            var buffer = send.GetPacked();
            var recd = NewModels.MsgPack3.VarString.Deserialize(buffer);
            recd.Freeze();
            var copy = new NewModels.Records.VarString(recd);
            copy.ShouldBe(orig);
        }

        [Fact]
        public async Task RoundtripNewModelAsBase()
        {
            using var dataStore = new DataFac.Storage.Testing.TestDataStore();
            var orig = new NewModels.Records.VarString() { Value = "The quick brown fox jumps over the lazy dog." };
            var send = new NewModels.MsgPack3.VarString(orig);
            await send.Pack(dataStore);
            ReadOnlyMemory<byte> buffer = MessagePackSerializer.Serialize<NewModels.MsgPack3.DomainBase>(send, null, TestContext.Current.CancellationToken);
            var recd = MessagePackSerializer.Deserialize<NewModels.MsgPack3.DomainBase>(buffer, null, TestContext.Current.CancellationToken) as NewModels.MsgPack3.VarString;
            recd.ShouldNotBeNull();
            recd.Freeze();
            var copy = new NewModels.Records.VarString(recd);
            copy.ShouldBe(orig);
        }
    }
}
