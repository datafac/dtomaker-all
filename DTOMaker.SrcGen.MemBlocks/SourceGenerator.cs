using DTOMaker.SrcGen.Core;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections;
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
        private static readonly DiagnosticDescriptor DME05 = new DiagnosticDescriptor(nameof(DME05),
            "Invalid entity length", "The entity length must be a whole power of 2 between 0 and 8192", DiagnosticCategory.Design, DiagnosticSeverity.Error, true);

        private static readonly DiagnosticDescriptor DME09 = new DiagnosticDescriptor(nameof(DME09),
            "Invalid layout method", "Entity layout method must be defined", DiagnosticCategory.Design, DiagnosticSeverity.Error, true);

        private static readonly DiagnosticDescriptor DME06 = new DiagnosticDescriptor(nameof(DME06),
            "Invalid member length", "The member length must be a whole power of 2 between 1 and 1024", DiagnosticCategory.Design, DiagnosticSeverity.Error, true);

        private static readonly DiagnosticDescriptor DME07 = new DiagnosticDescriptor(nameof(DME07),
            "Invalid member offset", "The member offset must be zero or greater", DiagnosticCategory.Design, DiagnosticSeverity.Error, true);

        private static readonly DiagnosticDescriptor DME08 = new DiagnosticDescriptor(nameof(DME08),
            "Invalid nullability", "Nullable<T> fields are not supported in MemBlocks", DiagnosticCategory.Design, DiagnosticSeverity.Error, true);

        private static readonly DiagnosticDescriptor DME13 = new DiagnosticDescriptor(nameof(DME13),
            "Member layout issue", "Member overlaps another, is misaligned, or extends beyond the end of the block", DiagnosticCategory.Design, DiagnosticSeverity.Error, true);

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

        private static bool IsValidFieldLength(int fieldLength)
        {
            const int minimum = 1;
            const int maximum = 1024;
            if (fieldLength < minimum) return false;
            if (fieldLength > maximum) return false;
            int comparand = 1;
            while (true)
            {
                if (comparand > fieldLength) return false;
                if (fieldLength == comparand) return true;
                comparand = comparand * 2;
            }
        }

        protected override ParsedMember OnCustomizeParsedMember(ParsedMember parsedMember, Location location)
        {
            bool updated = false;
            bool isExternal = false;
            int fieldLength = parsedMember.FieldLength;

            // set default length for external (variable length) Octets and String
            if (fieldLength == 0 && (parsedMember.Kind == MemberKind.String || parsedMember.Kind == MemberKind.Binary))
            {
                fieldLength = BlobIdV1Size;
                isExternal = true;
                updated = true;
            }
            // set default length for entities
            if (parsedMember.Kind == MemberKind.Entity)
            {
                fieldLength = BlobIdV1Size;
                isExternal = true;
                updated = true;
            }

            // checks
            List<Diagnostic> newDiagnostics = new();
            if (!IsValidFieldLength(fieldLength))
            {
                newDiagnostics.Add(Diagnostic.Create(DME06, location));
                updated = true;
            }
            if (parsedMember.FieldOffset < 0)
            {
                newDiagnostics.Add(Diagnostic.Create(DME07, location));
                updated = true;
            }
            if (parsedMember.IsNullable && parsedMember.Kind == MemberKind.Native)
            {
                newDiagnostics.Add(Diagnostic.Create(DME08, location));
                updated = true;
            }

            if (updated)
            {
                return parsedMember with
                {
                    FieldLength = fieldLength,
                    IsExternal = isExternal,
                    Diagnostics = new EquatableArray<Diagnostic>(parsedMember.Diagnostics.Concat(newDiagnostics)),
                };
            }
            else
            {
                return parsedMember;
            }
        }

        private static Diagnostic? CheckMemberLayout(int blockLength, IEnumerable<OutputMember> members)
        {
            BitArray blockMap = new BitArray(blockLength);
            foreach (var member in members.OrderBy(m => m.Sequence))
            {
                if (member.FieldOffset + member.FieldLength > blockLength)
                {
                    return Diagnostic.Create(DME13, member.Location);
                }

                if (member.FieldLength > 0 && (member.FieldOffset % member.FieldLength != 0))
                {
                    return Diagnostic.Create(DME13, member.Location);
                }

                // check value bytes layout
                for (var i = 0; i < member.FieldLength; i++)
                {
                    int offset = member.FieldOffset + i;
                    if (blockMap.Get(offset))
                    {
                        return Diagnostic.Create(DME13, member.Location);
                    }

                    // not assigned
                    blockMap.Set(offset, true);
                }
            }
            return null;
        }

        protected override Phase1Entity OnCustomizePhase1Entity(Phase1Entity entity, Location location, IReadOnlyList<OutputMember> members)
        {
            List<Diagnostic> newDiagnostics = new();
            Phase1Entity result = entity;
            if (entity.Layout == LayoutAlgo.Linear)
            {
                // calculate field offsets for Linear layout
                List<OutputMember> updatedMembers = new();
                int blockLength = entity.BlockLength;
                blockLength = 0;
                int nextFieldOffset = 0;
                foreach (var member in members.OrderBy(m => m.Sequence))
                {
                    // calculate this offset
                    while (member.FieldLength > 0 && nextFieldOffset % member.FieldLength != 0)
                    {
                        nextFieldOffset++;
                    }
                    int fieldOffset = nextFieldOffset;

                    // calc next offset
                    nextFieldOffset = nextFieldOffset + member.FieldLength;
                    while (nextFieldOffset > blockLength)
                    {
                        blockLength = blockLength == 0 ? 1 : blockLength * 2;
                    }

                    // emit updated member
                    updatedMembers.Add(member with
                    {
                        FieldOffset = fieldOffset,
                    });
                }

                result = entity with
                {
                    BlockLength = blockLength,
                    Members = new EquatableArray<OutputMember>(updatedMembers),
                };
            }

            // check for layout issues
            var newDiagnostic = CheckMemberLayout(result.BlockLength, result.Members);
            if (newDiagnostic is not null)
            {
                newDiagnostics.Add(newDiagnostic);
            }

            if (newDiagnostics.Count == 0) return result;

            return result with
            {
                Diagnostics = new EquatableArray<Diagnostic>(entity.Diagnostics.Concat(newDiagnostics)),
            };
        }
    }
}
