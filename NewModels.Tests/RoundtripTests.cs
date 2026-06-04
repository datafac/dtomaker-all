using Shouldly;
using System.Threading.Tasks;
using Xunit;

namespace NewModels.Tests.MsgPack3
{
    using DTOMaker.Runtime.MsgPack3;

    public class RoundtripTests_MsgPack3
    {
        [Fact]
        public async Task RoundtripNewModelAsLeaf()
        {
            var cancellation = TestContext.Current.CancellationToken;
            using var dataStore = new DataFac.Storage.Testing.TestDataStore();
            var orig = new NewModels.Records.T_ConcreteEntity_() { Value = "The quick brown fox jumps over the lazy dog." };
            var send = new NewModels.MsgPack3.T_ConcreteEntity_(orig);
            await send.Pack(dataStore, cancellation);
            var buffer = EntityBase.Serialize<NewModels.MsgPack3.T_ConcreteEntity_>(send, cancellation);
            var recd = EntityBase.Deserialize<NewModels.MsgPack3.T_ConcreteEntity_>(buffer, cancellation);
            recd.ShouldNotBeNull();
            recd.IsFrozen.ShouldBeTrue();
            recd.IsPacked.ShouldBeTrue();
            await recd.UnpackAll(dataStore, cancellation);
            var copy = new NewModels.Records.T_ConcreteEntity_(recd);
            copy.ShouldBe(orig);
        }

        [Fact]
        public async Task RoundtripNewModelAsBase()
        {
            var cancellation = TestContext.Current.CancellationToken;
            using var dataStore = new DataFac.Storage.Testing.TestDataStore();
            var orig = new NewModels.Records.T_ConcreteEntity_() { Value = "The quick brown fox jumps over the lazy dog." };
            var send = new NewModels.MsgPack3.T_ConcreteEntity_(orig);
            await send.Pack(dataStore, cancellation);
            var buffer = EntityBase.Serialize<NewModels.MsgPack3.T_BaseImplName_>(send, cancellation);
            var recd = EntityBase.Deserialize<NewModels.MsgPack3.T_BaseImplName_>(buffer, cancellation) as NewModels.MsgPack3.T_ConcreteEntity_;
            recd.ShouldNotBeNull();
            recd.IsFrozen.ShouldBeTrue();
            recd.IsPacked.ShouldBeTrue();
            await recd.UnpackAll(dataStore, cancellation);
            var copy = new NewModels.Records.T_ConcreteEntity_(recd);
            copy.ShouldBe(orig);
        }
    }
}

namespace NewModels.Tests.MemBlox2
{
    using DTOMaker.Runtime.MemBlocks;

    public class RoundtripTests_MemBlox2
    {
        [Fact]
        public async Task RoundtripNewModelAsLeaf()
        {
            var cancellation = TestContext.Current.CancellationToken;
            using var dataStore = new DataFac.Storage.Testing.TestDataStore();
            var orig = new NewModels.Records.T_ConcreteEntity_() { Value = "The quick brown fox jumps over the lazy dog." };
            var send = new NewModels.MemBlox2.T_ConcreteEntity_(orig);
            await send.Pack(dataStore, cancellation);
            var buffer = send.Serialize(cancellation);
            var recd = NewModels.MemBlox2.T_ConcreteEntity_.DeserializeFrom(buffer);
            recd.ShouldNotBeNull();
            recd.IsFrozen.ShouldBeTrue();
            recd.IsPacked.ShouldBeTrue();
            await recd.UnpackAll(dataStore, cancellation);
            var copy = new NewModels.Records.T_ConcreteEntity_(recd);
            copy.ShouldBe(orig);
        }

        [Fact]
        public async Task RoundtripNewModelAsBase()
        {
            var cancellation = TestContext.Current.CancellationToken;
            using var dataStore = new DataFac.Storage.Testing.TestDataStore();
            var orig = new NewModels.Records.T_ConcreteEntity_() { Value = "The quick brown fox jumps over the lazy dog." };
            var send = new NewModels.MemBlox2.T_ConcreteEntity_(orig);
            await send.Pack(dataStore, cancellation);
            var buffer = send.Serialize(cancellation);
            var recdBase = NewModels.MemBlox2.T_BaseImplName_.DeserializeFrom(buffer);
            recdBase.ShouldNotBeNull();
            recdBase.IsFrozen.ShouldBeTrue();
            recdBase.IsPacked.ShouldBeTrue();
            await recdBase.UnpackAll(dataStore, cancellation);
            recdBase.ShouldBeOfType<NewModels.MemBlox2.T_ConcreteEntity_>();
            NewModels.MemBlox2.T_ConcreteEntity_ recd =(recdBase as NewModels.MemBlox2.T_ConcreteEntity_)!;
            var copy = new NewModels.Records.T_ConcreteEntity_(recd);
            copy.ShouldBe(orig);
        }
    }
}
