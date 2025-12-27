using Microsoft.CodeAnalysis;

namespace DTOMaker.SrcGen.Core
{
    public static class DiagnosticsEN
    {
        private static DiagnosticDescriptor CreateDiagnostic(DiagnosticSeverity sev, string cat, string id, string title, string desc)
        {
            return new DiagnosticDescriptor(
                id: id,
                title: title,
                messageFormat: desc,
                category: cat.ToString(),
                defaultSeverity: sev,
                isEnabledByDefault: true);
        }

        public static readonly DiagnosticDescriptor INF01 = CreateDiagnostic(DiagnosticSeverity.Info, DiagnosticCategory.Other, nameof(INF01), "Source generated", "Source generation complete.");

        //public static readonly DiagnosticDescriptor IgnoredAttribute = CreateDiagnostic(DiagnosticSeverity.Warning, DiagnosticCategory.Other, nameof(IgnoredAttribute), "Ignored unknown attribute", "Unknown attribute was ignored.");

        public static readonly DiagnosticDescriptor DME01 = CreateDiagnostic(DiagnosticSeverity.Error, DiagnosticCategory.Syntax, nameof(DME01), "Invalid argument count", "Attribute has incorrect number of arguments.");
        public static readonly DiagnosticDescriptor DME02 = CreateDiagnostic(DiagnosticSeverity.Error, DiagnosticCategory.Syntax, nameof(DME02), "Invalid argument value", "The argument value is invalid or out of range.");
        public static readonly DiagnosticDescriptor DME03 = CreateDiagnostic(DiagnosticSeverity.Error, DiagnosticCategory.Design, nameof(DME03), "Invalid entity id",      "The interface must have a valid id attribute with a positive integer value.");
        public static readonly DiagnosticDescriptor DME04 = CreateDiagnostic(DiagnosticSeverity.Error, DiagnosticCategory.Design, nameof(DME04), "Invalid member sequence","The sequence number must be positive and unique within the entity.");
        public static readonly DiagnosticDescriptor DME05 = CreateDiagnostic(DiagnosticSeverity.Error, DiagnosticCategory.Design, nameof(DME05), "Invalid entity length",  "The entity length must be a whole power of 2 between 1 and 8192.");
        public static readonly DiagnosticDescriptor DME06 = CreateDiagnostic(DiagnosticSeverity.Error, DiagnosticCategory.Design, nameof(DME06), "Invalid member length",  "The member length must be a whole power of 2 between 1 and 1024.");
        public static readonly DiagnosticDescriptor DME07 = CreateDiagnostic(DiagnosticSeverity.Error, DiagnosticCategory.Design, nameof(DME07), "Invalid member offset",  "The member offset must be zero or greater.");
        public static readonly DiagnosticDescriptor DME08 = CreateDiagnostic(DiagnosticSeverity.Error, DiagnosticCategory.Design, nameof(DME08), "Invalid nullability",    "Nullable<T> fields are not supported in MemBlocks."); // todo allow Nullable<T>
        public static readonly DiagnosticDescriptor DME09 = CreateDiagnostic(DiagnosticSeverity.Error, DiagnosticCategory.Design, nameof(DME09), "Invalid layout method",  "Entity layout method must be defined.");
        public static readonly DiagnosticDescriptor DME10 = CreateDiagnostic(DiagnosticSeverity.Error, DiagnosticCategory.Design, nameof(DME10), "Invalid member type",    "The member type is not supported. Are you missing a converter?");
        public static readonly DiagnosticDescriptor DME11 = CreateDiagnostic(DiagnosticSeverity.Error, DiagnosticCategory.Design, nameof(DME11), "Invalid member type",    "Member sequence numbers must start at 1 and increase monotonically.");
        public static readonly DiagnosticDescriptor DME12 = CreateDiagnostic(DiagnosticSeverity.Error, DiagnosticCategory.Design, nameof(DME12), "Duplicate entity id",    "All entity ids must be unique.");

        /*
         */
    }

}