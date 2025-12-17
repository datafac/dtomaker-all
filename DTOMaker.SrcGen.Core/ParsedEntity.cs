using Microsoft.CodeAnalysis;
using System;

namespace DTOMaker.SrcGen.Core
{
    public record class ParsedEntity
    {
        public TypeFullName TFN { get; init; } = new();
        public ParsedName Intf => TFN.Intf;
        public ParsedName Impl => TFN.Impl;
        public int EntityId { get; init; } = 0;
        public TypeFullName? BaseTFN { get; init; } = null;

        public ParsedEntity(TypeFullName tfn, int entityId, TypeFullName? baseTFN)
        {
            TFN = tfn;
            EntityId = entityId;
            BaseTFN = baseTFN;
        }

        public override string ToString() => $"{TFN} [{EntityId}]";
    }
}
