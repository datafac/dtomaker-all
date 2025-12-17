using DTOMaker.SrcGen.Core;
using Shouldly;
using System;
using System.Collections.Immutable;
using Xunit;

namespace DTOMaker.SrcGen.JsonSystemText.Tests
{
    public class InternalPipelineTests
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
                public interface IVariant : IEntityBase
                {
                }
                [Entity][Id(2)]
                public interface IVarString : IVariant
                {
                    [Member(1)] String  Value { get; set; }
                }
                [Entity][Id(3)]
                public interface IVarNumber : IVariant
                {
                    [Member(1)] Int32  Value { get; set; }
                }
            }
            """;

        private static readonly TypeFullName EntityBase = 
            new TypeFullName(
                new ParsedName("DTOMaker.Runtime.IEntityBase"),
                new ParsedName("DTOMaker.Runtime.JsonSystemText.EntityBase"),
                MemberKind.Entity);

        private static TypeFullName CreateTFN(string name) =>
            new TypeFullName(
                new ParsedName($"MyOrg.Models.I{name}"),
                new ParsedName($"MyOrg.Models.{name}"),
                MemberKind.Entity);

        private static ParsedEntity CreateEntity(string name, int id, string? baseName) =>
            new ParsedEntity(CreateTFN(name), id, baseName is null ? EntityBase : CreateTFN(baseName));

        private static ParsedMember CreateMember(string entName, string fieldName, int sequence, Type type, bool isNullable = false)
        {
            TypeFullName memberType;
            if (type == typeof(int))
            {
                memberType = new TypeFullName(
                    new ParsedName("System.Int32"),
                    new ParsedName("System.Int32"),
                    MemberKind.Native);
            }
            else if (type == typeof(string))
            {
                memberType = new TypeFullName(
                    new ParsedName("System.String"),
                    new ParsedName("System.String"),
                    MemberKind.Native);
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
                    false, "", false);
        }

        private static readonly ImmutableArray<ParsedEntity> input = ImmutableArray<ParsedEntity>.Empty
                .Add(CreateEntity("Variant", 1, null))
                .Add(CreateEntity("VarString", 2, "Variant"))
                .Add(CreateEntity("VarNumber", 3, "Variant"))
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
            input[0].BaseTFN.ShouldNotBeNull();
            input[0].BaseTFN.ToString().ShouldBe("DTOMaker.Runtime.JsonSystemText.EntityBase : DTOMaker.Runtime.IEntityBase");
            input[0].EntityId.ShouldBe(1);
            input[1].TFN.Intf.FullName.ShouldBe("MyOrg.Models.IVarString");
            input[1].TFN.Impl.FullName.ShouldBe("MyOrg.Models.VarString");
            input[1].BaseTFN.ShouldNotBeNull();
            input[1].BaseTFN.ToString().ShouldBe("MyOrg.Models.Variant : IVariant");
            input[1].EntityId.ShouldBe(2);
            input[2].TFN.Intf.FullName.ShouldBe("MyOrg.Models.IVarNumber");
            input[2].TFN.Impl.FullName.ShouldBe("MyOrg.Models.VarNumber");
            input[2].BaseTFN.ShouldNotBeNull();
            input[2].BaseTFN.ToString().ShouldBe("MyOrg.Models.Variant : IVariant");
            input[2].EntityId.ShouldBe(3);
        }

        [Fact]
        public void Pipeline01_AddEntityBase()
        {
            // arrange
            input.Length.ShouldBe(3);

            // act
            var parsedEntities = SourceGeneratorBase.AddEntityBase(input);

            // assert
            parsedEntities.Length.ShouldBe(4);
            parsedEntities[0].TFN.Intf.FullName.ShouldBe("DTOMaker.Runtime.IEntityBase");
            parsedEntities[0].TFN.Impl.FullName.ShouldBe("DTOMaker.Runtime.JsonSystemText.EntityBase");
            parsedEntities[0].BaseTFN.ShouldBeNull();
            parsedEntities[0].EntityId.ShouldBe(0);

            parsedEntities[1].TFN.Intf.FullName.ShouldBe("MyOrg.Models.IVariant");
            parsedEntities[1].TFN.Impl.FullName.ShouldBe("MyOrg.Models.Variant");
            parsedEntities[1].BaseTFN.ShouldNotBeNull();
            parsedEntities[1].BaseTFN.ToString().ShouldBe("DTOMaker.Runtime.JsonSystemText.EntityBase : DTOMaker.Runtime.IEntityBase");
            parsedEntities[1].EntityId.ShouldBe(1);

            parsedEntities[2].TFN.Intf.FullName.ShouldBe("MyOrg.Models.IVarString");
            parsedEntities[2].TFN.Impl.FullName.ShouldBe("MyOrg.Models.VarString");
            parsedEntities[2].BaseTFN.ShouldNotBeNull();
            parsedEntities[2].BaseTFN.ToString().ShouldBe("MyOrg.Models.Variant : IVariant");
            parsedEntities[2].EntityId.ShouldBe(2);

            parsedEntities[3].TFN.Intf.FullName.ShouldBe("MyOrg.Models.IVarNumber");
            parsedEntities[3].TFN.Impl.FullName.ShouldBe("MyOrg.Models.VarNumber");
            parsedEntities[3].BaseTFN.ShouldNotBeNull();
            parsedEntities[3].BaseTFN.ToString().ShouldBe("MyOrg.Models.Variant : IVariant");
            parsedEntities[3].EntityId.ShouldBe(3);
        }

        [Fact]
        public void Pipeline02_ResolveMembers()
        {
            // arrange
            input.Length.ShouldBe(3);

            // act
            var parsedEntities = SourceGeneratorBase.AddEntityBase(input);
            parsedEntities.Length.ShouldBe(4);

            var result0 = SourceGeneratorBase.ResolveMembers(parsedEntities[0], members, parsedEntities);
            var result1 = SourceGeneratorBase.ResolveMembers(parsedEntities[1], members, parsedEntities);
            var result2 = SourceGeneratorBase.ResolveMembers(parsedEntities[2], members, parsedEntities);
            var result3 = SourceGeneratorBase.ResolveMembers(parsedEntities[3], members, parsedEntities);

            // assert
            result0.TFN.Intf.FullName.ShouldBe("DTOMaker.Runtime.IEntityBase");
            result0.Members.Count.ShouldBe(0);

            result1.TFN.Intf.FullName.ShouldBe("MyOrg.Models.IVariant");
            result1.Members.Count.ShouldBe(0);

            result2.TFN.Intf.FullName.ShouldBe("MyOrg.Models.IVarString");
            result2.Members.Count.ShouldBe(1);
            result2.Members.Array[0].Name.ShouldBe("Value");
            result2.Members.Array[0].Sequence.ShouldBe(1);
            result2.Members.Array[0].MemberType.Impl.Name.ShouldBe("String");
            result2.Members.Array[0].IsNullable.ShouldBe(false);

            result3.TFN.Intf.FullName.ShouldBe("MyOrg.Models.IVarNumber");
            result3.Members.Count.ShouldBe(1);
            result3.Members.Array[0].Name.ShouldBe("Value");
            result3.Members.Array[0].Sequence.ShouldBe(1);
            result3.Members.Array[0].MemberType.Impl.Name.ShouldBe("Int32");
            result3.Members.Array[0].IsNullable.ShouldBe(false);
        }

        [Fact]
        public void Pipeline03_ResolveEntities()
        {
            // arrange
            input.Length.ShouldBe(3);

            // act
            var parsedEntities = SourceGeneratorBase.AddEntityBase(input);
            parsedEntities.Length.ShouldBe(4);

            var entity0 = SourceGeneratorBase.ResolveMembers(parsedEntities[0], members, parsedEntities);
            var entity1 = SourceGeneratorBase.ResolveMembers(parsedEntities[1], members, parsedEntities);
            var entity2 = SourceGeneratorBase.ResolveMembers(parsedEntities[2], members, parsedEntities);
            var entity3 = SourceGeneratorBase.ResolveMembers(parsedEntities[3], members, parsedEntities);

            var entities = ImmutableArray.Create<Phase1Entity>(entity0, entity1, entity2, entity3);

            var result0 = SourceGeneratorBase.ResolveEntities(entity0, entities);
            var result1 = SourceGeneratorBase.ResolveEntities(entity1, entities);
            var result2 = SourceGeneratorBase.ResolveEntities(entity2, entities);
            var result3 = SourceGeneratorBase.ResolveEntities(entity3, entities);

            // assert
            result0.TFN.FullName.ShouldBe("DTOMaker.Runtime.JsonSystemText.EntityBase");
            result0.BaseEntity.ShouldBeNull();
            result0.ClassHeight.ShouldBe(0);
            result0.Members.Count.ShouldBe(0);
            result0.DerivedEntities.Count.ShouldBe(3);

            result1.TFN.FullName.ShouldBe("MyOrg.Models.Variant");
            result1.BaseEntity.ShouldNotBeNull();
            result1.BaseEntity.TFN.FullName.ShouldBe("DTOMaker.Runtime.JsonSystemText.EntityBase");
            result1.ClassHeight.ShouldBe(1);
            result1.Members.Count.ShouldBe(0);
            result1.DerivedEntities.Count.ShouldBe(2);

            result2.TFN.FullName.ShouldBe("MyOrg.Models.VarString");
            result2.BaseEntity.ShouldNotBeNull();
            result2.BaseEntity.TFN.FullName.ShouldBe("MyOrg.Models.Variant");
            result2.ClassHeight.ShouldBe(2);
            result2.Members.Count.ShouldBe(1);
            result2.DerivedEntities.Count.ShouldBe(0);

            result3.TFN.FullName.ShouldBe("MyOrg.Models.VarNumber");
            result3.BaseEntity.ShouldNotBeNull();
            result3.BaseEntity.TFN.FullName.ShouldBe("MyOrg.Models.Variant");
            result3.ClassHeight.ShouldBe(2);
            result3.Members.Count.ShouldBe(1);
            result3.DerivedEntities.Count.ShouldBe(0);
        }
    }
}