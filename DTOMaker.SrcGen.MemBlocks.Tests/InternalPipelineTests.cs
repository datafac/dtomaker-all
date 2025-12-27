using DTOMaker.SrcGen.Core;
using Shouldly;
using System;
using System.Collections.Immutable;
using Xunit;

namespace DTOMaker.SrcGen.MemBlocks.Tests
{
    public class InternalPipelineTests
    {
        private static TypeFullName CreateTFN(string name) =>
            new TypeFullName(
                new ParsedName($"MyOrg.Models.I{name}"),
                new ParsedName($"MyOrg.Models.{name}"),
                MemberKind.Entity,
                "JST");

        private static ParsedEntity CreateEntity(string name, int id, int keyOffset, string? baseName) =>
            new ParsedEntity(CreateTFN(name), id, baseName is null ? null : CreateTFN(baseName), [])
            {
                KeyOffset = keyOffset
            };

        private static ParsedMember CreateMember(string entName, string fieldName, int sequence, Type type, bool isNullable = false)
        {
            TypeFullName memberType;
            if (type == typeof(int))
            {
                memberType = new TypeFullName(
                    new ParsedName("System.Int32"),
                    new ParsedName("System.Int32"),
                    MemberKind.Native,
                    "JST");
            }
            else if (type == typeof(string))
            {
                memberType = new TypeFullName(
                    new ParsedName("System.String"),
                    new ParsedName("System.String"),
                    MemberKind.Native,
                    "JST");
            }
            else
            {
                throw new NotSupportedException($"Type {type.FullName} not supported in test");
            }
            return new ParsedMember(
                    $"MyOrg.Models.I{entName}.{fieldName}",
                    sequence,
                    memberType,
                    memberType.MemberKind,
                    isNullable,
                    []);
        }

        private static readonly ImmutableArray<ParsedEntity> input = ImmutableArray<ParsedEntity>.Empty
                .Add(CreateEntity("Variant", 1, 0, null))
                .Add(CreateEntity("VarString", 2, 0, "Variant"))
                .Add(CreateEntity("VarNumber", 3, 0, "Variant"))
            ;

        private static readonly ImmutableArray<ParsedMember> members = ImmutableArray<ParsedMember>.Empty
                .Add(CreateMember("VarString", "Value", 1, typeof(string), false))
                .Add(CreateMember("VarNumber", "Value", 1, typeof(int), false))
            ;

        [Fact]
        public void Pipeline00_VerifyInput()
        {
            // arrange

            // act

            // assert
            input.Length.ShouldBe(3);
            input[0].TFN.Intf.FullName.ShouldBe("MyOrg.Models.IVariant");
            input[0].TFN.Impl.FullName.ShouldBe("MyOrg.Models.Variant");
            input[0].BaseTFN.ShouldBeNull();
            input[0].EntityId.ShouldBe(1);
            input[0].KeyOffset.ShouldBe(0);
            input[1].TFN.Intf.FullName.ShouldBe("MyOrg.Models.IVarString");
            input[1].TFN.Impl.FullName.ShouldBe("MyOrg.Models.VarString");
            input[1].BaseTFN.ShouldNotBeNull();
            input[1].BaseTFN.ToString().ShouldBe("MyOrg.Models.Variant : IVariant");
            input[1].EntityId.ShouldBe(2);
            input[1].KeyOffset.ShouldBe(0);
            input[2].TFN.Intf.FullName.ShouldBe("MyOrg.Models.IVarNumber");
            input[2].TFN.Impl.FullName.ShouldBe("MyOrg.Models.VarNumber");
            input[2].BaseTFN.ShouldNotBeNull();
            input[2].BaseTFN.ToString().ShouldBe("MyOrg.Models.Variant : IVariant");
            input[2].EntityId.ShouldBe(3);
            input[2].KeyOffset.ShouldBe(0);
        }

        //[Fact]
        //public void Pipeline01_AddEntityBase()
        //{
        //    // arrange
        //    input.Length.ShouldBe(3);

        //    // act
        //    var parsedEntities = SourceGeneratorBase.AddEntityBase(input, "JST");

        //    // assert
        //    parsedEntities.Length.ShouldBe(4);
        //    parsedEntities[0].TFN.Intf.FullName.ShouldBe("DTOMaker.Runtime.IEntityBase");
        //    parsedEntities[0].TFN.Impl.FullName.ShouldBe("DTOMaker.Runtime.JST.EntityBase");
        //    parsedEntities[0].TFN.ToString().ShouldBe("DTOMaker.Runtime.JST.EntityBase : DTOMaker.Runtime.IEntityBase");
        //    parsedEntities[0].BaseTFN.ShouldBeNull();
        //    parsedEntities[0].EntityId.ShouldBe(0);

        //    parsedEntities[1].TFN.Intf.FullName.ShouldBe("MyOrg.Models.IVariant");
        //    parsedEntities[1].TFN.Impl.FullName.ShouldBe("MyOrg.Models.Variant");
        //    parsedEntities[1].BaseTFN.ShouldNotBeNull();
        //    parsedEntities[1].BaseTFN.ToString().ShouldBe("DTOMaker.Runtime.JST.EntityBase : DTOMaker.Runtime.IEntityBase");
        //    parsedEntities[1].EntityId.ShouldBe(1);

