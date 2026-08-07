using Shouldly;
using System.Threading.Tasks;
using Xunit;

namespace NewModels.Domain3.Tests.MsgPack3
{
    using DTOMaker.Runtime.MsgPack3;

    public class RoundtripTests_MsgPack3
    {
        [Fact]
        public async Task RoundtripNewModelAsLeaf()
        {
            var cancellation = TestContext.Current.CancellationToken;
            using var blobStore = new DataFac.Storage.Testing.TestBlobStore();
            var orig = new NewModels.Domain3.Records.T_ConcreteEntity_() { Value = "The quick brown fox jumps over the lazy dog." };
            var send = new NewModels.Domain3.MsgPack3.T_ConcreteEntity_(orig);
            await send.Pack(blobStore, cancellation);
            var buffer = EntityBase.Serialize<NewModels.Domain3.MsgPack3.T_ConcreteEntity_>(send, cancellation);
            var recd = EntityBase.Deserialize<NewModels.Domain3.MsgPack3.T_ConcreteEntity_>(buffer, cancellation);
            recd.ShouldNotBeNull();
            recd.IsFrozen.ShouldBeTrue();
            recd.IsPacked.ShouldBeTrue();
            await recd.UnpackAll(blobStore, cancellation);
            var copy = new NewModels.Domain3.Records.T_ConcreteEntity_(recd);
            copy.ShouldBe(orig);
        }

        [Fact]
        public async Task RoundtripNewModelAsBase()
        {
            var cancellation = TestContext.Current.CancellationToken;
            using var blobStore = new DataFac.Storage.Testing.TestBlobStore();
            var orig = new NewModels.Domain3.Records.T_ConcreteEntity_() { Value = "The quick brown fox jumps over the lazy dog." };
            var send = new NewModels.Domain3.MsgPack3.T_ConcreteEntity_(orig);
            await send.Pack(blobStore, cancellation);
            var buffer = EntityBase.Serialize<NewModels.Domain1.MsgPack3.T_BaseImplName_>(send, cancellation);
            var recd = EntityBase.Deserialize<NewModels.Domain1.MsgPack3.T_BaseImplName_>(buffer, cancellation) as NewModels.Domain3.MsgPack3.T_ConcreteEntity_;
            recd.ShouldNotBeNull();
            recd.IsFrozen.ShouldBeTrue();
            recd.IsPacked.ShouldBeTrue();
            await recd.UnpackAll(blobStore, cancellation);
            var copy = new NewModels.Domain3.Records.T_ConcreteEntity_(recd);
            copy.ShouldBe(orig);
        }
    }
}

namespace NewModels.Domain3.Tests.MemBlox2
{
    using DTOMaker.Runtime.MemBlocks;

    public class RoundtripTests_MemBlox2
    {
        [Fact]
        public async Task RoundtripNewModelAsLeaf()
        {
            var cancellation = TestContext.Current.CancellationToken;
            using var blobStore = new DataFac.Storage.Testing.TestBlobStore();
            var orig = new NewModels.Domain3.Records.T_ConcreteEntity_() { Value = "The quick brown fox jumps over the lazy dog." };
            var send = new NewModels.Domain3.MemBlox2.T_ConcreteEntity_(orig);
            await send.Pack(blobStore, cancellation);
            var buffer = send.Serialize(cancellation);
            var recd = NewModels.Domain3.MemBlox2.T_ConcreteEntity_.DeserializeFrom(buffer);
            recd.ShouldNotBeNull();
            recd.IsFrozen.ShouldBeTrue();
            recd.IsPacked.ShouldBeTrue();
            await recd.UnpackAll(blobStore, cancellation);
            var copy = new NewModels.Domain3.Records.T_ConcreteEntity_(recd);
            copy.ShouldBe(orig);
        }

        [Fact]
        public async Task RoundtripNewModelAsBase()
        {
            var cancellation = TestContext.Current.CancellationToken;
            using var blobStore = new DataFac.Storage.Testing.TestBlobStore();
            var orig = new NewModels.Domain3.Records.T_ConcreteEntity_() { Value = "The quick brown fox jumps over the lazy dog." };
            var send = new NewModels.Domain3.MemBlox2.T_ConcreteEntity_(orig);
            await send.Pack(blobStore, cancellation);
            var buffer = send.Serialize(cancellation);
            var recdBase = NewModels.Domain1.MemBlox2.T_BaseImplName_.DeserializeFrom(buffer);
            recdBase.ShouldNotBeNull();
            recdBase.IsFrozen.ShouldBeTrue();
            recdBase.IsPacked.ShouldBeTrue();
            await recdBase.UnpackAll(blobStore, cancellation);
            recdBase.ShouldBeOfType<NewModels.Domain3.MemBlox2.T_ConcreteEntity_>();
            NewModels.Domain3.MemBlox2.T_ConcreteEntity_ recd = (recdBase as NewModels.Domain3.MemBlox2.T_ConcreteEntity_)!;
            var copy = new NewModels.Domain3.Records.T_ConcreteEntity_(recd);
            copy.ShouldBe(orig);
        }
    }
}
