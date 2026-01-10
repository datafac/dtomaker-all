using DataFac.Memory;
using DataFac.Storage;
using DTOMaker.Runtime.MemBlocks;
using Shouldly;
using System;
using System.Buffers;
using System.Linq;
using System.Threading.Tasks;
using T_ImplNameSpace_;

#pragma warning disable CS0618 // Type or member is obsolete

namespace Template.MemBlocks.Tests
{
    internal sealed class TestEntity : EntityBase
    {
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

        private static readonly BlockHeader _header = BlockHeader.CreateNew(EntityId, _structureBits);

        protected override int OnGetClassHeight() => ClassHeight;
        protected override ReadOnlySequenceBuilder<byte> OnSequenceBuilder(ReadOnlySequenceBuilder<byte> builder) => base.OnSequenceBuilder(builder).Append(_readonlyLocalBlock);
        protected override int OnGetEntityId() => EntityId;
        public TestEntity() : base(_header)
        {
            _readonlyLocalBlock = _writableLocalBlock = new byte[BlockLength];
        }
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
            outgoing.A.A.B.A.ByteValue = (byte)1;
            outgoing.A.A.B.B.ByteValue = (byte)1;
            // entity id
            outgoing.A.B.A.Int16ValueLE = 4;
            // structure
            outgoing.B.Int64ValueLE = 0x61;

            Memory<byte> buffer = new byte[BlockHeader.HeaderSize];
            bool written = outgoing.TryWrite(buffer.Span);
            written.ShouldBeTrue();

            BlockHeader incoming = BlockHeader.ParseFrom(buffer);
            incoming.SignatureBits.ShouldBe(0x01015f7c);
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
            var buffer = orig.GetBuffers().Compact();
            buffer.Length.ShouldBe(48);

            buffer.Span[0].ShouldBe((byte)'|');  // marker byte 0
            buffer.Span[1].ShouldBe((byte)'_');  // marker byte 1
            buffer.Span[2].ShouldBe((byte)1);    // major version
            buffer.Span[3].ShouldBe((byte)1);    // minor version

            BlockHeader parsed = BlockHeader.ParseFrom(buffer);
            parsed.SignatureBits.ShouldBe(0x01015f7c);
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

            var orig = new T_ImplNameSpace_.T_EntityImplName_();
            orig.T_RequiredNativeStructMemberName_ = 123;
            //orig.T_NullableNativeStructMemberName_ = 456;
            orig.T_RequiredCustomStructMemberName_ = DayOfWeek.Monday;
            //orig.T_NullableCustomStructMemberName_ = DayOfWeek.Thursday;
            orig.T_RequiredFixLenStringMemberName_ = "abc";
            orig.T_NullableFixLenStringMemberName_ = "def";
            orig.T_RequiredVarLenBinaryMemberName_ = largeBinary;
            orig.T_NullableFixLenBinaryMemberName_ = smallBinary;
            orig.T_RequiredEntityMemberName_ = new T_MemberTypeImplSpace_.T_MemberTypeImplName_();
            orig.T_NullableEntityMemberName_ = new T_MemberTypeImplSpace_.T_MemberTypeImplName_();
            //orig.Freeze();
            await orig.Pack(dataStore);

            var copy = new T_ImplNameSpace_.T_EntityImplName_(orig);
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
            //orig.T_NullableNativeStructMemberName_ = 456;
            orig.T_RequiredCustomStructMemberName_ = DayOfWeek.Monday;
            //orig.T_NullableCustomStructMemberName_ = DayOfWeek.Thursday;
            orig.T_RequiredFixLenStringMemberName_ = "abc";
            orig.T_RequiredVarLenStringMemberName_ = "def";
            orig.T_NullableFixLenStringMemberName_ = "ghi";
            orig.T_NullableVarLenStringMemberName_ = null;
            orig.T_RequiredFixLenBinaryMemberName_ = smallBinary;
            orig.T_RequiredVarLenBinaryMemberName_ = largeBinary;
            orig.T_NullableFixLenBinaryMemberName_ = null;
            orig.T_NullableVarLenBinaryMemberName_ = largeBinary;
            orig.T_RequiredEntityMemberName_ = new T_MemberTypeImplSpace_.T_MemberTypeImplName_();
            orig.T_NullableEntityMemberName_ = new T_MemberTypeImplSpace_.T_MemberTypeImplName_();
            await orig.Pack(dataStore);
            orig.Freeze();

            var buffers = orig.GetBuffers();
            var copy = T_ImplNameSpace_.T_EntityImplName_.DeserializeFrom(buffers);
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
            //orig.T_NullableNativeStructMemberName_ = 456;
            orig.T_RequiredCustomStructMemberName_ = DayOfWeek.Monday;
            //orig.T_NullableCustomStructMemberName_ = DayOfWeek.Thursday;
            orig.T_RequiredFixLenStringMemberName_ = "abc";
            orig.T_RequiredVarLenStringMemberName_ = "def";
            orig.T_NullableFixLenStringMemberName_ = "ghi";
            orig.T_NullableVarLenStringMemberName_ = null;
            orig.T_RequiredFixLenBinaryMemberName_ = smallBinary;
            orig.T_RequiredVarLenBinaryMemberName_ = largeBinary;
            orig.T_NullableFixLenBinaryMemberName_ = null;
            orig.T_NullableVarLenBinaryMemberName_ = largeBinary;
            orig.T_RequiredEntityMemberName_ = new T_MemberTypeImplSpace_.T_MemberTypeImplName_();
            orig.T_NullableEntityMemberName_ = new T_MemberTypeImplSpace_.T_MemberTypeImplName_();
            await orig.Pack(dataStore);
            orig.Freeze();

            var buffers = orig.GetBuffers();
            var recd = T_BaseImplNameSpace_.T_BaseImplName_.DeserializeFrom(buffers);
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