        //    parsedEntities[2].TFN.Intf.FullName.ShouldBe("MyOrg.Models.IVarString");
        //    parsedEntities[2].TFN.Impl.FullName.ShouldBe("MyOrg.Models.VarString");
        //    parsedEntities[2].BaseTFN.ShouldNotBeNull();
        //    parsedEntities[2].BaseTFN.ToString().ShouldBe("MyOrg.Models.Variant : IVariant");
        //    parsedEntities[2].EntityId.ShouldBe(2);

        //    parsedEntities[3].TFN.Intf.FullName.ShouldBe("MyOrg.Models.IVarNumber");
        //    parsedEntities[3].TFN.Impl.FullName.ShouldBe("MyOrg.Models.VarNumber");
        //    parsedEntities[3].BaseTFN.ShouldNotBeNull();
        //    parsedEntities[3].BaseTFN.ToString().ShouldBe("MyOrg.Models.Variant : IVariant");
        //    parsedEntities[3].EntityId.ShouldBe(3);
        //}

        [Fact]
        public void Pipeline02_ResolveMembers()
        {
            // arrange
            input.Length.ShouldBe(3);

            // act
            //var parsedEntities = SourceGeneratorBase.AddEntityBase(input, "JST");
            //parsedEntities.Length.ShouldBe(4);

            var result0 = SourceGeneratorBase.ResolveMembers(input[0], members, input);
            var result1 = SourceGeneratorBase.ResolveMembers(input[1], members, input);
            var result2 = SourceGeneratorBase.ResolveMembers(input[2], members, input);
            //var result3 = SourceGeneratorBase.ResolveMembers(input[3], members, input);

            // assert
            //result0.TFN.Intf.FullName.ShouldBe("DTOMaker.Runtime.IEntityBase");
            //result0.Members.Count.ShouldBe(0);

            result0.TFN.Intf.FullName.ShouldBe("MyOrg.Models.IVariant");
            result0.Members.Count.ShouldBe(0);

            result1.TFN.Intf.FullName.ShouldBe("MyOrg.Models.IVarString");
            result1.Members.Count.ShouldBe(1);
            result1.Members.Array[0].Name.ShouldBe("Value");
            result1.Members.Array[0].Sequence.ShouldBe(1);
            result1.Members.Array[0].MemberType.Impl.Name.ShouldBe("String");
            result1.Members.Array[0].IsNullable.ShouldBe(false);

            result2.TFN.Intf.FullName.ShouldBe("MyOrg.Models.IVarNumber");
            result2.Members.Count.ShouldBe(1);
            result2.Members.Array[0].Name.ShouldBe("Value");
            result2.Members.Array[0].Sequence.ShouldBe(1);
            result2.Members.Array[0].MemberType.Impl.Name.ShouldBe("Int32");
            result2.Members.Array[0].IsNullable.ShouldBe(false);
        }

        [Fact]
        public void Pipeline03_ResolveEntities()
        {
            input.Length.ShouldBe(3);

            // arrange
            var ph1Ents = ImmutableArray.Create<Phase1Entity>([
                SourceGeneratorBase.ResolveMembers(input[0], members, input),
                SourceGeneratorBase.ResolveMembers(input[1], members, input),
                SourceGeneratorBase.ResolveMembers(input[2], members, input)]);

            var ph2Ents = ImmutableArray.Create<Phase2Entity>([
                SourceGeneratorBase.ResolveEntities1(ph1Ents[0], ph1Ents),
                SourceGeneratorBase.ResolveEntities1(ph1Ents[1], ph1Ents),
                SourceGeneratorBase.ResolveEntities1(ph1Ents[2], ph1Ents)]);

            var results = ImmutableArray.Create<OutputEntity>([
                SourceGeneratorBase.ResolveEntities2(ph2Ents[0], ph2Ents),
                SourceGeneratorBase.ResolveEntities2(ph2Ents[1], ph2Ents),
                SourceGeneratorBase.ResolveEntities2(ph2Ents[2], ph2Ents)]);

            var result0 = results[0];
            result0.TFN.FullName.ShouldBe("MyOrg.Models.Variant");
            result0.BaseEntity.ShouldBeNull();
            result0.ClassHeight.ShouldBe(1);
            result0.Members.Count.ShouldBe(0);
            result0.DerivedEntities.Count.ShouldBe(2);

            var result1 = results[1];
            result1.TFN.FullName.ShouldBe("MyOrg.Models.VarString");
            result1.BaseEntity.ShouldNotBeNull();
            result1.BaseEntity.TFN.FullName.ShouldBe("MyOrg.Models.Variant");
            result1.ClassHeight.ShouldBe(2);
            result1.Members.Count.ShouldBe(1);
            result1.DerivedEntities.Count.ShouldBe(0);

            var result2 = results[2];
            result2.TFN.FullName.ShouldBe("MyOrg.Models.VarNumber");
            result2.BaseEntity.ShouldNotBeNull();
            result2.BaseEntity.TFN.FullName.ShouldBe("MyOrg.Models.Variant");
            result2.ClassHeight.ShouldBe(2);
            result2.Members.Count.ShouldBe(1);
            result2.DerivedEntities.Count.ShouldBe(0);
        }
    }
}