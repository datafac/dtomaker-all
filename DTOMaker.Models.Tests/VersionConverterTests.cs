using Microsoft.Testing.Platform.Extensions.ArtifactPostProcessing;
using Shouldly;
using System;
using Xunit;

namespace DTOMaker.Models.Tests;

public class VersionConverterTests
{
    [Theory]
    [InlineData("0.0")]
    [InlineData("1.0")]
    [InlineData("1.2")]
    [InlineData("1.2.3")]
    [InlineData("1.2.3.4")]
    public void ValidRoundtrip(string input)
    {
        Version custom = VersionConverter.ToCustom(input);
        custom.ToString().ShouldBe(input);
        string native = VersionConverter.ToNative(custom);
        native.ShouldBe(input);
    }

    [Theory]
    [InlineData("")]
    [InlineData("invalid")]
    [InlineData("1.2.3.4.5")]
    public void InvalidRoundtrip(string native)
    {
        Version custom = VersionConverter.ToCustom(native);
        custom.ToString().ShouldNotBe(native);
        custom.ToString().ShouldBe("0.0");
    }

    [Fact]
    public void RoundtripEmptyA()
    {
        Version custom = new Version();
        custom.ToString().ShouldBe("0.0");
        string native = VersionConverter.ToNative(custom);
        native.ShouldBe("0.0");
    }
}
