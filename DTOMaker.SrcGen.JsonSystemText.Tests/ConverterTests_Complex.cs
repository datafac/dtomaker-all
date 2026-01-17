using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace DTOMaker.SrcGen.JsonSystemText.Tests
{
    public class ConverterTests_Complex
    {
        private static readonly string modelSource =
            """
            using System;
            using System.Numerics;
            using DataFac.Memory;
            using DTOMaker.Models;
            using DTOMaker.Runtime;
            using DTOMaker.Runtime.Converters;
            namespace MyOrg.Models
            {
                [Entity(1)]
                public interface IMyDTO : IEntityBase
                {
                    [Member(1, NativeType.PairOfInt64, "DTOMaker.Runtime.Converters.ComplexConverter")] Complex Field1 { get; set; }
                }
            }
            """;

        [Fact] public void CustomSrcGen_GeneratedSourcesLength() => modelSource.GenerateAndCheckLength(1);
        [Fact] public async Task CustomSrcGen_VerifyGeneratedSource0() => await Verifier.Verify(modelSource.GenerateAndGetOutput(0, "MyOrg.Models.JsonSystemText.MyDTO.g.cs"));
    }
}
