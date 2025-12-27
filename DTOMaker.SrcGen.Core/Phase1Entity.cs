using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace DTOMaker.SrcGen.Core
{
    public record class Phase1Entity
    {
        public Location Location { get; init; } = Location.None;
        public TypeFullName TFN { get; init; } = new();
        public int EntityId { get; init; }
        public int ClassHeight { get; init; }
        public EquatableArray<OutputMember> Members { get; init; } = EquatableArray<OutputMember>.Empty;
        public TypeFullName? BaseTFN { get; init; }
        public EquatableArray<Diagnostic> Diagnostics { get; init; } = EquatableArray<Diagnostic>.Empty;
        public int KeyOffset { get; init; }
        public int BlockLength { get; init; }
        public LayoutAlgo Layout { get; init; }

        public override string ToString() => $"{TFN} [{EntityId}] ({Members.Count} members)";
    }
}
