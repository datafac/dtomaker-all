using DTOMaker.Runtime.JsonSystemText;
using Shouldly;
using System;
using System.Linq;
using T_ImplNameSpace_;
using Xunit;

#pragma warning disable CS0618 // Type or member is obsolete

namespace Template.JsonSystemText.Tests
{
    public class RoundtripTests
    {
        [Fact]
        public void Roundtrip01AsEntity()
        {
            byte[] smallBinary = new byte[] { 1, 2, 3, 4, 5, 6, 7 };
            byte[] largeBinary = Enumerable.Range(0, 256).Select(i => (byte)i).ToArray();

            var orig = new T_EntityImplName_();
            orig.BaseField1 = 321;
            orig.T_RequiredScalarMemberName_ = 123;
            //todo orig.T_RequiredEntityMemberName_ = new T_MemberTypeImplSpace_.T_MemberTypeImplName_() { Field1 = 456L };
            orig.T_RequiredBinaryMemberName_ = largeBinary;
            orig.T_NullableBinaryMemberName_ = smallBinary;
            orig.Freeze();

            string buffer = orig.SerializeToJson<T_EntityImplName_>();
            var copy = buffer.DeserializeFromJson<T_EntityImplName_>();

            copy.ShouldNotBeNull();
            copy.Freeze();
            copy.IsFrozen.ShouldBeTrue();
            copy.BaseField1!.ShouldBe(orig.BaseField1);
            copy.T_RequiredScalarMemberName_.ShouldBe(orig.T_RequiredScalarMemberName_);
            copy.T_RequiredBinaryMemberName_.AsSpan().SequenceEqual(orig.T_RequiredBinaryMemberName_.AsSpan()).ShouldBeTrue();
            copy.Equals(orig).ShouldBeTrue();
            //copy.ShouldBe(orig);
            copy.GetHashCode().ShouldBe(orig.GetHashCode());
        }

        [Fact]
        public void Roundtrip03AsBase()
        {
            byte[] smallBinary = new byte[] { 1, 2, 3, 4, 5, 6, 7 };
            byte[] largeBinary = Enumerable.Range(0, 256).Select(i => (byte)i).ToArray();

            var orig = new T_EntityImplName_();
            orig.BaseField1 = 321;
            orig.T_RequiredScalarMemberName_ = 123;
            orig.T_RequiredBinaryMemberName_ = largeBinary;
            orig.T_NullableBinaryMemberName_ = smallBinary;
            orig.Freeze();

            string buffer = orig.SerializeToJson<T_BaseImplNameSpace_.T_BaseImplName_>();
            var recd = buffer.DeserializeFromJson<T_BaseImplNameSpace_.T_BaseImplName_>();

            recd.ShouldNotBeNull();
            recd.ShouldBeOfType<T_EntityImplName_>();
            recd.Freeze();
            var copy = recd as T_EntityImplName_;
            copy.ShouldNotBeNull();
            copy!.IsFrozen.ShouldBeTrue();
            copy.BaseField1!.ShouldBe(orig.BaseField1);
            copy.T_RequiredScalarMemberName_.ShouldBe(orig.T_RequiredScalarMemberName_);
            copy.T_RequiredBinaryMemberName_.AsSpan().SequenceEqual(orig.T_RequiredBinaryMemberName_.AsSpan()).ShouldBeTrue();
            copy.ShouldBe(orig);
            copy.GetHashCode().ShouldBe(orig.GetHashCode());
        }
    }
}