using Shouldly;
using MessagePack;
using System;

#pragma warning disable CS0618 // Type or member is obsolete

using T_ImplNameSpace_;
using System.Linq;
using DTOMaker.Runtime.MsgPack2;
using DataFac.Memory;
using System.Security.Principal;

namespace Template_MessagePack.Tests
{
    public interface ISandboxDTO
    {
        int Id { get; set; }
        string? Name { get; set; }
        Octets Data1 { get; set; }
        Octets? Data2 { get; set; }
    }
    [MessagePackObject]
    public sealed class SanboxDTO : ISandboxDTO, IEquatable<SanboxDTO>
    {
        [Key(1)] public int Id { get; set; }
        [Key(2)] public string? Name { get; set; }

        [Key(3)] public ReadOnlyMemory<byte> _data1 { get; set; } = ReadOnlyMemory<byte>.Empty;
        [IgnoreMember]
        public Octets Data1
        {
            get { return Octets.UnsafeWrap( _data1); }
            set { _data1 = value.AsMemory(); }
        }

        [Key(4)] public ReadOnlyMemory<byte>? _data2 { get; set; } = null;
        [IgnoreMember]
        public Octets? Data2
        {
            get { return _data2.HasValue ? Octets.UnsafeWrap(_data2.Value) : null; }
            set { _data2 = value is not null ? value.AsMemory() : null; }
        }

        public bool Equals(SanboxDTO? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            if (Id != other.Id) return false;
            if (Name != other.Name) return false;
            if (!_data1.Span.SequenceEqual(other._data1.Span)) return false;
            if (_data2 is null)
            {
                if (other._data2 is not null) return false;
            }
            else
            {
                if (other._data2 is null) return false;
                if (!_data2.Value.Span.SequenceEqual(other._data2.Value.Span)) return false;
            }
            return true;
        }
        public override bool Equals(object? obj) => obj is SanboxDTO other && Equals(other);
        public override int GetHashCode()
        {
            HashCode hasher = new HashCode();
            hasher.Add(Id);
            hasher.Add(Name);
#if NET8_0_OR_GREATER
            hasher.AddBytes(_data1.Span);
            if (_data2.HasValue)
            {
                hasher.Add(_data2.Value.Length);
                hasher.AddBytes(_data2.Value.Span);
            }
#else
            {
                var span1 = _data1.Span;
                for (int i = 0; i < span1.Length; i++)
                {
                    hasher.Add(span1[i]);
                }
            }
            if (_data2.HasValue)
            {
                var span2 = _data2.Value.Span;
                hasher.Add(span2.Length);
                for (int i = 0; i < span2.Length; i++)
                {
                    hasher.Add(span2[i]);
                }
            }
#endif
            return hasher.ToHashCode();
        }
    }
    public class SandboxTests
    {
        [Fact]
        public void Roundtrip()
        {
            var orig = new SanboxDTO();
            orig.Id = 123;
            orig.Name = "abc";
            orig.Data1 = new Octets(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            orig.Data2 = new Octets(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 });

            ReadOnlyMemory<byte> buffer = MessagePackSerializer.Serialize(orig);
            var copy  = MessagePackSerializer.Deserialize<SanboxDTO>(buffer);

            copy.ShouldNotBeNull();
            copy.Equals(orig).ShouldBeTrue();
            copy.ShouldBe(orig);
            copy.GetHashCode().ShouldBe(orig.GetHashCode());
        }
    }
    public class RoundtripTests
    {
        private static readonly Octets smallBinary = new Octets(new byte[] { 1, 2, 3, 4, 5, 6, 7 });
        private static readonly Octets largeBinary = new Octets(Enumerable.Range(0, 256).Select(i => (byte)i).ToArray());

        [Fact]
        public void Roundtrip01AsEntity()
        {
            var orig = new T_ImplNameSpace_.T_ConcreteEntity_();
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

            ReadOnlyMemory<byte> buffer = orig.SerializeToMessagePack<T_ConcreteEntity_>();
            var copy = buffer.DeserializeFromMessagePack<T_ConcreteEntity_>();

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
            var orig = new T_ConcreteEntity_();
            orig.BaseField1 = 321;
            orig.T_RequiredNativeStructMemberName_ = 123;
            orig.T_NullableNativeStructMemberName_ = 456;
            orig.T_RequiredCustomStructMemberName_ = DayOfWeek.Monday;
            orig.T_NullableCustomStructMemberName_ = DayOfWeek.Thursday;
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
            copy.Freeze();
            copy.IsFrozen.ShouldBeTrue();
            copy.Equals(orig).ShouldBeTrue();
            copy.ShouldBe(orig);
            copy.GetHashCode().ShouldBe(orig.GetHashCode());
        }
    }
}