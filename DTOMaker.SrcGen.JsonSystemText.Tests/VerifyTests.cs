using Shouldly;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using VerifyTests;
using VerifyXunit;
using Xunit;

namespace DTOMaker.SrcGen.JsonSystemText.Tests;

public class VerifyTests
{
    [Fact]
    public async Task RunVerifyChecks()
    {
        await VerifyChecks.Run();
    }
}
