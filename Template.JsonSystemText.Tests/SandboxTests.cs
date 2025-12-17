using DataFac.Memory;
using DTOMaker.Runtime.JsonSystemText;
using Shouldly;
using System;
using System.Linq;
using System.Text.Json.Serialization;
using Xunit;

namespace Template.JsonSystemText.Tests
{
    internal interface ISimple
    {
        int Field1 { get; }
        Octets Field2 { get; }
    }

    internal sealed class SimpleST : ISimple
    {
        [JsonPropertyName("fieldOne")]
        public int Field1 { get; set; }

        [JsonPropertyName("fieldTwo")]
        public byte[] Field2 { get; set; } = Array.Empty<byte>();

        Octets ISimple.Field2 => Octets.UnsafeWrap(Field2);
    }

    internal interface IParent
    {
        int Id { get; }
    }

    [JsonDerivedType(typeof(ParentST), 1)]
    [JsonDerivedType(typeof(Child1ST), 2)]
    internal class ParentST : IParent
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
    }

    internal interface IChild1 : IParent
    {
        string Name { get; }
    }

    internal sealed class Child1ST : ParentST, IChild1
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
    }

    public class SandboxTests
    {
        [Fact]
        public void RoundtripSimpleNS()
        {
            ReadOnlyMemory<byte> smallBinary = new byte[] { 1, 2, 3, 4, 5, 6, 7 };

            var orig = new SimpleST();
            orig.Field1 = 321;
            orig.Field2 = smallBinary.ToArray();

            string buffer = orig.SerializeToJson<SimpleST>();
            var copy = buffer.DeserializeFromJson<SimpleST>();

            copy.ShouldNotBeNull();

            ISimple iorig = orig;
            ISimple icopy = copy;
            icopy.Field1.ShouldBe(iorig.Field1);
            icopy.Field2.AsMemory().Span.SequenceEqual(iorig.Field2.AsMemory().Span).ShouldBeTrue();
        }

        [Fact]
        public void RoundtripNestedNSAsLeaf()
        {
            var orig = new Child1ST();
            orig.Id = 321;
            orig.Name = "Alice";

            string buffer = orig.SerializeToJson<Child1ST>();
            var copy = buffer.DeserializeFromJson<Child1ST>();

            copy.ShouldNotBeNull();

            IChild1 iorig = orig;
            IChild1 icopy = copy;
            icopy.Id.ShouldBe(iorig.Id);
            icopy.Name.ShouldBe(iorig.Name);
        }

        [Fact]
        public void RoundtripNestedNSAsRoot()
        {
            var orig = new Child1ST();
            orig.Id = 321;
            orig.Name = "Alice";

            string buffer = orig.SerializeToJson<ParentST>();
            var copy = buffer.DeserializeFromJson<ParentST>();

            copy.ShouldNotBeNull();
            copy.ShouldBeOfType<Child1ST>();

            IChild1 iorig = orig;
            IChild1? icopy = (copy as IChild1);
            icopy.ShouldNotBeNull();
            icopy.Id.ShouldBe(iorig.Id);
            icopy.Name.ShouldBe(iorig.Name);
        }
    }
}