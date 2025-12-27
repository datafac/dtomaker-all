using DataFac.Memory;
using DTOMaker.Models;
using DTOMaker.Runtime;
using DTOMaker.Runtime.MsgPack2;
using DTOMaker.SrcGen.MsgPack2.IntTests.MsgPack2;
using Shouldly;
using System;
using System.Text;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace DTOMaker.SrcGen.MsgPack2.IntTests;

[Entity(12)]
public interface ISimpleDTO_Octets : IEntityBase
{
    [Member(1)] Octets Field1 { get; set; }
    [Member(2)] Octets? Field2 { get; set; }
}

public class RoundtripBasicTypeTests_Octets
{
    public string Roundtrip_Octets(byte[] reqValue, byte[]? optValue)
    {
        var orig = new SimpleDTO_Octets { Field1 = reqValue, Field2 = optValue };
        orig.Freeze();
        orig.Field1.ShouldBe(reqValue);
        orig.Field2.ShouldBe(optValue);
        ReadOnlyMemory<byte> buffer = orig.SerializeToMessagePack();
        var copy = buffer.DeserializeFromMessagePack<SimpleDTO_Octets>();
        copy.ShouldNotBeNull();
        copy.ShouldBe(orig);
        copy.Field1.ShouldBe(reqValue);
        return buffer.Span.ToDisplay();
    }

    [Fact] public async Task Roundtrip_Octets_Defaults() => await Verifier.Verify(Roundtrip_Octets([], null));
    [Fact]
    public async Task Roundtrip_Octets_UnitVals() => await Verifier.Verify(Roundtrip_Octets(
            Encoding.UTF8.GetBytes("abc"), Encoding.UTF8.GetBytes("def")));

}
