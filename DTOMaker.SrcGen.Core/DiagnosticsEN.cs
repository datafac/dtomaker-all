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

        public static readonly DiagnosticDescriptor IgnoredAttribute = CreateDiagnostic(DiagnosticSeverity.Warning, DiagnosticCategory.Other, nameof(IgnoredAttribute), "Ignored unknown attribute", "Unknown attribute was ignored.");

        public static readonly DiagnosticDescriptor InvalidEntityId = CreateDiagnostic(DiagnosticSeverity.Error, DiagnosticCategory.Syntax, nameof(InvalidEntityId), "Missing or invalid entity Id", "The interface must have a valid Id attribute with a positive integer value.");
        public static readonly DiagnosticDescriptor InvalidArgCount = CreateDiagnostic(DiagnosticSeverity.Error, DiagnosticCategory.Syntax, nameof(InvalidArgCount), "Invalid argument count", "Attribute has incorrect number of arguments.");
        public static readonly DiagnosticDescriptor InvalidArgValue = CreateDiagnostic(DiagnosticSeverity.Error, DiagnosticCategory.Syntax, nameof(InvalidArgValue), "Invalid argument value", "The argument value is invalid or out of range.");
        public static readonly DiagnosticDescriptor InvalidMemberId = CreateDiagnostic(DiagnosticSeverity.Error, DiagnosticCategory.Design, nameof(InvalidMemberId), "Invalid member sequence", "The sequence number must be positive and unique within the entity.");

        /*
         */
    }

}