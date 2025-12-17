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
        public string ObsoleteMessage { get; init; }
        public bool ObsoleteIsError { get; init; }

        public ParsedMember(string fullname, int sequence, TypeFullName memberType, MemberKind kind, bool isNullable, bool isObsolete, string obsoleteMessage, bool obsoleteIsError)
        {
            FullName = fullname;
            Sequence = sequence;
            MemberType = memberType;
            Kind = kind;
            IsNullable = isNullable;
            IsObsolete = isObsolete;
            ObsoleteMessage = obsoleteMessage;
            ObsoleteIsError = obsoleteIsError;

            // derived properties
            PropName = fullname.Split('.').Last();
        }
    }
}
