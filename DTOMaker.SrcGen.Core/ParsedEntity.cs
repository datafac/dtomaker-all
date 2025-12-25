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
        public int KeyOffset { get; init; }
        public TypeFullName? BaseTFN { get; init; } = null;
        public EquatableArray<Diagnostic> Diagnostics { get; init; } = EquatableArray<Diagnostic>.Empty;

        public ParsedEntity(TypeFullName tfn, int entityId, int keyOffset, TypeFullName? baseTFN, IEnumerable<Diagnostic> diagnostics)
        {
            TFN = tfn;
            EntityId = entityId;
            KeyOffset = keyOffset;
            BaseTFN = baseTFN;
            Diagnostics = new EquatableArray<Diagnostic>(diagnostics);
        }

        public override string ToString() => $"{TFN} [{EntityId}]";
    }
}
