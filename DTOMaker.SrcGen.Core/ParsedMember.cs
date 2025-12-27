using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;

namespace DTOMaker.SrcGen.Core
{
    public record class ParsedMember
    {
        public string FullName { get; init; }
        public string PropName { get; init; }
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
        public EquatableArray<Diagnostic> Diagnostics { get; init; } = EquatableArray<Diagnostic>.Empty;

        public ParsedMember(string fullname, int sequence, TypeFullName memberType, MemberKind kind, bool isNullable, IEnumerable<Diagnostic> diagnostics)
        {
            FullName = fullname;
            Sequence = sequence;
            MemberType = memberType;
            Kind = kind;
            IsNullable = isNullable;
            Diagnostics = new EquatableArray<Diagnostic>(diagnostics);

            // derived properties
            PropName = fullname.Split('.').Last();
        }
    }
}
