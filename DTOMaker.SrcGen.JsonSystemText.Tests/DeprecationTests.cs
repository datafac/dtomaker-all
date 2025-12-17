using DTOMaker.SrcGen.Core;
using Shouldly;
using System.Collections.Immutable;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace DTOMaker.SrcGen.JsonSystemText.Tests
{
    public class DeprecationTests
    {
        private static readonly string modelSource =
            """
            using System;
            using DTOMaker.Models;
            using DTOMaker.Runtime;
            namespace MyOrg.Models
            {
                [Entity] [Id(1)]
                public interface IMyDTO : IEntityBase
                {
                    [Obsolete]                  [Member(1)] double Field1 { get; set; }
                    [Obsolete("Removed")]       [Member(2)] double Field2 { get; set; }
                    [Obsolete("Removed", true)] [Member(3)] double Field3 { get; set; }
                }
            }
            """;

        [Fact] public void Obsolete_GeneratedSourcesLength() => modelSource.GenerateAndCheckLength(2);
        [Fact] public async Task Obsolete_VerifyGeneratedSource0() => await Verifier.Verify(modelSource.GenerateAndGetOutput(0, "DTOMaker.Runtime.JsonSystemText.EntityBase.g.cs"));
        [Fact] public async Task Obsolete_VerifyGeneratedSource1() => await Verifier.Verify(modelSource.GenerateAndGetOutput(1, "MyOrg.Models.JsonSystemText.MyDTO.g.cs"));

    }
}