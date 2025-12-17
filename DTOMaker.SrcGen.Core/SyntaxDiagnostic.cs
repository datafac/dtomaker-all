using Microsoft.CodeAnalysis;

namespace DTOMaker.SrcGen.Core
{
    public sealed class SyntaxDiagnostic
    {
        public readonly string Id;
        public readonly string Title;
        public readonly string Category;
        public readonly Location Location;
        public readonly DiagnosticSeverity Severity;
        public readonly string Message;
        public SyntaxDiagnostic(string id, string title, string category, Location location, DiagnosticSeverity severity, string message)
        {
            Id = id;
            Title = title;
            Category = category;
            Location = location;
            Message = message;
            Severity = severity;
        }
    }
}