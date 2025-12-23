using Shouldly;
using System;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace DTOMaker.SrcGen.MsgPack2.Tests
{
    public class VerifyTests
    {
        [Fact]
        public async Task RunVerifyChecks()
        {
            await VerifyChecks.Run();
        }

        [Fact]
        public void VersionChecks()
        {
            Version version = typeof(MessagePack.MessagePackSerializer).Assembly.GetName().Version ?? new Version(0, 0, 0);

            version.Major.ShouldBe(2);
            version.ToString().ShouldBe("2.5.0.0");
        }

    }
}