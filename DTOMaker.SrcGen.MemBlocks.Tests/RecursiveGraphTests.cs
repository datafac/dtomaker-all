using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace DTOMaker.SrcGen.MemBlocks.Tests
{
    public class RecursiveGraphTests
    {
        private static readonly string modelSource =
            """
            using System;
            using DTOMaker.Models;
            using DTOMaker.Runtime;
            namespace MyOrg.Models
            {
                [Entity][Id(1)][Layout(LayoutMethod.Linear)]
                public interface IMyDTO : IEntityBase
                {
                    [Member(2)] IMyDTO? Field1 { get; set; }
                }
            }
            """;

        [Fact] public void EntitySrcGen_GeneratedSourcesLength() => modelSource.GenerateAndCheckLength(1);
        [Fact] public async Task EntitySrcGen_VerifyGeneratedSource0() => await Verifier.Verify(modelSource.GenerateAndGetOutput(0, "MyOrg.Models.MemBlocks.MyDTO.g.cs"));
    }
}