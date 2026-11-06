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

internal interface IDebugDTO : IEntityBase
{
    Version? MyVersion { get; set; }
}

internal sealed class DebugDTO : EntityBase, IDebugDTO
{
    public const string Field_01_001 = "_f01_001";

    [JsonPropertyName(Field_01_001)]
    public string? _native_value
    {
        get;
        set;
    }

    [JsonIgnore]
    public Version? MyVersion
    {
        get => _native_value is null ? null : new Version(_native_value);
        set => _native_value = value?.ToString();
    }

    protected override int OnGetEntityId() => 1;
    protected override IEntityBase OnShallowCopy() => new DebugDTO() { _native_value = this._native_value };
}

public class DebugDTOTests
{
    [Theory]
    [InlineData("1.0")]
    [InlineData("2.0.0")]
    [InlineData("3.0.0.0")]
    public void DebugDTO_Serialize(string value)
    {
        var orig = new DebugDTO();
        orig.MyVersion = new Version(value);
        var json = orig.SerializeToJson<DebugDTO>();
        json.ShouldContain(DebugDTO.Field_01_001);
        json.ShouldContain(value);
    }

    [Theory]
    [InlineData("1.0")]
    [InlineData("2.0.0")]
    [InlineData("3.0.0.0")]
    public void DebugDTO_Deserialize(string value)
    {
        var orig = new DebugDTO();
        orig.MyVersion = new Version(value);
        var json = orig.SerializeToJson();
        var copy = json.DeserializeFromJson<DebugDTO>();
        copy.ShouldNotBeNull();
        copy._native_value.ShouldBe(value);
        copy.MyVersion.ShouldBe(orig.MyVersion);
        copy.Equals(orig).ShouldBeTrue();
        copy.ShouldBe(orig);
    }
}