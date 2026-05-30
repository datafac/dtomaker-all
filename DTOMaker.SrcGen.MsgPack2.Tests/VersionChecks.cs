using Shouldly;
using System;
using Xunit;

namespace DTOMaker.SrcGen.MsgPack2.Tests
{
    public class VersionChecks
    {

        [Fact]
        public void MessagePackVersionCheck()
        {
            Version version = typeof(MessagePack.MessagePackSerializer).Assembly.GetName().Version ?? new Version(0, 0, 0);

            version.Major.ShouldBe(2);
            version.ToString().ShouldBe("2.5.0.0");
        }

        [Fact]
        public void RoslynCSharpVersionCheck()
        {
            Version version = typeof(Microsoft.CodeAnalysis.CSharp.LanguageVersion).Assembly.GetName().Version ?? new Version(0, 0, 0);

            version.Major.ShouldBe(5);
            version.Minor.ShouldBe(3);
            version.ToString().ShouldBe("5.3.0.0");
        }

    }
}