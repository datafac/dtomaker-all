using Shouldly;
using System;
using Xunit;

namespace DTOMaker.SrcGen.JsonSystemText.Tests
{
    public class ModelAttributeNameChecks
    {
        [Fact] public void EntityAttributeName() => DTOMaker.SrcGen.Core.SourceGeneratorBase.EntityAttribute.ShouldBe(nameof(DTOMaker.Models.EntityAttribute));
        [Fact] public void MemberAttributeName() => DTOMaker.SrcGen.Core.SourceGeneratorBase.MemberAttribute.ShouldBe(nameof(DTOMaker.Models.MemberAttribute));
        [Fact] public void ObsoleteAttributeName() => DTOMaker.SrcGen.Core.SourceGeneratorBase.ObsoleteAttribute.ShouldBe(nameof(System.ObsoleteAttribute));
        [Fact] public void KeyOffsetAttributeName() => DTOMaker.SrcGen.Core.SourceGeneratorBase.KeyOffsetAttribute.ShouldBe(nameof(DTOMaker.Models.KeyOffsetAttribute));
        [Fact] public void LengthAttributeName() => DTOMaker.SrcGen.Core.SourceGeneratorBase.LengthAttribute.ShouldBe(nameof(DTOMaker.Models.LengthAttribute));
        [Fact] public void OffsetAttributeName() => DTOMaker.SrcGen.Core.SourceGeneratorBase.OffsetAttribute.ShouldBe(nameof(DTOMaker.Models.OffsetAttribute));
        [Fact] public void EndianAttributeName() => DTOMaker.SrcGen.Core.SourceGeneratorBase.EndianAttribute.ShouldBe(nameof(DTOMaker.Models.EndianAttribute));
        [Fact] public void LayoutMethod_Undefined() => ((int)DTOMaker.SrcGen.Core.LayoutAlgo.Default).ShouldBe((int)DTOMaker.Models.LayoutMethod.Undefined);
        [Fact] public void LayoutMethod_Explicit() => ((int)DTOMaker.SrcGen.Core.LayoutAlgo.Explicit).ShouldBe((int)DTOMaker.Models.LayoutMethod.Explicit);
        [Fact] public void LayoutMethod_Linear() => ((int)DTOMaker.SrcGen.Core.LayoutAlgo.Linear).ShouldBe((int)DTOMaker.Models.LayoutMethod.Linear);
        //[Fact] public void LayoutMethod_Compact() => ((int)DTOMaker.SrcGen.Core.LayoutAlgo.Compact).ShouldBe((int)DTOMaker.Models.LayoutMethod.Compact);

        [Fact] public void NativeType_Undefined() => ((int)DTOMaker.SrcGen.Core.NativeType.Undefined).ShouldBe((int)DTOMaker.Models.NativeType.Undefined);
        [Fact] public void NativeType_SByte() => ((int)DTOMaker.SrcGen.Core.NativeType.SByte).ShouldBe((int)DTOMaker.Models.NativeType.SByte);
        [Fact] public void NativeType_Byte() => ((int)DTOMaker.SrcGen.Core.NativeType.Byte).ShouldBe((int)DTOMaker.Models.NativeType.Byte);
        [Fact] public void NativeType_Int16() => ((int)DTOMaker.SrcGen.Core.NativeType.Int16).ShouldBe((int)DTOMaker.Models.NativeType.Int16);
        [Fact] public void NativeType_UInt16() => ((int)DTOMaker.SrcGen.Core.NativeType.UInt16).ShouldBe((int)DTOMaker.Models.NativeType.UInt16);
        [Fact] public void NativeType_Int32() => ((int)DTOMaker.SrcGen.Core.NativeType.Int32).ShouldBe((int)DTOMaker.Models.NativeType.Int32);
        [Fact] public void NativeType_UInt32() => ((int)DTOMaker.SrcGen.Core.NativeType.UInt32).ShouldBe((int)DTOMaker.Models.NativeType.UInt32);
        [Fact] public void NativeType_Int64() => ((int)DTOMaker.SrcGen.Core.NativeType.Int64).ShouldBe((int)DTOMaker.Models.NativeType.Int64);
        [Fact] public void NativeType_UInt64() => ((int)DTOMaker.SrcGen.Core.NativeType.UInt64).ShouldBe((int)DTOMaker.Models.NativeType.UInt64);
        [Fact] public void NativeType_Int128() => ((int)DTOMaker.SrcGen.Core.NativeType.Int128).ShouldBe((int)DTOMaker.Models.NativeType.Int128);
        [Fact] public void NativeType_UInt128() => ((int)DTOMaker.SrcGen.Core.NativeType.UInt128).ShouldBe((int)DTOMaker.Models.NativeType.UInt128);
        [Fact] public void NativeType_Boolean() => ((int)DTOMaker.SrcGen.Core.NativeType.Boolean).ShouldBe((int)DTOMaker.Models.NativeType.Boolean);
        [Fact] public void NativeType_Char() => ((int)DTOMaker.SrcGen.Core.NativeType.Char).ShouldBe((int)DTOMaker.Models.NativeType.Char);
        [Fact] public void NativeType_Half() => ((int)DTOMaker.SrcGen.Core.NativeType.Half).ShouldBe((int)DTOMaker.Models.NativeType.Half);
        [Fact] public void NativeType_Single() => ((int)DTOMaker.SrcGen.Core.NativeType.Single).ShouldBe((int)DTOMaker.Models.NativeType.Single);
        [Fact] public void NativeType_Double() => ((int)DTOMaker.SrcGen.Core.NativeType.Double).ShouldBe((int)DTOMaker.Models.NativeType.Double);
        [Fact] public void NativeType_Decimal() => ((int)DTOMaker.SrcGen.Core.NativeType.Decimal).ShouldBe((int)DTOMaker.Models.NativeType.Decimal);
        [Fact] public void NativeType_Guid() => ((int)DTOMaker.SrcGen.Core.NativeType.Guid).ShouldBe((int)DTOMaker.Models.NativeType.Guid);
        [Fact] public void NativeType_String() => ((int)DTOMaker.SrcGen.Core.NativeType.String).ShouldBe((int)DTOMaker.Models.NativeType.String);
        [Fact] public void NativeType_Binary() => ((int)DTOMaker.SrcGen.Core.NativeType.Binary).ShouldBe((int)DTOMaker.Models.NativeType.Binary);
        [Fact] public void NativeType_PairOfInt16() => ((int)DTOMaker.SrcGen.Core.NativeType.PairOfInt16).ShouldBe((int)DTOMaker.Models.NativeType.PairOfInt16);
        [Fact] public void NativeType_PairOfInt32() => ((int)DTOMaker.SrcGen.Core.NativeType.PairOfInt32).ShouldBe((int)DTOMaker.Models.NativeType.PairOfInt32);
        [Fact] public void NativeType_PairOfInt64() => ((int)DTOMaker.SrcGen.Core.NativeType.PairOfInt64).ShouldBe((int)DTOMaker.Models.NativeType.PairOfInt64);
    }
}
