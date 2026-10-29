using DTOMaker.Converters.Numerics;
using DTOMaker.Models;
using DTOMaker.Runtime.JsonSystemText;
using DTOMaker.SrcGen.JsonSystemText.IntTests.JsonSystemText;
using Shouldly;
using System;
using System.Collections;
using System.Numerics;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace DTOMaker.SrcGen.JsonSystemText.IntTests;

[Entity(56)]
public interface ISimpleDTO_Version : IEntityBase { [Member(1, NativeType.String, typeof(VersionConverter))] Version Value { get; set; } }

public class RoundtripBasicTypeTests_Custom_Version
{
    public string Roundtrip_Version(Version reqValue)
    {
        ISimpleDTO_Version orig = new SimpleDTO_Version();
        orig.Value = reqValue;
        orig.Freeze();
        orig.Value.ShouldBe(reqValue);
        var json = orig.SerializeToJson();
        ISimpleDTO_Version? copy = json.DeserializeFromJson<SimpleDTO_Version>();
        copy.ShouldNotBeNull();
        copy.ShouldBe(orig);
        copy.Value.ShouldBe(reqValue);
        return json;
    }

    //[Fact] public async Task Roundtrip_Version_Defaults() => await Verifier.Verify(Roundtrip_Version(default));
    [Fact] public async Task Roundtrip_Version_Value0() => await Verifier.Verify(Roundtrip_Version(new Version()));
    [Fact] public async Task Roundtrip_Version_Value1() => await Verifier.Verify(Roundtrip_Version(new Version("1")));
    [Fact] public async Task Roundtrip_Version_Value2() => await Verifier.Verify(Roundtrip_Version(new Version(1, 2)));
    [Fact] public async Task Roundtrip_Version_Value3() => await Verifier.Verify(Roundtrip_Version(new Version(1, 2, 3)));
    [Fact] public async Task Roundtrip_Version_Value4() => await Verifier.Verify(Roundtrip_Version(new Version(1, 2, 3, 4)));

}

