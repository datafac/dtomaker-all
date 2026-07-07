using Shouldly;
using System;
using Xunit;

namespace DTOMaker.SrcGen.JsonNewtonSoft.Tests
{
    public class VersionChecks
    {
        [Fact]
        public void RoslynCSharpVersionCheck()
        {
            Version version = typeof(Microsoft.CodeAnalysis.CSharp.LanguageVersion).Assembly.GetName().Version ?? new Version(0, 0, 0);

            version.Major.ShouldBe(5);
            version.Minor.ShouldBe(6);
            version.ToString().ShouldBe("5.6.0.0");
        }

    }
}
