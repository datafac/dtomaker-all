using DTOMaker.TestHelpers;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace DTOMaker.SrcGen.JsonNewtonSoft.Tests;

public class ConverterTests_Enum
{
    private static readonly string modelSource =
        """
        using System;
        using DataFac.Memory;
        using DTOMaker.Models;
        namespace MyOrg.Models
        {
            [Entity(1)]
            public interface IMyDTO : IEntityBase
            {
                [Member(1, NativeType.Byte, typeof(DTOMaker.Models.DayOfWeekConverter))] DayOfWeek Field1 { get; set; }
                [Member(2, NativeType.Byte, typeof(DTOMaker.Models.DayOfWeekConverter))] DayOfWeek? Field2 { get; set; }
            }
        }
        """;

    [Fact] public void CustomSrcGen_GeneratedSourcesLength() => new SourceGenerator().GenerateAndCheckLength(modelSource, 3);
    [Fact] public async Task CustomSrcGen_VerifyGeneratedSource1() => await Verifier.Verify(new SourceGenerator().GenerateAndGetOutput(modelSource, 0, "MyOrg.Models.Global.g.cs"));
    [Fact] public async Task CustomSrcGen_VerifyGeneratedSource2() => await Verifier.Verify(new SourceGenerator().GenerateAndGetOutput(modelSource, 1, "MyOrg.Models.JsonNewtonSoft.Domain.g.cs"));
    [Fact] public async Task CustomSrcGen_VerifyGeneratedSource3() => await Verifier.Verify(new SourceGenerator().GenerateAndGetOutput(modelSource, 2, "MyOrg.Models.JsonNewtonSoft.MyDTO.g.cs"));
}