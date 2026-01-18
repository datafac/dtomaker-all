using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace DTOMaker.SrcGen.MemBlocks.Tests;

public class ConverterTests_Enum
{
    private static readonly string modelSource =
        """
            using System;
            using DataFac.Memory;
            using DTOMaker.Models;
            namespace MyOrg.Models
            {
                [Entity(1, LayoutMethod.Linear)]
                public interface IMyDTO : IEntityBase
                {
                    [Member(1, NativeType.Int32, typeof(DTOMaker.Models.DayOfWeekConverter))] DayOfWeek Field1 { get; set; }
                }
            }
            """;

    [Fact] public void CustomSrcGen_GeneratedSourcesLength() => modelSource.GenerateAndCheckLength(1);
    [Fact] public async Task CustomSrcGen_VerifyGeneratedSource0() => await Verifier.Verify(modelSource.GenerateAndGetOutput(0, "MyOrg.Models.MemBlocks.MyDTO.g.cs"));
}
