using Shouldly;
using System;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace DTOMaker.SrcGen.JsonNewtonSoft.Tests
{
    public class VerifyTests
    {
        [Fact]
        public async Task RunVerifyChecks()
        {
            await VerifyChecks.Run();
        }

        [Fact]
        public async Task CheckVerifyVersion()
        {
            // check we are pinned to V31.20
            var fileVersion = System.Diagnostics.FileVersionInfo.GetVersionInfo(typeof(VerifyChecks).Assembly.Location);
            fileVersion.ShouldNotBeNull();
            fileVersion.ProductVersion.ShouldStartWith("31.20.");

            Version version = typeof(VerifyChecks).Assembly.GetName().Version ?? new Version(0, 0, 0);
            version.ToString().ShouldBe("1.0.0.0"); // todo ShouldBe("31.20.0.0");
        }
    }
}
