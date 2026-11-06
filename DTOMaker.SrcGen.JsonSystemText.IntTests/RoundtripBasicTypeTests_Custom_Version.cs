using DTOMaker.Models;
using DTOMaker.Runtime.JsonSystemText;
using DTOMaker.SrcGen.JsonSystemText.IntTests.JsonSystemText;
using Shouldly;
using System;
using System.Text.Json.Serialization;
using Xunit;

namespace DTOMaker.SrcGen.JsonSystemText.IntTests;

[Entity(56)]
public interface ISimpleDTO_Version : IEntityBase { [Member(1, NativeType.String, typeof(VersionConverter))] Version Value { get; set; } }

public class RoundtripBasicTypeTests_Custom_Version
{
    private void Roundtrip_Version(Version reqValue)
    {
        var orig = new SimpleDTO_Version();
        orig.Value = reqValue;
        orig.Freeze();
        orig.Value.ShouldBe(reqValue);
        var json = orig.SerializeToJson();
        var copy = json.DeserializeFromJson<SimpleDTO_Version>();

        copy.ShouldNotBeNull();
        copy.Value.ShouldBe(reqValue);
        copy.Equals(orig).ShouldBeTrue();
        copy.ShouldBe(orig);
        int origHash = orig.GetHashCode();
        int copyHash = copy.GetHashCode();
        copyHash.ShouldBe(origHash);

        ISimpleDTO_Version orig2 = orig;
        ISimpleDTO_Version copy2 = copy;    
        copy2.Value.ShouldBe(orig2.Value);
    }

    //[Fact] public async Task Roundtrip_Version_Defaults() => await Verifier.Verify(Roundtrip_Version(default));
    [Fact] public void Roundtrip_Version_Value0() => Roundtrip_Version(new Version());
    [Fact] public void Roundtrip_Version_Value1() => Roundtrip_Version(new Version("1.0"));
    [Fact] public void Roundtrip_Version_Value2() => Roundtrip_Version(new Version(1, 2));
    [Fact] public void Roundtrip_Version_Value3() => Roundtrip_Version(new Version(1, 2, 3));
    [Fact] public void Roundtrip_Version_Value4() => Roundtrip_Version(new Version(1, 2, 3, 4));

}
