using DataFac.Memory;
using DataFac.Storage;
using DTOMaker.Models;
using DTOMaker.Runtime;
using DTOMaker.Runtime.MemBlocks;
using Shouldly;
using System;
using System.Buffers;
using System.Linq;
using System.Threading.Tasks;
using T_BaseImplNameSpace_;
using T_ImplNameSpace_;

#pragma warning disable CS0618 // Type or member is obsolete

namespace Template.MemBlocks.Tests
{
    internal interface ITestEntity
    {

    }
    internal sealed class TestEntity : EntityBase, ITestEntity, IEquatable<TestEntity>, IMemBlocksEntity<TestEntity>
    {
        private sealed class _EntityFactory : IMemBlocksEntityFactory<TestEntity>
        {
            public TestEntity CreateInstance(ReadOnlyMemory<byte> buffer) => TestEntity.DeserializeFrom(buffer);
        }
        private static readonly _EntityFactory _factory = new _EntityFactory();
        public IMemBlocksEntityFactory<TestEntity> GetFactory() => _factory;

        //##if(false) {
        //private const int T_ClassHeight_ = 2;
        //private const int T_BlockLength_ = 1024;
        //private const bool T_MemberObsoleteIsError_ = false;
        //private const long T_BlockStructureCode_ = 0x0A62;
        //##}
        private static readonly long _structureBits = 0x0051;
        private const int ClassHeight = 1;
        public const int EntityId = 4;
        private const int BlockLength = 32;
        private readonly Memory<byte> _writableLocalBlock;
        private readonly ReadOnlyMemory<byte> _readonlyLocalBlock;

        private static readonly EntityMetadata _metadata = new EntityMetadata(EntityId, _structureBits);

        private static TestEntity DeserializeFrom(ReadOnlyMemory<byte> buffer)
        {
            EntityContent content = EntityContent.FromSingleBuffer(_metadata, buffer);
            return new TestEntity(content);
        }
        public static TestEntity CreateInstance(ReadOnlyMemory<byte> buffer) => TestEntity.DeserializeFrom(buffer);

        protected override int OnGetEntityId() => EntityId;
        protected override void OnFreeze() => base.OnFreeze();
        protected override void OnGetBuffers(Span<ReadOnlyMemory<byte>> buffers)
        {
            base.OnGetBuffers(buffers);
            buffers[ClassHeight] = _readonlyLocalBlock;
        }
        protected override ValueTask OnPack(IDataStore dataStore) => base.OnPack(dataStore);
        protected override ValueTask OnUnpack(IDataStore dataStore, int depth) => base.OnUnpack(dataStore, depth);
        protected override IEntityBase OnPartCopy() => new TestEntity(this);

        public TestEntity() : base(_metadata)
        {
            _readonlyLocalBlock = _writableLocalBlock = new byte[BlockLength];
        }
        public TestEntity(TestEntity source) : base(_metadata)
        {
            _readonlyLocalBlock = _writableLocalBlock = new byte[BlockLength];
            //this.Field1 = source.Field1;
        }

        protected TestEntity(EntityMetadata metadata, EntityContent content) : base(metadata, content)
        {
            var sourceBlock = content.Buffers[ClassHeight];
            if (sourceBlock.Length < BlockLength)
            {
                // source too small - allocate new
                Memory<byte> memory = new byte[BlockLength];
                sourceBlock.CopyTo(memory);
                _readonlyLocalBlock = memory;
            }
            else
            {
                _readonlyLocalBlock = sourceBlock;
            }
            _writableLocalBlock = Memory<byte>.Empty;
        }

        internal TestEntity(EntityContent content) : this(_metadata, content) { }

        public bool Equals(TestEntity? other)
        {
            if (ReferenceEquals(this, other)) return true;
            if (other is null) return false;
            if (!base.Equals(other)) return false;
            if (!_readonlyLocalBlock.Span.SequenceEqual(other._readonlyLocalBlock.Span)) return false;
            return true;
        }
        public override bool Equals(object? obj) => obj is TestEntity other && Equals(other);
        public override int GetHashCode() => base.GetHashCode();
    }
    public class EntityBaseTests
    {
        [Fact]
        public void ParseBlockHeader()
        {
            BlockB016 outgoing = default;
            // signature
            outgoing.A.A.A.A.ByteValue = (byte)'|';
            outgoing.A.A.A.B.ByteValue = (byte)'_';
            outgoing.A.A.B.A.ByteValue = (byte)2;
            outgoing.A.A.B.B.ByteValue = (byte)1;
            // entity id
            outgoing.A.B.A.Int16ValueLE = 4;
            // structure
            outgoing.B.Int64ValueLE = 0x61;

            Memory<byte> buffer = new byte[BlockHeader.HeaderSize];
            bool written = outgoing.TryWrite(buffer.Span);
            written.ShouldBeTrue();

            BlockHeader incoming = BlockHeader.ParseFrom(buffer);
            incoming.SignatureBits.ShouldBe(0x01025f7c);
            incoming.StructureBits.ShouldBe(0x61);
            incoming.EntityId.ShouldBe(4);
        }

