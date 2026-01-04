using DTOMaker.SrcGen.Core;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace DTOMaker.SrcGen.MsgPack2
{

    [Generator]
    public sealed class SourceGenerator : SourceGeneratorBase
    {
        private static readonly SourceGeneratorParameters _parameters = new SourceGeneratorParameters()
        {
            GeneratorId = GeneratorId.MsgPack2,
            Language = Language_CSharp.Instance,
            ImplSpaceSuffix = "MsgPack2"
        };
        protected override SourceGeneratorParameters OnBeginInitialize(IncrementalGeneratorInitializationContext context) => _parameters;
        protected override void OnEndInitialize(IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<OutputEntity> entities)
        {
            // generate entities
            context.RegisterSourceOutput(entities, (spc, ent) =>
            {
                var generator = new EntityGenerator(_parameters);
                string source = generator.GenerateSourceText(ent);
                string hintName = $"{ent.TFN.Impl.Space}.{ent.TFN.Impl.Name}.g.cs";
                spc.AddSource(hintName, SourceText.From(source, Encoding.UTF8));
            });
        }

        protected override OutputEntity OnCustomizeOutputEntity(OutputEntity entity, Phase2Entity inputEntity, ImmutableArray<Phase2Entity> allEntities)
        {
            return entity;
        }

        protected override ParsedEntity OnCustomizeParsedEntity(ParsedEntity entity, Location location)
        {
            return entity;
        }

        protected override ParsedMember OnCustomizeParsedMember(ParsedMember member, Location location)
        {
            return member;
        }

        protected override Phase1Entity OnCustomizePhase1Entity(Phase1Entity entity, Location location, IReadOnlyList<OutputMember> members)
        {
            return entity;
        }
    }
}
