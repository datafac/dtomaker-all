using DTOMaker.Models;
using DTOMaker.Runtime;
using DTOMaker.Runtime.MsgPack2;
using DTOMaker.SrcGen.MsgPack2.IntTests.MsgPack2;
using Shouldly;
using System;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace DTOMaker.SrcGen.MsgPack2.IntTests;

[Entity]
[Id(11)]
public interface ISimpleDTO_String : IEntityBase
{
    [Member(1)] String Field1 { get; set; }
    [Member(2)] String? Field2 { get; set; }
}

public class RoundtripBasicTypeTests_String
{
    public string Roundtrip_String(String reqValue, String? optValue)
    {
        var orig = new SimpleDTO_String { Field1 = reqValue, Field2 = optValue };
        orig.Freeze();
        orig.Field1.ShouldBe(reqValue);
        orig.Field2.ShouldBe(optValue);
        ReadOnlyMemory<byte> buffer = orig.SerializeToMessagePack();
        var copy = buffer.DeserializeFromMessagePack<SimpleDTO_String>();
        copy.ShouldNotBeNull();
        copy.ShouldBe(orig);
        copy.Field1.ShouldBe(reqValue);
        return buffer.Span.ToDisplay();
    }

    [Fact] public async Task Roundtrip_String_Defaults() => await Verifier.Verify(Roundtrip_String(string.Empty, null));
    [Fact] public async Task Roundtrip_String_UnitVals() => await Verifier.Verify(Roundtrip_String("abc", "def"));

}
