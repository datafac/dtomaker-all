using Microsoft.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace DTOMaker.SrcGen.Core
{
    public sealed record OutputMember
    {
        public Location Location { get; init; } = Location.None;
        public string Name { get; init; } = string.Empty;
        public int Sequence { get; init; }
        public TypeFullName MemberType { get; init; }
        public MemberKind Kind { get; init; }
        public bool IsCustom { get; init; }
        public bool IsNullable { get; init; }
        public ObsoleteInformation? ObsoleteInfo { get; init; }
        public bool IsObsolete => ObsoleteInfo is not null;
        public int FieldOffset { get; init; }
        public int FieldLength { get; init; }
        public bool IsBigEndian { get; init; }
        public bool IsExternal { get; init; }
        public bool IsEmbedded => !IsExternal;
        public string? ConverterSpace { get; init; }
        public string? ConverterNameqqq { get; init; }
        public EquatableArray<Diagnostic> Diagnostics { get; init; } = EquatableArray<Diagnostic>.Empty;
    }
}
