using Microsoft.CodeAnalysis;

namespace DTOMaker.SrcGen.Core
{
    public static class DiagnosticsEN
    {
        private static DiagnosticDescriptor CreateInfoDiagnostic(string cat, string id, string title, string desc)
        {
            return new DiagnosticDescriptor(
                id: id,
                title: title,
                messageFormat: desc,
                category: cat.ToString(),
                defaultSeverity: DiagnosticSeverity.Info,
                isEnabledByDefault: true);
        }

        private static readonly DiagnosticDescriptor _ok01 = CreateInfoDiagnostic(DiagnosticCategory.Other, "OK01", "Source generated", "Source generation complete.");
        public static DiagnosticDescriptor OK01 => _ok01;
    }

}