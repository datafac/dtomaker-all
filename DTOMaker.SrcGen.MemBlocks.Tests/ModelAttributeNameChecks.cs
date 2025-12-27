using DTOMaker.SrcGen.Core;
using Shouldly;
using System;
using Xunit;

namespace DTOMaker.SrcGen.MemBlocks.Tests
{
    public class ModelAttributeNameChecks
    {
        [Fact] public void EntityAttributeName() => SourceGeneratorBase.EntityAttribute.ShouldBe(nameof(DTOMaker.Models.EntityAttribute));
        [Fact] public void MemberAttributeName() => SourceGeneratorBase.MemberAttribute.ShouldBe(nameof(DTOMaker.Models.MemberAttribute));
        [Fact] public void ObsoleteAttributeName() => SourceGeneratorBase.ObsoleteAttribute.ShouldBe(nameof(System.ObsoleteAttribute));
        [Fact] public void KeyOffsetAttributeName() => SourceGeneratorBase.KeyOffsetAttribute.ShouldBe(nameof(DTOMaker.Models.KeyOffsetAttribute));
        [Fact] public void LengthAttributeName() => SourceGeneratorBase.LengthAttribute.ShouldBe(nameof(DTOMaker.Models.LengthAttribute));
        [Fact] public void OffsetAttributeName() => SourceGeneratorBase.OffsetAttribute.ShouldBe(nameof(DTOMaker.Models.OffsetAttribute));
        [Fact] public void EndianAttributeName() => SourceGeneratorBase.EndianAttribute.ShouldBe(nameof(DTOMaker.Models.EndianAttribute));
        [Fact] public void LayoutMethod_Undefined() => ((int)LayoutAlgo.Default).ShouldBe((int)DTOMaker.Models.LayoutMethod.Undefined);
        [Fact] public void LayoutMethod_Explicit() => ((int)LayoutAlgo.Explicit).ShouldBe((int)DTOMaker.Models.LayoutMethod.Explicit);
        [Fact] public void LayoutMethod_Linear() => ((int)LayoutAlgo.Linear).ShouldBe((int)DTOMaker.Models.LayoutMethod.Linear);
        //[Fact] public void LayoutMethod_Compact() => ((int)LayoutAlgo.Compact).ShouldBe((int)DTOMaker.Models.LayoutMethod.Compact);
    }
}