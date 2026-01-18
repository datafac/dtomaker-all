using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace DTOMaker.SrcGen.MemBlocks.Tests;

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
                [Entity(1, LayoutMethod.Linear)]
                public interface IMyDTO : IEntityBase
                {
                    [Member(1, NativeType.PairOfInt64, typeof(DTOMaker.Models.ComplexConverter))] Complex Field1 { get; set; }
                }
            }
            """;

    [Fact] public void CustomSrcGen_GeneratedSourcesLength() => modelSource.GenerateAndCheckLength(1);
    [Fact] public async Task CustomSrcGen_VerifyGeneratedSource0() => await Verifier.Verify(modelSource.GenerateAndGetOutput(0, "MyOrg.Models.MemBlocks.MyDTO.g.cs"));
}
