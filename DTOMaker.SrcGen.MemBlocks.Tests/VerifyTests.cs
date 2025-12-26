using Shouldly;
using System;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace DTOMaker.SrcGen.MemBlocks.Tests
{
    public class VerifyTests
    {
        [Fact]
        public async Task RunVerifyChecks()
        {
            await VerifyChecks.Run();
        }
    }
}