using DTOMaker.SrcGen.Core;
using DTOMaker.SrcGen.MemBlocks.BlockLayout;
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

        private static readonly DiagnosticDescriptor DME06 = new DiagnosticDescriptor(nameof(DME06),
            "Invalid member length", "The member length must be a whole power of 2 between 1 and 1024", DiagnosticCategory.Design, DiagnosticSeverity.Error, true);

        private static readonly DiagnosticDescriptor DME07 = new DiagnosticDescriptor(nameof(DME07),
            "Invalid member offset", "The member offset must be zero or greater", DiagnosticCategory.Design, DiagnosticSeverity.Error, true);

        private static readonly DiagnosticDescriptor DME13 = new DiagnosticDescriptor(nameof(DME13),
            "Member layout issue", "Member overlaps another, is misaligned, or extends beyond the end of the block", DiagnosticCategory.Design, DiagnosticSeverity.Error, true);

        private static readonly SourceGeneratorParameters _parameters = new SourceGeneratorParameters()
        {
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
            // calculate structure code and block offset
            int blockOffset = 0;
            var structureCode = new StructureCode(inputEntity.ClassHeight, inputEntity.BlockLength);
            Phase2Entity? parent = allEntities.FirstOrDefault(e => e.TFN == inputEntity.BaseEntity?.TFN);
            while (parent is not null)
            {
                blockOffset += parent.BlockLength;
                structureCode = structureCode.AddInnerBlock(parent.ClassHeight, parent.BlockLength);
                parent = parent.BaseEntity is not null
                    ? allEntities.FirstOrDefault(e => e.TFN == parent.BaseEntity.TFN)
                    : null;
            }

            return outputEntity with
            {
                BlockStructureCode = structureCode.Bits,
                BlockOffset = blockOffset
            };
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

        private static int GetFieldLengthNotUsed(TypeFullName tfn)
        {
            string typeName = tfn.Impl.FullName;
            switch (typeName)
            {
                case KnownType.SystemBoolean:
                case KnownType.SystemByte:
                case KnownType.SystemSByte:
                    return 1;
                case KnownType.SystemInt16:
                case KnownType.SystemUInt16:
                case KnownType.SystemChar:
                case KnownType.SystemHalf:
                    return 2;
                case KnownType.SystemInt32:
                case KnownType.SystemUInt32:
                case KnownType.SystemSingle:
                case KnownType.PairOfInt16:
                    return 4;
                case KnownType.SystemInt64:
                case KnownType.SystemUInt64:
                case KnownType.SystemDouble:
                case KnownType.PairOfInt32:
                    return 8;
                case KnownType.SystemInt128:
                case KnownType.SystemUInt128:
                case KnownType.SystemGuid:
                case KnownType.SystemDecimal:
                case KnownType.PairOfInt64:
                case KnownType.QuadOfInt32:
                    return 16;
                default:
                    return 0;
            }
        }

        private static int GetFieldLength(MemberKind kind, NativeType fieldType)
        {
            return kind switch
            {
                MemberKind.Entity or MemberKind.String or MemberKind.Binary => 64,
                MemberKind.Struct => fieldType switch
                {
                    NativeType.Boolean
                        or NativeType.Byte
                        or NativeType.SByte => 1,
                    NativeType.Int16
                        or NativeType.UInt16
                        or NativeType.Char
                        or NativeType.Half => 2,
                    NativeType.Int32
                        or NativeType.UInt32
                        or NativeType.PairOfInt16
                        or NativeType.Single => 4,
                    NativeType.Int64
                        or NativeType.UInt64
                        or NativeType.PairOfInt32
                        or NativeType.Double => 8,
                    NativeType.Decimal
                        or NativeType.Int128
                        or NativeType.UInt128
                        or NativeType.PairOfInt64
                        or NativeType.QuadOfInt32
                        or NativeType.Guid => 16,
                    NativeType.String
                        or NativeType.Binary => 64,
                    _ => 0,
                },
                _ => 0,
            };
        }

        protected override ParsedMember OnCustomizeParsedMember(ParsedMember parsedMember, Location location)
        {
            // nothing to do yet
            return parsedMember;
        }

        private static Diagnostic? CheckMemberLayout(int blockLength, IEnumerable<OutputMember> members)
        {
            BitArray blockMap = new BitArray(blockLength);
            foreach (var member in members.OrderBy(m => m.Sequence))
            {
                if (member.FieldOffset < 0)
                    return Diagnostic.Create(DME07, member.Location);

                if (member.FieldOffset + member.FieldLength > blockLength)
                    return Diagnostic.Create(DME13, member.Location);

                if (member.FieldLength > 0 && (member.FieldOffset % member.FieldLength != 0))
                    return Diagnostic.Create(DME13, member.Location);

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
            // calculate field offsets
            int flagBitsFieldLength = GetFieldLength(MemberKind.Struct, NativeType.UInt32);
            int lastFlagBitsInstance = -1;
            int lastFlagBitsPosition = 31;
            var memberMap = members.ToDictionary(m => m.Sequence);
            var builder = new BlockMapBuilder();
            foreach (var member in members.OrderBy(m => m.Sequence))
            {
                int fieldLength = GetFieldLength(member.Kind, member.MemberType.NativeType);
                if (!IsValidFieldLength(fieldLength))
                {
                    newDiagnostics.Add(Diagnostic.Create(DME06, member.Location));
                }
                else
                {
                    if (member.Kind == MemberKind.Struct)
                    {
                        // allocate null bit for all structs to support nullability
                        lastFlagBitsPosition++;
                        if (lastFlagBitsPosition >= 32)
                        {
                            lastFlagBitsInstance++;
                            builder.AddField(
                                new InternalFieldDef($"_NullBitsField{lastFlagBitsInstance:D2}", 0, flagBitsFieldLength, lastFlagBitsInstance));
                            lastFlagBitsPosition = 0;
                        }
                        BitAddress nullAddress = new BitAddress(lastFlagBitsInstance, lastFlagBitsPosition, 0, 0);
                        builder.AddField(
                            new ExternalFieldDef(member.Name, 0, fieldLength, member.Sequence, nullAddress));
                    }
                    else
                    {
                        builder.AddField(
                            new ExternalFieldDef(member.Name, 0, fieldLength, member.Sequence, null));
                    }
                }
            }
            BlockMap blockMap = builder.Build();
            Dictionary<int, InternalFieldDef> internalFieldDefs = new Dictionary<int, InternalFieldDef>();
            foreach (FieldDef fd in blockMap.Fields.Array)
            {
                switch(fd)
                {
                    case InternalFieldDef ifd:
                        internalFieldDefs[ifd.Instance] = ifd;
                        break;
                    case ExternalFieldDef efd:
                        BitAddress? nullAddr = efd.NullAddress;
                        if (nullAddr is not null)
                        {
                            InternalFieldDef ifd = internalFieldDefs[nullAddr.Instance];
                            memberMap[efd.Sequence] = memberMap[efd.Sequence] with
                            {
                                FieldOffset = fd.Offset,
                                FieldLength = fd.Length,
                                NullAddress = nullAddr with { FieldOffset = ifd.Offset, FieldLength = ifd.Length },
                            };
                        }
                        else
                        {
                            memberMap[efd.Sequence] = memberMap[efd.Sequence] with
                            {
                                FieldOffset = fd.Offset,
                                FieldLength = fd.Length,
                            };
                        }
                        break;
                }
            }
            int blockLength = blockMap.BlockSize;
            Phase1Entity result = entity with
            {
                BlockLength = blockLength,
                Members = new EquatableArray<OutputMember>(memberMap.Values.OrderBy(m => m.Sequence)),
            };

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
