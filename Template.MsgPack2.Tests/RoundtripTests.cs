using Shouldly;
using MessagePack;
using System;

#pragma warning disable CS0618 // Type or member is obsolete

using T_ImplNameSpace_;
using System.Linq;
using DTOMaker.Runtime.MsgPack2;

namespace Template_MessagePack.Tests
{
    public class RoundtripTests
    {
        [Fact]
        public void Roundtrip01AsEntity()
        {
            ReadOnlyMemory<byte> smallBinary = new byte[] { 1, 2, 3, 4, 5, 6, 7 };
            ReadOnlyMemory<byte> largeBinary = new ReadOnlyMemory<byte>(Enumerable.Range(0, 256).Select(i => (byte)i).ToArray());

            var orig = new T_ConcreteEntity_();
            orig.BaseField1 = 321;
            orig.T_RequiredScalarMemberName_ = 123;
            orig.T_RequiredEntityMemberName_ = new T_MemberTypeImplSpace_.T_MemberTypeImplName_() { Field1 = 456L };
            orig.T_RequiredBinaryMemberName_ = largeBinary;
            orig.T_NullableBinaryMemberName_ = smallBinary;
            orig.Freeze();

            ReadOnlyMemory<byte> buffer = orig.SerializeToMessagePack<T_ConcreteEntity_>();
            var copy = buffer.DeserializeFromMessagePack<T_ConcreteEntity_>();

            copy.Freeze();
            copy.IsFrozen.ShouldBeTrue();
            copy.BaseField1!.ShouldBe(orig.BaseField1);
            copy.T_RequiredScalarMemberName_.ShouldBe(orig.T_RequiredScalarMemberName_);
            copy.T_RequiredEntityMemberName_.Field1.ShouldBe(orig.T_RequiredEntityMemberName_.Field1);
            copy.T_RequiredBinaryMemberName_.Span.SequenceEqual(orig.T_RequiredBinaryMemberName_.Span).ShouldBeTrue();
            copy.ShouldBe(orig);
            copy.GetHashCode().ShouldBe(orig.GetHashCode());
        }

        [Fact]
        public void Roundtrip03AsBase()
        {
            ReadOnlyMemory<byte> smallBinary = new byte[] { 1, 2, 3, 4, 5, 6, 7 };
            ReadOnlyMemory<byte> largeBinary = new ReadOnlyMemory<byte>(Enumerable.Range(0, 256).Select(i => (byte)i).ToArray());

            var orig = new T_ConcreteEntity_();
            orig.BaseField1 = 321;
            orig.T_RequiredScalarMemberName_ = 123;
            orig.T_RequiredEntityMemberName_ = new T_MemberTypeImplSpace_.T_MemberTypeImplName_() { Field1 = 456L };
            orig.T_RequiredBinaryMemberName_ = largeBinary;
            orig.T_NullableBinaryMemberName_ = smallBinary;
            orig.Freeze();

            ReadOnlyMemory<byte> buffer = orig.SerializeToMessagePack<T_BaseImplNameSpace_.T_BaseImplName_>();
            var recd = buffer.DeserializeFromMessagePack<T_BaseImplNameSpace_.T_BaseImplName_>();

            recd.ShouldNotBeNull();
            recd.ShouldBeOfType<T_ConcreteEntity_>();
            recd.Freeze();
            var copy = recd as T_ConcreteEntity_;
            copy.ShouldNotBeNull();
            copy!.IsFrozen.ShouldBeTrue();
            copy.BaseField1!.ShouldBe(orig.BaseField1);
            copy.T_RequiredScalarMemberName_.ShouldBe(orig.T_RequiredScalarMemberName_);
            copy.T_RequiredBinaryMemberName_.Span.SequenceEqual(orig.T_RequiredBinaryMemberName_.Span).ShouldBeTrue();
            copy.ShouldBe(orig);
            copy.GetHashCode().ShouldBe(orig.GetHashCode());
        }
    }
}