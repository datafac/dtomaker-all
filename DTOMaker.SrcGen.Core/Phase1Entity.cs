using System.Collections.Generic;

namespace DTOMaker.SrcGen.Core
{
    public record class Phase1Entity : IResolvedEntity
    {
        public TypeFullName TFN { get; init; } = new();
        public int EntityId { get; init; }
        public int KeyOffset { get; init; }
        public int ClassHeight { get; init; }
        public EquatableArray<OutputMember> Members { get; init; } = EquatableArray<OutputMember>.Empty;
        public TypeFullName? BaseTFN { get; init; }

        public IResolvedEntity? BaseEntity => throw new System.NotImplementedException();
        public IReadOnlyCollection<IResolvedEntity> DerivedEntities => throw new System.NotImplementedException();

        public override string ToString() => $"{TFN} [{EntityId}] ({Members.Count} members)";
    }
}
