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
        [Fact] public void IdAttributeName() => SourceGeneratorBase.IdAttribute.ShouldBe(nameof(DTOMaker.Models.IdAttribute));
        [Fact] public void ObsoleteAttributeName() => SourceGeneratorBase.ObsoleteAttribute.ShouldBe(nameof(System.ObsoleteAttribute));
        [Fact] public void KeyOffsetAttributeName() => SourceGeneratorBase.KeyOffsetAttribute.ShouldBe(nameof(DTOMaker.Models.KeyOffsetAttribute));
        [Fact] public void FixedLengthAttributeName() => SourceGeneratorBase.FixedLengthAttribute.ShouldBe(nameof(DTOMaker.Models.FixedLengthAttribute));
    }
}