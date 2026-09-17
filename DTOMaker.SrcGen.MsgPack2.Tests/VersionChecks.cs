using Shouldly;
using System;
using Xunit;

namespace DTOMaker.SrcGen.MsgPack2.Tests;

public class VersionChecks
{

    [Fact]
    public void MessagePackVersionCheck()
    {
        Version version = typeof(MessagePack.MessagePackSerializer).Assembly.GetName().Version ?? new Version(0, 0, 0);

        version.Major.ShouldBe(3);
        version.ToString().ShouldBe("3.1.8.0");
    }

    [Fact]
    public void RoslynCSharpVersionCheck()
    {
        Version version = typeof(Microsoft.CodeAnalysis.CSharp.LanguageVersion).Assembly.GetName().Version ?? new Version(0, 0, 0);

        version.Major.ShouldBe(5);
        version.ToString().ShouldBe("5.9.0.0");
    }

}