        [Fact]
        public async Task BlockHeaderIsConstant()
        {
            using var dataStore = new DataFac.Storage.Testing.TestDataStore();
            var orig = new TestEntity();
            await orig.Pack(dataStore);
            orig.Freeze();
            var buffer = orig.GetContent().ToSingleBuffer();
            buffer.Length.ShouldBe(48);

            buffer.Span[0].ShouldBe((byte)'|');  // marker byte 0
            buffer.Span[1].ShouldBe((byte)'_');  // marker byte 1
            buffer.Span[2].ShouldBe((byte)2);    // major version
            buffer.Span[3].ShouldBe((byte)1);    // minor version

            BlockHeader parsed = BlockHeader.ParseFrom(buffer);
            parsed.SignatureBits.ShouldBe(0x01025f7c);
            parsed.StructureBits.ShouldBe(0x51);
            parsed.EntityId.ShouldBe(4);
        }
    }

    public class RoundtripTests
    {
        private static readonly Octets smallBinary = new Octets(new byte[] { 1, 2, 3, 4, 5, 6, 7 });
        private static readonly Octets largeBinary = new Octets(Enumerable.Range(0, 256).Select(i => (byte)i).ToArray());

        [Fact]

        public async Task Roundtrip_Direct()
        {
            using var dataStore = new DataFac.Storage.Testing.TestDataStore();

            var orig = new T_EntityImplName_();
            orig.BaseField1 = 321;
            orig.T_RequiredNativeStructMemberName_ = 123;
            orig.T_NullableNativeStructMemberName_ = 456;
            orig.T_RequiredCustomStructMemberName_ = DayOfWeek.Monday;
            orig.T_NullableCustomStructMemberName_ = DayOfWeek.Thursday;
            orig.T_RequiredStringMemberName_ = "def";
            orig.T_NullableStringMemberName_ = null;
            orig.T_RequiredBinaryMemberName_ = largeBinary;
            orig.T_NullableBinaryMemberName_ = largeBinary;
            orig.T_RequiredEntityMemberName_ = new T_MemberTypeImplSpace_.T_MemberTypeImplName_();
            orig.T_NullableEntityMemberName_ = new T_MemberTypeImplSpace_.T_MemberTypeImplName_();
            await orig.Pack(dataStore);

            var copy = new T_EntityImplName_(orig);
            await copy.Pack(dataStore);
            copy.IsFrozen.ShouldBeTrue();
            copy.Equals(orig).ShouldBeTrue();
            copy.ShouldBe(orig);
            copy.GetHashCode().ShouldBe(orig.GetHashCode());
        }

        [Fact]
        public async Task Roundtrip_AsEntity()
        {
            using var dataStore = new DataFac.Storage.Testing.TestDataStore();

            var orig = new T_EntityImplName_();
            orig.BaseField1 = 321;
            orig.T_RequiredNativeStructMemberName_ = 123;
            orig.T_NullableNativeStructMemberName_ = 456;
            orig.T_RequiredCustomStructMemberName_ = DayOfWeek.Monday;
            orig.T_NullableCustomStructMemberName_ = DayOfWeek.Thursday;
            orig.T_RequiredStringMemberName_ = "def";
            orig.T_NullableStringMemberName_ = null;
            orig.T_RequiredBinaryMemberName_ = largeBinary;
            orig.T_NullableBinaryMemberName_ = largeBinary;
            orig.T_RequiredEntityMemberName_ = new T_MemberTypeImplSpace_.T_MemberTypeImplName_();
            orig.T_NullableEntityMemberName_ = new T_MemberTypeImplSpace_.T_MemberTypeImplName_();
            await orig.Pack(dataStore);
            orig.Freeze();

            var buffer = orig.GetContent().ToSingleBuffer();
            var copy = T_ImplNameSpace_.T_EntityImplName_.CreateInstance(buffer);
            await copy.UnpackAll(dataStore);

            copy.IsFrozen.ShouldBeTrue();
            copy.Equals(orig).ShouldBeTrue();
            copy.ShouldBe(orig);
            copy.GetHashCode().ShouldBe(orig.GetHashCode());
        }

        [Fact]
        public async Task Roundtrip_AsBase()
        {
            using var dataStore = new DataFac.Storage.Testing.TestDataStore();

            var orig = new T_EntityImplName_();
            orig.BaseField1 = 321;
            orig.T_RequiredNativeStructMemberName_ = 123;
            orig.T_NullableNativeStructMemberName_ = 456;
            orig.T_RequiredCustomStructMemberName_ = DayOfWeek.Monday;
            orig.T_NullableCustomStructMemberName_ = DayOfWeek.Thursday;
            orig.T_RequiredStringMemberName_ = "def";
            orig.T_NullableStringMemberName_ = null;
            orig.T_RequiredBinaryMemberName_ = largeBinary;
            orig.T_NullableBinaryMemberName_ = largeBinary;
            orig.T_RequiredEntityMemberName_ = new T_MemberTypeImplSpace_.T_MemberTypeImplName_();
            orig.T_NullableEntityMemberName_ = new T_MemberTypeImplSpace_.T_MemberTypeImplName_();
            await orig.Pack(dataStore);
            orig.Freeze();

            var buffer = orig.GetContent().ToSingleBuffer();
            var recd = T_BaseImplNameSpace_.T_BaseImplName_.CreateInstance(buffer);
            recd.ShouldBeOfType<T_EntityImplName_>();
            var copy = recd as T_EntityImplName_;
            copy.ShouldNotBeNull();
            await copy.UnpackAll(dataStore);

            copy.IsFrozen.ShouldBeTrue();
            copy.Equals(orig).ShouldBeTrue();
            copy.ShouldBe(orig);
            copy.GetHashCode().ShouldBe(orig.GetHashCode());
        }
    }
}