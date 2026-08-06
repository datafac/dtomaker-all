using DTOMaker.TestHelpers;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace DTOMaker.SrcGen.MsgPack2.Tests;

public class EntityGeneratorTests
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
            [Member(1)] int  Field1 { get; set; }
            [Member(2)] int? Field2 { get; set; }
        }
        [Entity(2)]
        public interface IDerived : IMyDTO
        {
            [Member(1)] String  Field11 { get; set; }
            [Member(2)] String? Field12 { get; set; }
        }
    }
    namespace MyOrg.Model2
    {
        [Entity(3)]
        public interface IMyDTO : IEntityBase
        {
            [Member(1)] IMyDTO  Field31 { get; set; }
            [Member(2)] IMyDTO? Field32 { get; set; }
        }
    }
    namespace MyOrg.Model3
    {
        [Entity(4)]
        public interface IMyDTO : IEntityBase
        {
            [Member(1)] Octets  Field41 { get; set; }
            [Member(2)] Octets? Field42 { get; set; }
        }
    }
    """;

    [Fact] public void EntitySrcGen_GeneratedSourcesLength() => new SourceGenerator().GenerateAndCheckLength(modelSource, 6);
    [Fact] public async Task EntitySrcGen_VerifyGeneratedSource0() => await Verifier.Verify(new SourceGenerator().GenerateAndGetOutput(modelSource, 0, "MyOrg.Models.Global.g.cs"));
    [Fact] public async Task EntitySrcGen_VerifyGeneratedSource1() => await Verifier.Verify(new SourceGenerator().GenerateAndGetOutput(modelSource, 1, "MyOrg.Models.MsgPack2.Domain.g.cs"));
    [Fact] public async Task EntitySrcGen_VerifyGeneratedSource2() => await Verifier.Verify(new SourceGenerator().GenerateAndGetOutput(modelSource, 2, "MyOrg.Models.MsgPack2.MyDTO.g.cs"));
    [Fact] public async Task EntitySrcGen_VerifyGeneratedSource3() => await Verifier.Verify(new SourceGenerator().GenerateAndGetOutput(modelSource, 3, "MyOrg.Models.MsgPack2.Derived.g.cs"));
    [Fact] public async Task EntitySrcGen_VerifyGeneratedSource4() => await Verifier.Verify(new SourceGenerator().GenerateAndGetOutput(modelSource, 4, "MyOrg.Model2.MsgPack2.MyDTO.g.cs"));
    [Fact] public async Task EntitySrcGen_VerifyGeneratedSource5() => await Verifier.Verify(new SourceGenerator().GenerateAndGetOutput(modelSource, 5, "MyOrg.Model3.MsgPack2.MyDTO.g.cs"));
}
