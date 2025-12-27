using Microsoft.CodeAnalysis;

namespace DTOMaker.SrcGen.Core
{
    public static class DiagnosticsEN
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
        public static readonly DiagnosticDescriptor DME05 = Create(DiagnosticSeverity.Error, DiagnosticCategory.Design, nameof(DME05), "Invalid entity length",  "The entity length must be a whole power of 2 between 0 and 8192.");
        public static readonly DiagnosticDescriptor DME06 = Create(DiagnosticSeverity.Error, DiagnosticCategory.Design, nameof(DME06), "Invalid member length",  "The member length must be a whole power of 2 between 1 and 1024.");
        public static readonly DiagnosticDescriptor DME07 = Create(DiagnosticSeverity.Error, DiagnosticCategory.Design, nameof(DME07), "Invalid member offset",  "The member offset must be zero or greater.");
        public static readonly DiagnosticDescriptor DME08 = Create(DiagnosticSeverity.Error, DiagnosticCategory.Design, nameof(DME08), "Invalid nullability",    "Nullable<T> fields are not supported in MemBlocks."); // todo allow Nullable<T>
        public static readonly DiagnosticDescriptor DME09 = Create(DiagnosticSeverity.Error, DiagnosticCategory.Design, nameof(DME09), "Invalid layout method",  "Entity layout method must be defined.");
        public static readonly DiagnosticDescriptor DME10 = Create(DiagnosticSeverity.Error, DiagnosticCategory.Design, nameof(DME10), "Invalid member type",    "The member type is not supported. Are you missing a converter?");
        public static readonly DiagnosticDescriptor DME11 = Create(DiagnosticSeverity.Error, DiagnosticCategory.Design, nameof(DME11), "Member sequence issue",  "Member sequence numbers must start at 1 and increase monotonically.");
        public static readonly DiagnosticDescriptor DME12 = Create(DiagnosticSeverity.Error, DiagnosticCategory.Design, nameof(DME12), "Duplicate entity id",    "Duplicate entity id. Entity ids must be unique.");
        public static readonly DiagnosticDescriptor DME13 = Create(DiagnosticSeverity.Error, DiagnosticCategory.Design, nameof(DME13), "Member layout issue",    "Member overlaps another, is misaligned, or extends beyond the end of the block.");
    }

}