using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace DTOMaker.SrcGen.JsonSystemText.Tests
{
    public class EntityGeneratorTests
    {
        private static readonly string modelSource =
            """
            using System;
            using DataFac.Memory;
            using DTOMaker.Models;
            using DTOMaker.Runtime;
            namespace MyOrg.Models
            {
                [Entity][Id(1)]
                public interface IMyDTO : IEntityBase
                {
                    [Member(1)] int  Field1 { get; set; }
                    [Member(2)] int? Field2 { get; set; }
                }
                [Entity][Id(2)]
                public interface IDerived : IMyDTO
                {
                    [Member(1)] String  Field11 { get; set; }
                    [Member(2)] String? Field12 { get; set; }
                }
            }
            namespace MyOrg.Model2
            {
                [Entity][Id(3)]
                public interface IMyDTO : IEntityBase
                {
                    [Member(1)] IMyDTO  Field31 { get; set; }
                    [Member(2)] IMyDTO? Field32 { get; set; }
                }
            }
            namespace MyOrg.Model3
            {
                [Entity][Id(4)]
                public interface IMyDTO : IEntityBase
                {
                    [Member(1)] Octets  Field41 { get; set; }
                    [Member(2)] Octets? Field42 { get; set; }
                }
            }
            """;

        [Fact] public void EntitySrcGen_GeneratedSourcesLength() => modelSource.GenerateAndCheckLength(5);
        [Fact] public async Task EntitySrcGen_VerifyGeneratedSource0() => await Verifier.Verify(modelSource.GenerateAndGetOutput(0, "DTOMaker.Runtime.JsonSystemText.EntityBase.g.cs"));
        [Fact] public async Task EntitySrcGen_VerifyGeneratedSource1() => await Verifier.Verify(modelSource.GenerateAndGetOutput(1, "MyOrg.Models.JsonSystemText.MyDTO.g.cs"));
        [Fact] public async Task EntitySrcGen_VerifyGeneratedSource2() => await Verifier.Verify(modelSource.GenerateAndGetOutput(2, "MyOrg.Models.JsonSystemText.Derived.g.cs"));
        [Fact] public async Task EntitySrcGen_VerifyGeneratedSource3() => await Verifier.Verify(modelSource.GenerateAndGetOutput(3, "MyOrg.Model2.JsonSystemText.MyDTO.g.cs"));
        [Fact] public async Task EntitySrcGen_VerifyGeneratedSource4() => await Verifier.Verify(modelSource.GenerateAndGetOutput(4, "MyOrg.Model3.JsonSystemText.MyDTO.g.cs"));
    }
}
