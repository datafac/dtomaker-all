using Microsoft.CodeAnalysis;
using System;
using System.Collections;
using System.Collections.Generic;

namespace DTOMaker.SrcGen.Core
{
    public record class ParsedEntity
    {
        public TypeFullName TFN { get; init; } = new();
        public int EntityId { get; init; }
        public TypeFullName? BaseTFN { get; init; } = null;
        public EquatableArray<Diagnostic> Diagnostics { get; init; } = EquatableArray<Diagnostic>.Empty;
        public int KeyOffset { get; init; }
        public int BlockLength { get; init; }
        public LayoutMethod LayoutMethod { get; init; }

        public ParsedEntity(TypeFullName tfn, int entityId, TypeFullName? baseTFN, IEnumerable<Diagnostic> diagnostics)
        {
            TFN = tfn;
            EntityId = entityId;
            BaseTFN = baseTFN;
            Diagnostics = new EquatableArray<Diagnostic>(diagnostics);
        }

        public override string ToString() => $"{TFN} [{EntityId}]";
    }
}
