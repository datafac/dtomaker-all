using DTOMaker.TestHelpers;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace DTOMaker.SrcGen.MsgPack2.Tests;

public class ConverterTests_Complex
{
    private static readonly string modelSource =
        """
        using System;
        using System.Numerics;
        using DataFac.Memory;
        using DTOMaker.Models;
        namespace MyOrg.Models
        {
            [Entity(1)]
            public interface IMyDTO : IEntityBase
            {
                [Member(1, NativeType.PairOfInt64, typeof(DTOMaker.Models.ComplexConverter))] Complex Field1 { get; set; }
            }
        }
        """;

    [Fact] public void CustomSrcGen_GeneratedSourcesLength() => new SourceGenerator().GenerateAndCheckLength(modelSource, 3);
    [Fact] public async Task CustomSrcGen_VerifyGeneratedSource0() => await Verifier.Verify(new SourceGenerator().GenerateAndGetOutput(modelSource, 0, "MyOrg.Models.Global.g.cs"));
    [Fact] public async Task CustomSrcGen_VerifyGeneratedSource1() => await Verifier.Verify(new SourceGenerator().GenerateAndGetOutput(modelSource, 1, "MyOrg.Models.MsgPack2.Domain.g.cs"));
    [Fact] public async Task CustomSrcGen_VerifyGeneratedSource2() => await Verifier.Verify(new SourceGenerator().GenerateAndGetOutput(modelSource, 2, "MyOrg.Models.MsgPack2.MyDTO.g.cs"));
}