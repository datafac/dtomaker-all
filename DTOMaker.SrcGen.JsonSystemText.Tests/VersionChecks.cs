using Shouldly;
using System;
using Xunit;

namespace DTOMaker.SrcGen.JsonSystemText.Tests
{
    public class VersionChecks
    {
        [Fact]
        public void RoslynCSharpVersionCheck()
        {
            Version version = typeof(Microsoft.CodeAnalysis.CSharp.LanguageVersion).Assembly.GetName().Version ?? new Version(0, 0, 0);

            version.Major.ShouldBe(4);
            version.Minor.ShouldBe(14);
            version.ToString().ShouldBe("4.14.0.0");
        }

    }
}
