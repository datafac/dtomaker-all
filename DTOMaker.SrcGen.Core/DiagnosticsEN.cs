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
        public static readonly DiagnosticDescriptor DME03 = CreateDiagnostic(DiagnosticSeverity.Error, DiagnosticCategory.Design, nameof(DME03), "Invalid entity Id", "The interface must have a valid Id attribute with a positive integer value.");
        public static readonly DiagnosticDescriptor DME04 = CreateDiagnostic(DiagnosticSeverity.Error, DiagnosticCategory.Design, nameof(DME04), "Invalid member sequence", "The sequence number must be positive and unique within the entity.");

        /*
         */
    }

}