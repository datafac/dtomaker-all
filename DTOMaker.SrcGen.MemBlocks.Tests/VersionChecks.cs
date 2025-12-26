using Shouldly;
using System;
using Xunit;

namespace DTOMaker.SrcGen.MemBlocks.Tests
{
    public class VersionChecks
    {
        /// <summary>
        /// Some build pipelines have not yet updated to use Roslyn 5.0 or later.
        /// This test ensures that the expected Roslyn version is being used.
        /// </summary>
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