using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace DTOMaker.SrcGen.Core
{
    public abstract class SourceGeneratorBase : IIncrementalGenerator
    {
        protected abstract void OnBeginInitialize(IncrementalGeneratorInitializationContext context);
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

        //private const string DomainAttribute = nameof(DomainAttribute);
        private const string EntityAttribute = nameof(EntityAttribute);
        private const string MemberAttribute = nameof(MemberAttribute);
        private const string IdAttribute = nameof(IdAttribute);
        private const string ObsoleteAttribute = nameof(ObsoleteAttribute);

        protected static SyntaxDiagnostic? TryGetAttributeArgumentValue<T>(AttributeData attrData, Location location, int index, Action<T> action)
        {
            object? input = attrData.ConstructorArguments[index].Value;
            if (input is T value)
            {
                action(value);
                return null;
            }

            string inputAsStr = input is null ? "(null)" : $"'{input}' <{input.GetType().Name}>";

            return
                new SyntaxDiagnostic(
                    DiagnosticId.DTOM0005, "Invalid argument value", DiagnosticCategory.Syntax, location, DiagnosticSeverity.Error,
                    $"Could not read arg[{index}] {inputAsStr} as <{typeof(T).Name}>");
        }

        private static SyntaxDiagnostic? CheckAttributeArguments(AttributeData attrData, Location location, int expectedCount)
        {
            var attrArgs = attrData.ConstructorArguments;
            if (attrArgs.Length == expectedCount)
                return null;

            return new SyntaxDiagnostic(
                    DiagnosticId.DTOM0002, "Invalid argument count", DiagnosticCategory.Syntax, location, DiagnosticSeverity.Error,
                    $"Expected {attrData.AttributeClass?.Name} attribute to have {expectedCount} arguments, but it has {attrArgs.Length}.");
        }

        private static ParsedMember? GetParsedMember(GeneratorAttributeSyntaxContext ctx)
        {
            //List<SyntaxDiagnostic> syntaxErrors = new();
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

            // Get the namespace the enum is declared in, if any
            int sequence = 0;
            bool isObsolete = false;
            string obsoleteMessage = string.Empty;
            bool obsoleteIsError = false;

            // Loop through all of the attributes on the interface
            foreach (AttributeData attributeData in propSymbol.GetAttributes())
            {
                string? attrName = attributeData.AttributeClass?.Name;
                SyntaxDiagnostic? diagnostic = null;
                switch (attrName)
                {
                    case null:
                        break;
                    case MemberAttribute:
                        // get sequence
                        diagnostic =
                            CheckAttributeArguments(attributeData, location, 1)
                            ?? TryGetAttributeArgumentValue<int>(attributeData, location, 0, (value) => { sequence = value; });
                        break;
                    case ObsoleteAttribute:
                        isObsolete = true;
                        var attributeArguments = attributeData.ConstructorArguments;
                        if (attributeArguments.Length == 1)
                        {
                            TryGetAttributeArgumentValue<string>(attributeData, location, 0, (value) => { obsoleteMessage = value; });
                        }
                        if (attributeArguments.Length == 2)
                        {
                            TryGetAttributeArgumentValue<string>(attributeData, location, 0, (value) => { obsoleteMessage = value; });
                            TryGetAttributeArgumentValue<bool>(attributeData, location, 1, (value) => { obsoleteIsError = value; });
                        }
                        break;
                    default:
                        // todo pass to derived
                        diagnostic = new SyntaxDiagnostic(
                            "WRN001", "Ignored unknown attribute", DiagnosticCategory.Other, location, DiagnosticSeverity.Warning,
                            $"The attribute '{attrName}' is not recognized.");
                        break;
                }

                if (diagnostic is not null)
                {
                    //syntaxErrors.Add(diagnostic);
                }
            }

            if (sequence <= 0)
            {
                //syntaxErrors.Add(new SyntaxDiagnostic(
                //    "ERR001", "Missing or invalid member sequence", DiagnosticCategory.Syntax, syntaxNode.GetLocation(), DiagnosticSeverity.Error,
                //    $"The interface '{propSymbol.Name}' must have a valid Id attribute with a positive integer value."));
            }

            // Get the full type name of the enum e.g. Colour, 
            // or OuterClass<T>.Colour if it was nested in a generic type (for example)
            string fullname = propSymbol.ToString();

            (TypeFullName tfn, MemberKind kind, bool isNullable) = GetTypeInfo(propSymbol.Type);

            return new ParsedMember(fullname, sequence, tfn, kind, isNullable, isObsolete, obsoleteMessage, obsoleteIsError);
        }

        private static (TypeFullName tfn, MemberKind kind, bool isNullable) GetTypeInfo(ITypeSymbol typeSymbol)
        {
            TypeFullName tfn = new TypeFullName(typeSymbol);
            MemberKind kind = tfn.MemberKind;
            bool isNullable = false;
            if (typeSymbol is INamedTypeSymbol namedTypeSymbol)
            {
                if (namedTypeSymbol.IsGenericType && namedTypeSymbol.Name == "Nullable" && namedTypeSymbol.TypeArguments.Length == 1)
                {
                    // nullable value type
                    isNullable = true;
                    ITypeSymbol typeArg0 = namedTypeSymbol.TypeArguments[0];
                    tfn = new TypeFullName(typeArg0);
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

        private static MemberKind GetMemberKind(ITypeSymbol typeSymbol)
        {
            string fullname = typeSymbol.ToString();
            return fullname switch
            {
                KnownType.SystemBoolean => MemberKind.Native,
                KnownType.SystemSByte => MemberKind.Native,
                KnownType.SystemByte => MemberKind.Native,
                KnownType.SystemInt16 => MemberKind.Native,
                KnownType.SystemUInt16 => MemberKind.Native,
                KnownType.SystemChar => MemberKind.Native,
                KnownType.SystemHalf => MemberKind.Native,
                KnownType.SystemInt32 => MemberKind.Native,
                KnownType.SystemUInt32 => MemberKind.Native,
                KnownType.SystemSingle => MemberKind.Native,
                KnownType.SystemInt64 => MemberKind.Native,
                KnownType.SystemUInt64 => MemberKind.Native,
                KnownType.SystemDouble => MemberKind.Native,
                KnownType.SystemInt128 => MemberKind.Native,
                KnownType.SystemUInt128 => MemberKind.Native,
                KnownType.SystemGuid => MemberKind.Native,
                KnownType.SystemDecimal => MemberKind.Native,
                KnownType.SystemString => MemberKind.String,
                // custom types
                KnownType.PairOfInt16 => MemberKind.Native,
                KnownType.PairOfInt32 => MemberKind.Native,
                KnownType.PairOfInt64 => MemberKind.Native,
                KnownType.MemoryOctets => MemberKind.Binary,
                _ => MemberKind.Unknown,
            };
        }

        private static ParsedEntity? GetParsedEntity(GeneratorAttributeSyntaxContext ctx)
        {
            //List<SyntaxDiagnostic> syntaxErrors = new();
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

            // Loop through all of the attributes on the interface
            foreach (AttributeData attributeData in intfSymbol.GetAttributes())
            {
                string? attrName = attributeData.AttributeClass?.Name;
                SyntaxDiagnostic? diagnostic = null;
                switch (attrName)
                {
                    case null:
                        break;
                    //DomainAttribute => null,
                    case EntityAttribute:
                        break;
                    case MemberAttribute:
                        break;
                    case IdAttribute:
                        // get entity id
                        diagnostic =
                            CheckAttributeArguments(attributeData, location, 1)
                            ?? TryGetAttributeArgumentValue<int>(attributeData, location, 0, (value) => { entityId = value; });
                        break;
                    default:
                        // todo pass to derived
                        diagnostic = new SyntaxDiagnostic(
                            "WRN001", "Ignored unknown attribute", DiagnosticCategory.Other, location, DiagnosticSeverity.Warning,
                            $"The attribute '{attrName}' is not recognized.");
                        break;
                }

                if (diagnostic is not null)
                {
                    //syntaxErrors.Add(diagnostic);
                }
            }

            if (entityId <= 0)
            {
                //syntaxErrors.Add(new SyntaxDiagnostic(
                //    "ERR001", "Missing or invalid Id", DiagnosticCategory.Syntax, syntaxNode.GetLocation(), DiagnosticSeverity.Error,
                //    $"The interface '{intfSymbol.Name}' must have a valid Id attribute with a positive integer value."));
            }

            // Get the full type name of the enum e.g. Colour, 
            // or OuterClass<T>.Colour if it was nested in a generic type (for example)
            //string fullname = intfSymbol.ToString();

            // Get all the members in the enum
            ImmutableArray<ISymbol> intfMembers = intfSymbol.GetMembers();
            var members = new List<string>(intfMembers.Length);

            // Get all the fields from the enum, and add their name to the list
            //foreach (ISymbol member in intfMembers)
            //{
            //    if (member is IFieldSymbol field && field.ConstantValue is not null)
            //    {
            //        members.Add(member.Name);
            //    }
            //}

            var baseIntf = intfSymbol.Interfaces.FirstOrDefault();
            TypeFullName? baseTFN = baseIntf is not null ? new TypeFullName(baseIntf) : null;
            return new ParsedEntity(new TypeFullName(intfSymbol), entityId, baseTFN);
        }

        private static int GetClassHeight(ParsedEntity thisEntity, ImmutableArray<ParsedEntity> allEntities)
        {
            if (thisEntity.BaseTFN is null) return 0; //we are root
            var parentEntity = allEntities.FirstOrDefault(e => e.TFN.Intf == thisEntity.BaseTFN?.Intf);
            if (parentEntity is null) return 0; // parent not found
            if (parentEntity.BaseTFN is null) return 1; // parent is root (shortcut)
            return 1 + GetClassHeight(parentEntity, allEntities);
        }

        private static List<Phase1Entity> GetDerivedEntities(TypeFullName parentTFN, ImmutableArray<Phase1Entity> allEntities)
        {
            var derivedEntities = new List<Phase1Entity>();
            foreach (var entity in allEntities)
            {
                if (entity.BaseTFN == parentTFN)
                {
                    // found derived
                    derivedEntities.Add(entity);
                    // now recurse
                    derivedEntities.AddRange(GetDerivedEntities(entity.TFN, allEntities));
                }
            }
            return derivedEntities;
        }

        public static ImmutableArray<ParsedEntity> AddEntityBase(ImmutableArray<ParsedEntity> parsedEntities)
        {
            // add base entity
            var baseEntityIntf = new ParsedName("DTOMaker.Runtime.IEntityBase");
            var baseEntityImpl = new ParsedName("DTOMaker.Runtime.JsonSystemText.EntityBase");
            var baseEntityTFN = new TypeFullName(baseEntityIntf, baseEntityImpl, MemberKind.Entity);
            var baseEntity = new ParsedEntity(baseEntityTFN, 0, null);
            var builder = ImmutableArray<ParsedEntity>.Empty.ToBuilder();
            builder.Add(baseEntity);
            builder.AddRange(parsedEntities);
            // add closed generic entities
            // todo
            return builder.ToImmutable();
        }

        public static Phase1Entity ResolveMembers(ParsedEntity entity, ImmutableArray<ParsedMember> members, ImmutableArray<ParsedEntity> entities)
        {
            string prefix = entity.Intf.FullName + ".";
            var outputMembers = new List<OutputMember>();
            foreach (ParsedMember member in members)
            {
                if (member.FullName.StartsWith(prefix, StringComparison.Ordinal))
                {
                    outputMembers.Add(new OutputMember()
                    {
                        Name = member.PropName,
                        Sequence = member.Sequence,
                        MemberType = member.MemberType,
                        Kind = member.Kind,
                        IsNullable = member.IsNullable,
                        IsObsolete = member.IsObsolete,
                        ObsoleteMessage = member.ObsoleteMessage,
                        ObsoleteIsError = member.ObsoleteIsError,
                    });
                }
            }
            int classHeight = GetClassHeight(entity, entities);
            return new Phase1Entity()
            {
                TFN = entity.TFN,
                EntityId = entity.EntityId,
                ClassHeight = classHeight,
                Members = new EquatableArray<OutputMember>(outputMembers.OrderBy(m => m.Sequence)),
                BaseTFN = entity.BaseTFN,
            };
        }

        public static OutputEntity ResolveEntities(Phase1Entity entity, ImmutableArray<Phase1Entity> allEnts)
        {
            var baseEntity = allEnts.FirstOrDefault(e => e.TFN == entity.BaseTFN);
            List<Phase1Entity> derivedEntities = GetDerivedEntities(entity.TFN, allEnts);
            return new OutputEntity()
            {
                TFN = entity.TFN,
                EntityId = entity.EntityId,
                ClassHeight = entity.ClassHeight,
                Members = entity.Members,
                BaseEntity = baseEntity,
                DerivedEntities = new EquatableArray<Phase1Entity>(derivedEntities.OrderBy(e => e.Intf.FullName))
            };
        }

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            // do derived stuff
            OnBeginInitialize(context);

            // filter for entities
            IncrementalValuesProvider<ParsedEntity> parsedEntities1 = context.SyntaxProvider
                .ForAttributeWithMetadataName(
                    "DTOMaker.Models.EntityAttribute",
                    predicate: static (syntaxNode, _) => syntaxNode is InterfaceDeclarationSyntax,
                    transform: static (ctx, _) => GetParsedEntity(ctx))
                .Where(static e => e is not null)!;

            // filter for Members
            IncrementalValuesProvider<ParsedMember> parsedMembers = context.SyntaxProvider
                .ForAttributeWithMetadataName(
                    "DTOMaker.Models.MemberAttribute",
                    predicate: static (syntaxNode, _) => syntaxNode is PropertyDeclarationSyntax,
                    transform: static (ctx, _) => GetParsedMember(ctx))
                .Where(static m => m is not null)!;

            // add base entity
            IncrementalValuesProvider<ParsedEntity> parsedEntities2 = parsedEntities1.Collect().Select((list1, _) => AddEntityBase(list1)).SelectMany((list2, _) => list2.ToImmutableArray());

            var parsedMatrix = parsedEntities2.Collect().Combine(parsedMembers.Collect());

            // resolve members and class height
            IncrementalValuesProvider<Phase1Entity> phase1Entities = parsedEntities2.Combine(parsedMatrix)
                .Select((pair, _) =>
                {
                    var entity = pair.Left;
                    var members = pair.Right.Right;
                    var allents = pair.Right.Left;
                    return ResolveMembers(entity, members, allents);
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
            //        };
            //    });

            // resolve base entity and derived entities
            IncrementalValuesProvider<OutputEntity> outputEntities = phase1Entities.Combine(phase1Entities.Collect())
                .Select((pair, _) =>
                {
                    var entity = pair.Left;
                    var allEnts = pair.Right;
                    return ResolveEntities(entity, allEnts);
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

            // todo emit metadata in json format
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

            context.RegisterSourceOutput(context.CompilationProvider, (spc, compilation) =>
            {
                // This is a way to check that the source generator is running
                // You can remove this diagnostic if you don't need it
                spc.ReportDiagnostic(Diagnostic.Create(DiagnosticsEN.OK01, Location.None));
            });

            // do derived stuff
            OnEndInitialize(context, outputEntities);
        }
    }

}
