using Shouldly;
using System.Threading.Tasks;
using Xunit;

namespace T_ConcreteNameSpace_.Tests.MsgPack3
{
    using DTOMaker.Runtime.MsgPack3;

    public class RoundtripTests_MsgPack3
    {
        [Fact]
        public async Task RoundtripNewModelAsLeaf()
        {
            var cancellation = TestContext.Current.CancellationToken;
            using var blobStore = new DataFac.Storage.Testing.TestBlobStore();
            var orig = new T_ConcreteNameSpace_.Records.T_ConcreteImplName_() { Value = "The quick brown fox jumps over the lazy dog." };
            var send = new T_ConcreteNameSpace_.MsgPack3.T_ConcreteImplName_(orig);
            await send.Pack(blobStore, cancellation);
            var buffer = EntityBase.Serialize<T_ConcreteNameSpace_.MsgPack3.T_ConcreteImplName_>(send, cancellation);
            var recd = EntityBase.Deserialize<T_ConcreteNameSpace_.MsgPack3.T_ConcreteImplName_>(buffer, cancellation);
            recd.ShouldNotBeNull();
            recd.IsFrozen.ShouldBeTrue();
            recd.IsPacked.ShouldBeTrue();
            await recd.UnpackAll(blobStore, cancellation);
            var copy = new T_ConcreteNameSpace_.Records.T_ConcreteImplName_(recd);
            copy.ShouldBe(orig);
        }

        [Fact]
        public async Task RoundtripNewModelAsBase()
        {
            var cancellation = TestContext.Current.CancellationToken;
            using var blobStore = new DataFac.Storage.Testing.TestBlobStore();
            var orig = new T_ConcreteNameSpace_.Records.T_ConcreteImplName_() { Value = "The quick brown fox jumps over the lazy dog." };
            var send = new T_ConcreteNameSpace_.MsgPack3.T_ConcreteImplName_(orig);
            await send.Pack(blobStore, cancellation);
            var buffer = EntityBase.Serialize<T_AncestorNameSpace_.MsgPack3.T_AncestorImplName_>(send, cancellation);
            var recd = EntityBase.Deserialize<T_AncestorNameSpace_.MsgPack3.T_AncestorImplName_>(buffer, cancellation) as T_ConcreteNameSpace_.MsgPack3.T_ConcreteImplName_;
            recd.ShouldNotBeNull();
            recd.IsFrozen.ShouldBeTrue();
            recd.IsPacked.ShouldBeTrue();
            await recd.UnpackAll(blobStore, cancellation);
            var copy = new T_ConcreteNameSpace_.Records.T_ConcreteImplName_(recd);
            copy.ShouldBe(orig);
        }
    }
}

namespace T_ConcreteNameSpace_.Tests.MemBlox2
{
    using DTOMaker.Runtime.MemBlocks;

    public class RoundtripTests_MemBlox2
    {
        [Fact]
        public async Task RoundtripNewModelAsLeaf()
        {
            var cancellation = TestContext.Current.CancellationToken;
            using var blobStore = new DataFac.Storage.Testing.TestBlobStore();
            var orig = new T_ConcreteNameSpace_.Records.T_ConcreteImplName_() { Value = "The quick brown fox jumps over the lazy dog." };
            var send = new T_ConcreteNameSpace_.MemBlox2.T_ConcreteImplName_(orig);
            await send.Pack(blobStore, cancellation);
            var buffer = send.Serialize(cancellation);
            var recd = T_ConcreteNameSpace_.MemBlox2.T_ConcreteImplName_.DeserializeFrom(buffer);
            recd.ShouldNotBeNull();
            recd.IsFrozen.ShouldBeTrue();
            recd.IsPacked.ShouldBeTrue();
            await recd.UnpackAll(blobStore, cancellation);
            var copy = new T_ConcreteNameSpace_.Records.T_ConcreteImplName_(recd);
            copy.ShouldBe(orig);
        }

        [Fact]
        public async Task RoundtripNewModelAsBase()
        {
            var cancellation = TestContext.Current.CancellationToken;
            using var blobStore = new DataFac.Storage.Testing.TestBlobStore();
            var orig = new T_ConcreteNameSpace_.Records.T_ConcreteImplName_() { Value = "The quick brown fox jumps over the lazy dog." };
            var send = new T_ConcreteNameSpace_.MemBlox2.T_ConcreteImplName_(orig);
            await send.Pack(blobStore, cancellation);
            var buffer = send.Serialize(cancellation);
            var recdBase = T_AncestorNameSpace_.MemBlox2.T_AncestorImplName_.DeserializeFrom(buffer);
            recdBase.ShouldNotBeNull();
            recdBase.IsFrozen.ShouldBeTrue();
            recdBase.IsPacked.ShouldBeTrue();
            await recdBase.UnpackAll(blobStore, cancellation);
            recdBase.ShouldBeOfType<T_ConcreteNameSpace_.MemBlox2.T_ConcreteImplName_>();
            T_ConcreteNameSpace_.MemBlox2.T_ConcreteImplName_ recd = (recdBase as T_ConcreteNameSpace_.MemBlox2.T_ConcreteImplName_)!;
            var copy = new T_ConcreteNameSpace_.Records.T_ConcreteImplName_(recd);
            copy.ShouldBe(orig);
        }
    }
}
