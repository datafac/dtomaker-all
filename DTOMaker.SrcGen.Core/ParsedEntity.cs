using Microsoft.CodeAnalysis;
using System;

namespace DTOMaker.SrcGen.Core
{
    public record class ParsedEntity
    {
        public TypeFullName TFN { get; init; } = new();
        public int EntityId { get; init; }
        public int KeyOffset { get; init; }
        public TypeFullName? BaseTFN { get; init; } = null;

        public ParsedEntity(TypeFullName tfn, int entityId, int keyOffset, TypeFullName? baseTFN)
        {
            TFN = tfn;
            EntityId = entityId;
            KeyOffset = keyOffset;
            BaseTFN = baseTFN;
        }

        public override string ToString() => $"{TFN} [{EntityId}]";
    }
}
