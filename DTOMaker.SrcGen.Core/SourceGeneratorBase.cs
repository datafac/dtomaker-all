using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace DTOMaker.SrcGen.Core
{
    public abstract class SourceGeneratorBase : IIncrementalGenerator
    {
        //private const string DomainAttribute = nameof(DomainAttribute);
        public const string EntityAttribute = nameof(EntityAttribute);
        public const string MemberAttribute = nameof(MemberAttribute);
        public const string ObsoleteAttribute = nameof(ObsoleteAttribute);
        public const string KeyOffsetAttribute = nameof(KeyOffsetAttribute);
        public const string LengthAttribute = nameof(LengthAttribute);
        public const string OffsetAttribute = nameof(OffsetAttribute);
        public const string EndianAttribute = nameof(EndianAttribute);
        public const int BlobIdV1Size = 64;

        protected abstract SourceGeneratorParameters OnBeginInitialize(IncrementalGeneratorInitializationContext context);
        protected abstract void OnEndInitialize(IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<OutputEntity> model);

        // determine the namespace the syntax node is declared in, if any
        static string GetNamespace(BaseTypeDeclarationSyntax syntax)
        {
            // If we don't have a namespace at all we'll return an empty string
            // This accounts for the "default namespace" case
            string nameSpace = string.Empty;

            // Get the containing syntax node for the type declaration
            // (could be a nested type, for example)
            SyntaxNode? potentialNamespaceParent = syntax.Parent;

            // Keep moving "out" of nested classes etc until we get to a namespace
            // or until we run out of parents
            while (potentialNamespaceParent != null &&
                    potentialNamespaceParent is not NamespaceDeclarationSyntax
                    && potentialNamespaceParent is not FileScopedNamespaceDeclarationSyntax)
            {
                potentialNamespaceParent = potentialNamespaceParent.Parent;
            }

            // Build up the final namespace by looping until we no longer have a namespace declaration
            if (potentialNamespaceParent is BaseNamespaceDeclarationSyntax namespaceParent)
            {
                // We have a namespace. Use that as the type
                nameSpace = namespaceParent.Name.ToString();

                // Keep moving "out" of the namespace declarations until we 
                // run out of nested namespace declarations
                while (true)
                {
                    if (namespaceParent.Parent is not NamespaceDeclarationSyntax parent)
                    {
                        break;
                    }

                    // Add the outer namespace as a prefix to the final namespace
                    nameSpace = $"{namespaceParent.Name}.{nameSpace}";
                    namespaceParent = parent;
                }
            }

            // return the final namespace
            return nameSpace;
        }

        protected static Diagnostic? TryGetAttributeArgumentValue<T>(AttributeData attrData, Location location, int index, Action<T> action)
        {
            object? input = attrData.ConstructorArguments[index].Value;
            if (input is T value)
            {
                action(value);
                return null;
            }

            string inputAsStr = input is null ? "(null)" : $"'{input}' <{input.GetType().Name}>";

            return Diagnostic.Create(DiagnosticsEN.DME02, location);
        }

        private static Diagnostic? CheckAttributeArguments(AttributeData attrData, Location location, int expectedCount)
        {
            var attrArgs = attrData.ConstructorArguments;
            if (attrArgs.Length == expectedCount)
                return null;

            return Diagnostic.Create(DiagnosticsEN.DME01, location);
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

        private static int GetFieldLength(TypeFullName tfn)
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
                    return 16;
                default:
                    return 0;
            }
        }

        private static ParsedMember? GetParsedMember(GeneratorAttributeSyntaxContext ctx, SourceGeneratorParameters srcGenParams)
        {
            List<Diagnostic> diagnostics = new();
            SemanticModel semanticModel = ctx.SemanticModel;
            SyntaxNode syntaxNode = ctx.TargetNode;
            Location location = syntaxNode.GetLocation();

            if (syntaxNode is not PropertyDeclarationSyntax propDeclarationSyntax)
            {
                // something went wrong
                return null;
            }

            // Get the semantic representation of the enum syntax
            ISymbol? declSynbol = semanticModel.GetDeclaredSymbol(propDeclarationSyntax);
            if (declSynbol is not IPropertySymbol propSymbol)
            {
                // something went wrong
                return null;
            }

            string fullname = propSymbol.ToString();

            (TypeFullName tfn, MemberKind kind, bool isNullable) = GetTypeInfo(propSymbol.Type, srcGenParams.ImplSpaceSuffix);

            // Get the namespace the enum is declared in, if any
            int sequence = 0;
            bool isObsolete = false;
            string obsoleteMessage = string.Empty;
            bool obsoleteIsError = false;
            int fieldOffset = 0;
            int fieldLength = GetFieldLength(tfn);
            bool isBigEndian = false;
            bool isExternal = false;

            // Loop through all of the attributes on the interface
            foreach (AttributeData attributeData in propSymbol.GetAttributes())
            {
                string? attrName = attributeData.AttributeClass?.Name;
                Diagnostic? diagnostic = null;
                switch (attrName)
                {
                    case null:
                        break;
                    case MemberAttribute:
                        // get sequence
                        diagnostic
                            = CheckAttributeArguments(attributeData, location, 1)
                            ?? TryGetAttributeArgumentValue<int>(attributeData, location, 0, (value) => { sequence = value; });
                        break;
                    case ObsoleteAttribute:
                        isObsolete = true;
                        var attributeArguments = attributeData.ConstructorArguments;
                        diagnostic = attributeArguments.Length switch
                        {
                            0 => null,
                            1 => TryGetAttributeArgumentValue<string>(attributeData, location, 0, (value) => { obsoleteMessage = value; }),
                            2 => TryGetAttributeArgumentValue<string>(attributeData, location, 0, (value) => { obsoleteMessage = value; })
                                ?? TryGetAttributeArgumentValue<bool>(attributeData, location, 1, (value) => { obsoleteIsError = value; }),
                            _ => Diagnostic.Create(DiagnosticsEN.DME01, location),
                        };
                        break;
                    case LengthAttribute: // used by MemBlocks
                        diagnostic
                            = CheckAttributeArguments(attributeData, location, 1)
                            ?? TryGetAttributeArgumentValue<int>(attributeData, location, 0, (value) => { fieldLength = value; });
                        break;
                    case OffsetAttribute: // used by MemBlocks
                        diagnostic
                            = CheckAttributeArguments(attributeData, location, 1)
                            ?? TryGetAttributeArgumentValue<int>(attributeData, location, 0, (value) => { fieldOffset = value; });
                        break;
                    case EndianAttribute: // used by MemBlocks
                        diagnostic
                            = CheckAttributeArguments(attributeData, location, 1)
                            ?? TryGetAttributeArgumentValue<bool>(attributeData, location, 0, (value) => { isBigEndian = value; });
                        break;
                    default:
                        // ignore other attributes
                        diagnostic = null;
                        break;
                }

                if (diagnostic is not null)
                {
                    diagnostics.Add(diagnostic);
                }
            }

            if (sequence <= 0)
            {
                diagnostics.Add(Diagnostic.Create(DiagnosticsEN.DME04, location));
            }

            if (kind == MemberKind.Unknown)
            {
                diagnostics.Add(Diagnostic.Create(DiagnosticsEN.DME10, location));
            }

            if (srcGenParams.GeneratorId == GeneratorId.MemBlocks)
            {
                // set default length for Octets and String
                if (fieldLength == 0 && (kind == MemberKind.String || kind == MemberKind.Binary))
                {
                    fieldLength = BlobIdV1Size;
                    isExternal = true;
                }
                // set default length for Octets and String
                if (kind == MemberKind.Entity)
                {
                    fieldLength = BlobIdV1Size;
                    isExternal = true;
                }

                // checks
                if (!IsValidFieldLength(fieldLength))
                {
                    diagnostics.Add(Diagnostic.Create(DiagnosticsEN.DME06, location));
                }
                if (fieldOffset < 0)
                {
                    diagnostics.Add(Diagnostic.Create(DiagnosticsEN.DME07, location));
                }
                if (isNullable && kind == MemberKind.Native)
                {
                    diagnostics.Add(Diagnostic.Create(DiagnosticsEN.DME08, location));
                }
            }

            return new ParsedMember(location, fullname, sequence, tfn, kind, isNullable, diagnostics)
            {
                ObsoleteInfo = isObsolete
                    ? new ObsoleteInformation() { Message = obsoleteMessage, IsError = obsoleteIsError }
                    : null,
                FieldOffset = fieldOffset,
                FieldLength = fieldLength,
                IsBigEndian = isBigEndian,
                IsExternal = isExternal,
            };
        }

        private static (TypeFullName tfn, MemberKind kind, bool isNullable) GetTypeInfo(ITypeSymbol typeSymbol, string implSpaceSuffix)
        {
            TypeFullName tfn = new TypeFullName(typeSymbol, implSpaceSuffix);
            MemberKind kind = tfn.MemberKind;
            bool isNullable = false;
            if (typeSymbol is INamedTypeSymbol namedTypeSymbol)
            {
                if (namedTypeSymbol.IsGenericType && namedTypeSymbol.Name == "Nullable" && namedTypeSymbol.TypeArguments.Length == 1)
                {
                    // nullable value type
                    isNullable = true;
                    ITypeSymbol typeArg0 = namedTypeSymbol.TypeArguments[0];
                    tfn = new TypeFullName(typeArg0, implSpaceSuffix);
                    kind = tfn.MemberKind;
                }
            }
            if (typeSymbol.NullableAnnotation == NullableAnnotation.Annotated)
            {
                // nullable ref type
                isNullable = true;
            }
            return (tfn, kind, isNullable);
        }

        protected abstract ParsedEntity OnCustomizeParsedEntity(ParsedEntity parsedEntity, Location location);

        private ParsedEntity? GetParsedEntity(GeneratorAttributeSyntaxContext ctx, SourceGeneratorParameters srcGenParams)
        {
            List<Diagnostic> diagnostics = new();
            SemanticModel semanticModel = ctx.SemanticModel;
            SyntaxNode syntaxNode = ctx.TargetNode;
            Location location = syntaxNode.GetLocation();

            if (syntaxNode is not InterfaceDeclarationSyntax intfDeclarationSyntax)
            {
                // something went wrong
                return null;
            }

            // Get the semantic representation of the enum syntax
            if (semanticModel.GetDeclaredSymbol(intfDeclarationSyntax) is not INamedTypeSymbol intfSymbol)
            {
                // something went wrong
                return null;
            }

            // Get the namespace the enum is declared in, if any
            //string generatedNamespace = GetNamespace(intfDeclarationSyntax);
            int entityId = 0;
            int keyOffset = 0;
            LayoutAlgo layoutAlgo = LayoutAlgo.Default;
            int blockLength = 0;

            // Loop through all of the attributes on the interface
            foreach (AttributeData attributeData in intfSymbol.GetAttributes())
            {
                string? attrName = attributeData.AttributeClass?.Name;
                Diagnostic? diagnostic = null;
                switch (attrName)
                {
                    case EntityAttribute:
                        // get entity id and layout algo
                        diagnostic =
                            CheckAttributeArguments(attributeData, location, 2)
                            ?? TryGetAttributeArgumentValue<int>(attributeData, location, 0, (value) => { entityId = value; })
                            ?? TryGetAttributeArgumentValue<int>(attributeData, location, 1, (value) => { layoutAlgo = (LayoutAlgo)value; });
                        break;
                    case KeyOffsetAttribute: // used by MessagePack 
                        diagnostic =
                            CheckAttributeArguments(attributeData, location, 1)
                            ?? TryGetAttributeArgumentValue<int>(attributeData, location, 0, (value) => { keyOffset = value; });
                        break;
                    case LengthAttribute: // used by MemBlocks
                        diagnostic
                            = CheckAttributeArguments(attributeData, location, 1)
                            ?? TryGetAttributeArgumentValue<int>(attributeData, location, 0, (value) => { blockLength = value; });
                        break;
                    default:
                        // ignore other attributes
                        diagnostic = null;
                        break;
                }

                if (diagnostic is not null)
                {
                    diagnostics.Add(diagnostic);
                }
            }

            if (entityId <= 0)
            {
                diagnostics.Add(Diagnostic.Create(DiagnosticsEN.DME03, location));
            }

            var baseIntf = intfSymbol.Interfaces.FirstOrDefault();
            TypeFullName? baseTFN = baseIntf is not null ? new TypeFullName(baseIntf, srcGenParams.ImplSpaceSuffix) : null;
            var result = new ParsedEntity(location, new TypeFullName(intfSymbol, srcGenParams.ImplSpaceSuffix), entityId, baseTFN, diagnostics)
            {
                KeyOffset = keyOffset,
                BlockLength = blockLength,
                Layout = layoutAlgo,
            };

            result = OnCustomizeParsedEntity(result, location);

            return result;
        }

        private static int GetClassHeight(ParsedEntity thisEntity, ImmutableArray<ParsedEntity> allEntities)
        {
            if (thisEntity.BaseTFN is null) return 1; //we are root
            var parentEntity = allEntities.FirstOrDefault(e => e.TFN.Intf == thisEntity.BaseTFN?.Intf);
            if (parentEntity is null) return 1; // parent not found
            return 1 + GetClassHeight(parentEntity, allEntities);
        }

        private static bool HasValidBase(ParsedEntity thisEntity, ImmutableArray<ParsedEntity> allEntities)
        {
            if (thisEntity.BaseTFN is null) return false;

            // special case for IEntityBase
            if (thisEntity.BaseTFN.Value.Intf.Name == SpecialName.RuntimeBaseIntfName
                && thisEntity.BaseTFN.Value.Intf.Space == SpecialName.RuntimeNamespace) return true;

            var baseEntity = allEntities.FirstOrDefault(e => e.TFN.Intf == thisEntity.BaseTFN?.Intf);
            if (baseEntity is null)
                return false;
            else
                return true;
        }

        private static List<Phase1Entity> GetDerivedEntities1(TypeFullName parentTFN, ImmutableArray<Phase1Entity> allEntities)
        {
            var derivedEntities = new List<Phase1Entity>();
            foreach (var entity in allEntities)
            {
                if (entity.BaseTFN == parentTFN)
                {
                    // found derived
                    derivedEntities.Add(entity);
                    // now recurse
                    derivedEntities.AddRange(GetDerivedEntities1(entity.TFN, allEntities));
                }
            }
            return derivedEntities;
        }

        private static List<Phase2Entity> GetDerivedEntities2(TypeFullName parentTFN, ImmutableArray<Phase2Entity> allEntities)
        {
            var derivedEntities = new List<Phase2Entity>();
            foreach (var entity in allEntities)
            {
                if (entity.BaseEntity is not null && entity.BaseEntity.TFN == parentTFN)
                {
                    // found derived
                    derivedEntities.Add(entity);
                    // now recurse
                    derivedEntities.AddRange(GetDerivedEntities2(entity.TFN, allEntities));
                }
            }
            return derivedEntities;
        }

        public static Diagnostic? CheckMemberLayout(int blockLength, IEnumerable<OutputMember> members)
        {
            BitArray blockMap = new BitArray(blockLength);
            foreach (var member in members.OrderBy(m => m.Sequence))
            {
                if (member.FieldOffset + member.FieldLength > blockLength)
                {
                    return Diagnostic.Create(DiagnosticsEN.DME13, member.Location);
                }

                if (member.FieldLength > 0 && (member.FieldOffset % member.FieldLength != 0))
                {
                    return Diagnostic.Create(DiagnosticsEN.DME13, member.Location);
                }

                // check value bytes layout
                for (var i = 0; i < member.FieldLength; i++)
                {
                    int offset = member.FieldOffset + i;
                    if (blockMap.Get(offset))
                    {
                        return Diagnostic.Create(DiagnosticsEN.DME13, member.Location);
                    }

                    // not assigned
                    blockMap.Set(offset, true);
                }
            }
            return null;
        }

        internal static Phase1Entity ResolveMembers(ParsedEntity entity, ImmutableArray<ParsedMember> members, ImmutableArray<ParsedEntity> entities, SourceGeneratorParameters srcGenParams)
        {
            string prefix = entity.TFN.Intf.FullName + ".";
            var outputMembers = new List<OutputMember>();
            var newDiagnostics = new List<Diagnostic>();
            int expectedMemberSequence = 1;
            int blockLength = entity.BlockLength;
            // MemBlocks: calculate block length for Linear layout
            if (srcGenParams.GeneratorId == GeneratorId.MemBlocks && entity.Layout == LayoutAlgo.Linear)
            {
                blockLength = 0;
            }
            int nextFieldOffset = 0;
            foreach (ParsedMember member in members.OrderBy(m => m.Sequence))
            {
                if (member.FullName.StartsWith(prefix, StringComparison.Ordinal))
                {
                    // check for member sequence issues
                    if (member.Sequence != expectedMemberSequence)
                    {
                        newDiagnostics.Add(Diagnostic.Create(DiagnosticsEN.DME11, member.Location));
                    }
                    expectedMemberSequence++;

                    // MemBlocks: calculate member offset for Linear layout
                    int fieldOffset = member.FieldOffset;
                    int fieldLength = member.FieldLength;
                    if (srcGenParams.GeneratorId == GeneratorId.MemBlocks && entity.Layout == LayoutAlgo.Linear)
                    {
                        // calculate this offset
                        while (fieldLength > 0 && nextFieldOffset % fieldLength != 0)
                        {
                            nextFieldOffset++;
                        }
                        fieldOffset = nextFieldOffset;

                        // calc next offset
                        nextFieldOffset = nextFieldOffset + fieldLength;
                        while (nextFieldOffset > blockLength)
                        {
                            blockLength = blockLength == 0 ? 1 : blockLength * 2;
                        }
                    }

                    // emit member
                    outputMembers.Add(new OutputMember()
                    {
                        Location = member.Location,
                        Name = member.PropName,
                        Sequence = member.Sequence,
                        MemberType = member.MemberType,
                        Kind = member.Kind,
                        IsNullable = member.IsNullable,
                        ObsoleteInfo = member.ObsoleteInfo,
                        Diagnostics = member.Diagnostics,
                        FieldOffset = fieldOffset,
                        FieldLength = fieldLength,
                        IsBigEndian = member.IsBigEndian,
                        IsExternal = member.IsExternal,
                    });
                }
            }
            int classHeight = GetClassHeight(entity, entities);

            // check base is valid
            if (!HasValidBase(entity, entities))
            {
                newDiagnostics.Add(Diagnostic.Create(DiagnosticsEN.DME14, entity.Location));
            }

            if (srcGenParams.GeneratorId == GeneratorId.MemBlocks)
            {
                // check for MemBlocks layout issues
                var diagnostic = CheckMemberLayout(blockLength, outputMembers);
                if (diagnostic is not null)
                {
                    newDiagnostics.Add(diagnostic);
                }
            }

            return new Phase1Entity()
            {
                Location = entity.Location,
                TFN = entity.TFN,
                EntityId = entity.EntityId,
                ClassHeight = classHeight,
                Members = new EquatableArray<OutputMember>(outputMembers.OrderBy(m => m.Sequence)),
                BaseTFN = entity.BaseTFN,
                Diagnostics = newDiagnostics.Count > 0
                    ? new EquatableArray<Diagnostic>(entity.Diagnostics.Concat(newDiagnostics))
                    : entity.Diagnostics,
                KeyOffset = entity.KeyOffset,
                BlockLength = blockLength,
                Layout = entity.Layout,
            };
        }

        public static Phase2Entity ResolveEntities1(Phase1Entity entity, ImmutableArray<Phase1Entity> allEnts)
        {
            var newDiagnostics = new List<Diagnostic>();
            var baseEntity = allEnts.FirstOrDefault(e => e.TFN == entity.BaseTFN);
            List<Phase1Entity> derivedEntities = GetDerivedEntities1(entity.TFN, allEnts);

            // check entity id uniqueness
            int duplicateCount = allEnts.Count(e => e.EntityId == entity.EntityId);
            if (duplicateCount > 1)
            {
                newDiagnostics.Add(Diagnostic.Create(DiagnosticsEN.DME12, entity.Location));
            }
            return new Phase2Entity()
            {
                TFN = entity.TFN,
                EntityId = entity.EntityId,
                ClassHeight = entity.ClassHeight,
                Members = entity.Members,
                BaseEntity = baseEntity,
                DerivedEntities = new EquatableArray<Phase1Entity>(derivedEntities.OrderBy(e => e.TFN.Intf.FullName)),
                Diagnostics = newDiagnostics.Count > 0
                    ? new EquatableArray<Diagnostic>(entity.Diagnostics.Concat(newDiagnostics))
                    : entity.Diagnostics,
                KeyOffset = entity.KeyOffset,
                BlockLength = entity.BlockLength,
                Layout = entity.Layout,
            };
        }

        protected abstract OutputEntity OnCustomizeOutputEntity(OutputEntity outputEntity, Phase2Entity inputEntity, ImmutableArray<Phase2Entity> allEntities);
        public OutputEntity ResolveEntities2(Phase2Entity entity, ImmutableArray<Phase2Entity> allEnts)
        {
            var baseEntity = allEnts.FirstOrDefault(e => e.TFN == entity.BaseEntity?.TFN);
            List<Phase2Entity> derivedEntities = GetDerivedEntities2(entity.TFN, allEnts);

            var result = new OutputEntity()
            {
                TFN = entity.TFN,
                EntityId = entity.EntityId,
                ClassHeight = entity.ClassHeight,
                Members = entity.Members,
                BaseEntity = baseEntity,
                DerivedEntities = new EquatableArray<Phase2Entity>(derivedEntities.OrderBy(e => e.TFN.Intf.FullName)),
                Diagnostics = entity.Diagnostics,
                KeyOffset = entity.KeyOffset,
                BlockLength = entity.BlockLength,
            };

            result = OnCustomizeOutputEntity(result, entity, allEnts);

            return result;
        }

        protected void EmitDiagnostics(SourceProductionContext spc, OutputEntity ent)
        {
            foreach (var diagnostic in ent.Diagnostics)
            {
                spc.ReportDiagnostic(diagnostic);
            }
            foreach (var member in ent.Members)
            {
                foreach (var diagnostic in member.Diagnostics)
                {
                    spc.ReportDiagnostic(diagnostic);
                }
            }
        }

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            // do derived stuff
            SourceGeneratorParameters srcGenParams = OnBeginInitialize(context);
            //string implSpaceSuffix = srcGenParams.ImplSpaceSuffix!;


            // filter for entities
            IncrementalValuesProvider<ParsedEntity> parsedEntities1 = context.SyntaxProvider
                .ForAttributeWithMetadataName(
                    "DTOMaker.Models.EntityAttribute",
                    predicate: static (syntaxNode, _) => syntaxNode is InterfaceDeclarationSyntax,
                    transform: (ctx, _) => GetParsedEntity(ctx, srcGenParams))
                .Where(static e => e is not null)!;

            // filter for Members
            IncrementalValuesProvider<ParsedMember> parsedMembers = context.SyntaxProvider
                .ForAttributeWithMetadataName(
                    "DTOMaker.Models.MemberAttribute",
                    predicate: static (syntaxNode, _) => syntaxNode is PropertyDeclarationSyntax,
                    transform: (ctx, _) => GetParsedMember(ctx, srcGenParams))
                .Where(static m => m is not null)!;

            // add base entity
            //IncrementalValuesProvider<ParsedEntity> parsedEntities2 = parsedEntities1.Collect().Select((list1, _) => AddEntityBase(list1, implSpaceSuffix)).SelectMany((list2, _) => list2.ToImmutableArray());

            var parsedMatrix = parsedEntities1.Collect().Combine(parsedMembers.Collect());

            // resolve members and class height
            IncrementalValuesProvider<Phase1Entity> phase1Entities = parsedEntities1.Combine(parsedMatrix)
                .Select((pair, _) =>
                {
                    var entity = pair.Left;
                    var members = pair.Right.Right;
                    var allents = pair.Right.Left;
                    return ResolveMembers(entity, members, allents, srcGenParams);
                });

            // generate closed generic entities
            //IncrementalValuesProvider<Phase2Entity> phase2Entities = phase1Entities.Combine(phase1Entities.Where(e => e.TFN.IsGeneric && !e.TFN.IsClosed).Collect())
            //    .Select((pair, _) =>
            //    {
            //        var entity = pair.Left;
            //        var openGenericEntities = pair.Right;
            //        return new Phase2Entity()
            //        {
            //            TFN = entity.TFN,
            //            EntityId = entity.EntityId,
            //            ClassHeight = entity.ClassHeight,
            //            Members = entity.Members,
            //            BaseEntity = baseEntity,
            //            DerivedEntities = new EquatableArray<Phase1Entity>(derivedEntities.OrderBy(e => e.Intf.FullName))
            //            Diagnostics = entity.Diagnostics,    
            //        };
            //    });

            // resolve base entity and derived entities
            IncrementalValuesProvider<Phase2Entity> phase2Entities = phase1Entities.Combine(phase1Entities.Collect())
                .Select((pair, _) =>
                {
                    var entity = pair.Left;
                    var allEnts = pair.Right;
                    return ResolveEntities1(entity, allEnts);
                })
                .Where(e => e.TFN.IsClosed);

            IncrementalValuesProvider<OutputEntity> outputEntities = phase2Entities.Combine(phase2Entities.Collect())
                .Select((pair, _) =>
                {
                    var entity = pair.Left;
                    var allEnts = pair.Right;
                    return ResolveEntities2(entity, allEnts);
                })
                .Where(e => e.TFN.IsClosed);

            // generate summary
            //context.RegisterSourceOutput(outputEntities.Collect(), (spc, entities) =>
            //{
            //    var sb = new StringBuilder();
            //    sb.AppendLine("// <auto-generated/>");
            //    sb.AppendLine("// Entities:");
            //    foreach (var entity in entities)
            //    {
            //        sb.AppendLine($"// - {entity.NameSpace}.{entity.IntfName} ({entity.Members.Count} members)");
            //    }
            //    sb.AppendLine("// End.");
            //    spc.AddSource("Metadata.Summary.g.cs", SourceText.From(sb.ToString(), Encoding.UTF8));
            //});

            // emit metadata in json format
            //IncrementalValueProvider<string?> projectDirProvider = context.AnalyzerConfigOptionsProvider
            //    .Select(static (provider, _) =>
            //    {
            //        provider.GlobalOptions.TryGetValue("build_property.projectdir", out string? projectDirectory);
            //        return projectDirectory;
            //    });

            //string? projectDirectory = null;
            //context.RegisterSourceOutput(
            //    projectDirProvider,
            //    (context, source) =>
            //    {
            //        projectDirectory = source;
            //    });

            //IncrementalValuesProvider<ModelEntity> modelEntities = parsedEntities
            //    .Select((mi, _) => new ModelEntity
            //    {
            //        NameSpace = mi.NameSpace,
            //        IntfName = mi.IntfName,
            //        EntityId = mi.EntityId,
            //        Members = new EquatableArray<ModelMember>(mi.Values.Select(v => new ModelMember { PropName = v }).ToArray())
            //    });
            //context.RegisterSourceOutput(modelEntities.Collect(), (spc, entities) =>
            //{
            //    ModelMetadata metadata = new()
            //    {
            //        Entities = new EquatableArray<ModelEntity>(entities.ToArray())
            //    };
            //    string metadataText = metadata.ToString();
            //    spc.AddSource("Metadata.g.json", SourceText.From(metadataText, Encoding.UTF8));
            //});

            // emit entity related diagnostics
            context.RegisterSourceOutput(outputEntities, EmitDiagnostics);

            context.RegisterSourceOutput(context.CompilationProvider, (spc, compilation) =>
            {
                // This is a way to check that the source generator is running
                // You can remove this diagnostic if you don't need it
                spc.ReportDiagnostic(Diagnostic.Create(DiagnosticsEN.INF01, Location.None));
            });

            // do derived stuff
            OnEndInitialize(context, outputEntities);
        }
    }
}
