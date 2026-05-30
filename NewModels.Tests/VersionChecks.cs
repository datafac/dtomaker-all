using Shouldly;
using System;
using Xunit;

namespace NewModels.Tests
{
    public class VersionChecks
    {
        [Fact]
        public void MessagePackVersionCheck()
        {
            Version version = typeof(MessagePack.MessagePackSerializer).Assembly.GetName().Version ?? new Version(0, 0, 0);
            version.Major.ShouldBe(3);
            version.ToString().ShouldBe("3.1.6.0");
        }
    }
}
