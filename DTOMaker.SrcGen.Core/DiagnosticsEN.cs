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

        public static readonly DiagnosticDescriptor WRN01 = CreateDiagnostic(DiagnosticSeverity.Warning, DiagnosticCategory.Other, nameof(WRN01), "Ignored unknown attribute", "Unknown attribute was ignored.");

        public static readonly DiagnosticDescriptor ERR01 = CreateDiagnostic(DiagnosticSeverity.Error, DiagnosticCategory.Syntax, nameof(ERR01), "Missing or invalid Id", "The interface must have a valid Id attribute with a positive integer value.");
        public static readonly DiagnosticDescriptor ERR02 = CreateDiagnostic(DiagnosticSeverity.Error, DiagnosticCategory.Syntax, nameof(ERR02), "Invalid argument count", "Attribute has incorrect number of arguments.");
        public static readonly DiagnosticDescriptor ERR03 = CreateDiagnostic(DiagnosticSeverity.Error, DiagnosticCategory.Syntax, nameof(ERR03), "Invalid argument value", "The argument value is invalid or out of range.");
        public static readonly DiagnosticDescriptor ERR04 = CreateDiagnostic(DiagnosticSeverity.Error, DiagnosticCategory.Design, nameof(ERR04), "Invalid member sequence", "The sequence number must be positive and unique within the entity.");

        /*
         */
    }

}