using DataFac.Memory;
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
        private static readonly Octets smallBinary = new Octets(new byte[] { 1, 2, 3, 4, 5, 6, 7 });
        private static readonly Octets largeBinary = new Octets(Enumerable.Range(0, 256).Select(i => (byte)i).ToArray());

        [Fact]
        public void Roundtrip01AsEntity()
        {
            var orig = new T_ImplNameSpace_.T_EntityImplName_();
            orig.T_RequiredNativeStructMemberName_ = 123;
            orig.T_NullableNativeStructMemberName_ = 456;
            orig.T_RequiredCustomStructMemberName_ = DayOfWeek.Monday;
            orig.T_NullableCustomStructMemberName_ = DayOfWeek.Thursday;
            orig.T_RequiredStringMemberName_ = "abc";
            orig.T_NullableStringMemberName_ = "def";
            orig.T_RequiredBinaryMemberName_ = largeBinary;
            orig.T_NullableBinaryMemberName_ = smallBinary;
            orig.T_RequiredEntityMemberName_ = new T_MemberTypeImplSpace_.T_MemberTypeImplName_();
            orig.T_NullableEntityMemberName_ = new T_MemberTypeImplSpace_.T_MemberTypeImplName_();
            orig.Freeze();

            string buffer = orig.SerializeToJson<T_EntityImplName_>();
            var copy = buffer.DeserializeFromJson<T_EntityImplName_>();

            copy.ShouldNotBeNull();
            copy.Freeze();
            copy.IsFrozen.ShouldBeTrue();
            copy.Equals(orig).ShouldBeTrue();
            copy.ShouldBe(orig);
            copy.GetHashCode().ShouldBe(orig.GetHashCode());
        }

        [Fact]
        public void Roundtrip03AsBase()
        {
            var impl = new T_ImplNameSpace_.T_EntityImplName_();
            T_IntfNameSpace_.T_EntityIntfName_ orig = impl;
            orig.T_RequiredNativeStructMemberName_ = 123;
            orig.T_NullableNativeStructMemberName_ = 456;
            orig.T_RequiredCustomStructMemberName_ = DayOfWeek.Monday;
            orig.T_NullableCustomStructMemberName_ = DayOfWeek.Thursday;
            orig.T_RequiredStringMemberName_ = "abc";
            orig.T_NullableStringMemberName_ = "def";
            orig.T_RequiredBinaryMemberName_ = largeBinary;
            orig.T_NullableBinaryMemberName_ = smallBinary;
            orig.T_RequiredEntityMemberName_ = new T_MemberTypeImplSpace_.T_MemberTypeImplName_();
            orig.T_NullableEntityMemberName_ = new T_MemberTypeImplSpace_.T_MemberTypeImplName_();
            orig.Freeze();

            string buffer = impl.SerializeToJson<T_BaseImplNameSpace_.T_BaseImplName_>();
            var recd = buffer.DeserializeFromJson<T_BaseImplNameSpace_.T_BaseImplName_>();

            recd.ShouldNotBeNull();
            recd.ShouldBeOfType<T_EntityImplName_>();
            recd.Freeze();
            var copy = recd as T_EntityImplName_;
            copy.ShouldNotBeNull();
            copy.IsFrozen.ShouldBeTrue();
            copy.ShouldBe(orig);
            copy.GetHashCode().ShouldBe(orig.GetHashCode());
        }
    }
}