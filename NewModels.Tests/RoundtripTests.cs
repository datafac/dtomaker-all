using DTOMaker.Runtime.MsgPack3;
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
            var buffer = send.SerializeToMessagePack<NewModels.MsgPack3.VarString>(TestContext.Current.CancellationToken);
            var recd = buffer.DeserializeFromMessagePack<NewModels.MsgPack3.VarString>(TestContext.Current.CancellationToken);
            recd.ShouldNotBeNull();
            recd.IsFrozen.ShouldBeTrue();
            //todo recd.IsPacked.ShouldbeTrue();
            await recd.UnpackAll(dataStore);
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
            var buffer = send.SerializeToMessagePack<NewModels.MsgPack3.DomainBase>(TestContext.Current.CancellationToken);
            var recd = buffer.DeserializeFromMessagePack<NewModels.MsgPack3.DomainBase>(TestContext.Current.CancellationToken) as NewModels.MsgPack3.VarString;
            recd.ShouldNotBeNull();
            recd.IsFrozen.ShouldBeTrue();
            //todo recd.IsPacked.ShouldbeTrue();
            await recd.UnpackAll(dataStore);
            var copy = new NewModels.Records.VarString(recd);
            copy.ShouldBe(orig);
        }
    }
}
