using DTOMaker.SrcGen.Core;
using Shouldly;
using System.Collections.Immutable;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace DTOMaker.SrcGen.JsonNewtonSoft.Tests
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
                [Entity(1)]
                public interface IMyDTO : IEntityBase
                {
                    [Obsolete]                  [Member(1)] double Field1 { get; set; }
                    [Obsolete("Removed")]       [Member(2)] double Field2 { get; set; }
                    [Obsolete("Removed", true)] [Member(3)] double Field3 { get; set; }
                }
            }
            """;

        [Fact] public void Obsolete_GeneratedSourcesLength() => modelSource.GenerateAndCheckLength(1);
        [Fact] public async Task Obsolete_VerifyGeneratedSource0() => await Verifier.Verify(modelSource.GenerateAndGetOutput(0, "MyOrg.Models.JsonNewtonSoft.MyDTO.g.cs"));

    }
}