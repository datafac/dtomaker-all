using DTOMaker.SrcGen.Core;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace DTOMaker.SrcGen.MemBlocks
{
    [Generator]
    public sealed class SourceGenerator : SourceGeneratorBase
    {
        public static readonly DiagnosticDescriptor DME05 = new DiagnosticDescriptor(nameof(DME05), 
            "Invalid entity length", "The entity length must be a whole power of 2 between 0 and 8192", DiagnosticCategory.Design, DiagnosticSeverity.Error, true);

        public static readonly DiagnosticDescriptor DME09 = new DiagnosticDescriptor(nameof(DME09), 
            "Invalid layout method", "Entity layout method must be defined", DiagnosticCategory.Design, DiagnosticSeverity.Error, true);

        private static readonly SourceGeneratorParameters _parameters = new SourceGeneratorParameters()
        {
            GeneratorId = GeneratorId.MemBlocks,
            Language = Language_CSharp.Instance,
            ImplSpaceSuffix = "MemBlocks"
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

        protected override OutputEntity OnCustomizeOutputEntity(OutputEntity outputEntity, Phase2Entity inputEntity, ImmutableArray<Phase2Entity> allEntities)
        {
            // calculate structure code
            var structureCode = new StructureCode(inputEntity.ClassHeight, inputEntity.BlockLength);
            Phase2Entity? parent = allEntities.FirstOrDefault(e => e.TFN == inputEntity.BaseEntity?.TFN);
            while (parent is not null)
            {
                structureCode = structureCode.AddInnerBlock(parent.ClassHeight, parent.BlockLength);
                parent = parent.BaseEntity is not null
                    ? allEntities.FirstOrDefault(e => e.TFN == parent.BaseEntity.TFN)
                    : null;
            }

            return outputEntity with { BlockStructureCode = structureCode.Bits };
        }

        private static bool IsValidBlockLength(int blockLength)
        {
            const int minimum = 0;
            const int maximum = 8192;
            if (blockLength < minimum) return false;
            if (blockLength > maximum) return false;
            if (blockLength == 0) return true;
            int comparand = 1;
            while (true)
            {
                if (comparand > blockLength) return false;
                if (blockLength == comparand) return true;
                comparand = comparand * 2;
            }
        }

        protected override ParsedEntity OnCustomizeParsedEntity(ParsedEntity parsedEntity, Location location)
        {
            List<Diagnostic> newDiagnostics = new();
            // check block length
            if (!IsValidBlockLength(parsedEntity.BlockLength))
            {
                newDiagnostics.Add(Diagnostic.Create(DME05, location));
            }
            // check layout algo
            var layoutAlgo = parsedEntity.Layout;
            if (layoutAlgo != LayoutAlgo.Explicit && layoutAlgo != LayoutAlgo.Linear)
            {
                newDiagnostics.Add(Diagnostic.Create(DME09, location));
            }

            if (newDiagnostics.Count == 0) return parsedEntity;

            return parsedEntity with
            {
                Diagnostics = new EquatableArray<Diagnostic>(parsedEntity.Diagnostics.Concat(newDiagnostics)),
            };
        }
    }
}
