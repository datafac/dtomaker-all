using Microsoft.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace DTOMaker.SrcGen.Core
{
    public sealed record OutputMember
    {
        public string Name { get; init; } = string.Empty;
        public int Sequence { get; init; }
        public TypeFullName MemberType { get; init; }
        public MemberKind Kind { get; init; }
        public bool IsNullable { get; init; }
        public bool IsObsolete { get; init; }
        public string ObsoleteMessage { get; init; } = string.Empty;
        public bool ObsoleteIsError { get; init; }
        public int FieldOffset { get; init; }
        public int FieldLength { get; init; }
        public bool IsBigEndian { get; init; }
        public bool IsExternal { get; init; }
        public bool IsEmbedded => !IsExternal;
        public EquatableArray<Diagnostic> Diagnostics { get; init; } = EquatableArray<Diagnostic>.Empty;
    }
}
