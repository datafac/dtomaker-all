using Microsoft.CodeAnalysis;

namespace DTOMaker.SrcGen.Core
{
    internal static class SpecialName
    {
        public const string RuntimeBaseIntfSpace = "DTOMaker.Models";
        public const string RuntimeBaseIntfName = "IEntityBase";
        public const string RuntimeBaseImplSpace = "DTOMaker.Runtime";
        public const string RuntimeBaseImplName = "EntityBase";
    }
    internal static class DiagnosticsEN
    {
        private static DiagnosticDescriptor Create(DiagnosticSeverity sev, string cat, string id, string title, string desc)
        {
            return new DiagnosticDescriptor(id, title, desc, cat, sev, true);
        }

        public static readonly DiagnosticDescriptor INF01 = Create(DiagnosticSeverity.Info, DiagnosticCategory.Other, nameof(INF01), "Source generated", "Source generation complete.");

        //public static readonly DiagnosticDescriptor IgnoredAttribute = CreateDiagnostic(DiagnosticSeverity.Warning, DiagnosticCategory.Other, nameof(IgnoredAttribute), "Ignored unknown attribute", "Unknown attribute was ignored.");

        public static readonly DiagnosticDescriptor DME01 = Create(DiagnosticSeverity.Error, DiagnosticCategory.Syntax, nameof(DME01), "Invalid argument count", "Attribute has incorrect number of arguments.");
        public static readonly DiagnosticDescriptor DME02 = Create(DiagnosticSeverity.Error, DiagnosticCategory.Syntax, nameof(DME02), "Invalid argument value", "The argument value is invalid or out of range.");
        public static readonly DiagnosticDescriptor DME03 = Create(DiagnosticSeverity.Error, DiagnosticCategory.Design, nameof(DME03), "Invalid entity id",      "The interface must have a valid id attribute with a positive integer value.");
        public static readonly DiagnosticDescriptor DME04 = Create(DiagnosticSeverity.Error, DiagnosticCategory.Design, nameof(DME04), "Invalid member sequence","The sequence number must be positive and unique within the entity.");
        public static readonly DiagnosticDescriptor DME10 = Create(DiagnosticSeverity.Error, DiagnosticCategory.Design, nameof(DME10), "Invalid member type",    "The member type is not supported. Are you missing a converter?");
        public static readonly DiagnosticDescriptor DME11 = Create(DiagnosticSeverity.Error, DiagnosticCategory.Design, nameof(DME11), "Member sequence issue",  "Member sequence numbers must start at 1 and increase monotonically.");
        public static readonly DiagnosticDescriptor DME12 = Create(DiagnosticSeverity.Error, DiagnosticCategory.Design, nameof(DME12), "Duplicate entity id",    "Duplicate entity id. Entity ids must be unique.");
        public static readonly DiagnosticDescriptor DME14 = Create(DiagnosticSeverity.Error, DiagnosticCategory.Design, nameof(DME14), "Invalid base entity",    "Invalid or missing base entity. Do all entities have [Entity] attributes?");
    }

}