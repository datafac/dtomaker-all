using System;

namespace DTOMaker.SrcGen.Core
{
    public record class ParsedName
    {
        public string FullName { get; } = string.Empty;
        public string Space { get; } = string.Empty;
        public string Name { get; } = string.Empty;

        public ParsedName() { }
        public ParsedName(string fullname)
        {
            FullName = fullname;
            int lastDotPos = fullname.AsSpan().LastIndexOf('.');
            Space = lastDotPos < 0 ? string.Empty : fullname.AsSpan().Slice(0, lastDotPos).ToString();
            Name = lastDotPos < 0 ? fullname : fullname.AsSpan().Slice(lastDotPos + 1).ToString();
        }

        public ParsedName(string space, string name)
        {
            Space = space;
            Name = name;
            FullName = space + "." + name;
        }

        public override string ToString() => FullName;
    }
}
