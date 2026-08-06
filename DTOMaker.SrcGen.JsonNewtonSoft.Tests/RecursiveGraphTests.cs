using DTOMaker.TestHelpers;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace DTOMaker.SrcGen.JsonNewtonSoft.Tests
{
    public class RecursiveGraphTests
    {
        private static readonly string modelSource =
            """
            using System;
            using DTOMaker.Models;
            namespace MyOrg.Models
            {
                [Entity(1)]
                public interface IMyDTO : IEntityBase
                {
                    [Member(1)] IMyDTO? Field1 { get; set; }
                }
            }
            """;

        [Fact] public void EntitySrcGen_GeneratedSourcesLength() => new SourceGenerator().GenerateAndCheckLength(modelSource, 3);
        [Fact] public async Task EntitySrcGen_VerifyGeneratedSource0() => await Verifier.Verify(new SourceGenerator().GenerateAndGetOutput(modelSource, 0, "MyOrg.Models.Global.g.cs"));
        [Fact] public async Task EntitySrcGen_VerifyGeneratedSource1() => await Verifier.Verify(new SourceGenerator().GenerateAndGetOutput(modelSource, 1, "MyOrg.Models.JsonNewtonSoft.Domain.g.cs"));
        [Fact] public async Task EntitySrcGen_VerifyGeneratedSource2() => await Verifier.Verify(new SourceGenerator().GenerateAndGetOutput(modelSource, 2, "MyOrg.Models.JsonNewtonSoft.MyDTO.g.cs"));
    }
}
