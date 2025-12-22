using System.Collections.Generic;

namespace DTOMaker.SrcGen.Core;

public record class Phase2Entity : IResolvedEntity
{
    public TypeFullName TFN { get; init; } = new();
    public int EntityId { get; init; }
    public int ClassHeight { get; init; }
    public EquatableArray<OutputMember> Members { get; init; } = EquatableArray<OutputMember>.Empty;
    public Phase1Entity? BaseEntity { get; init; }
    public EquatableArray<Phase1Entity> DerivedEntities { get; init; } = EquatableArray<Phase1Entity>.Empty;

    IResolvedEntity? IResolvedEntity.BaseEntity => BaseEntity;
    IReadOnlyCollection<IResolvedEntity> IResolvedEntity.DerivedEntities => DerivedEntities;

    public override string ToString() => $"{TFN} [{EntityId}] ({Members.Count} members)";

}

public record class OutputEntity : IResolvedEntity
{
    public TypeFullName TFN { get; init; } = new();
    public int EntityId { get; init; }
    public int ClassHeight { get; init; }
    public EquatableArray<OutputMember> Members { get; init; } = EquatableArray<OutputMember>.Empty;
    public Phase2Entity? BaseEntity { get; init; }
    public EquatableArray<Phase2Entity> DerivedEntities { get; init; } = EquatableArray<Phase2Entity>.Empty;

    IResolvedEntity? IResolvedEntity.BaseEntity => BaseEntity;
    IReadOnlyCollection<IResolvedEntity> IResolvedEntity.DerivedEntities => DerivedEntities;

    public override string ToString() => $"{TFN} [{EntityId}] ({Members.Count} members)";

}

public interface IResolvedEntity
{
    public TypeFullName TFN { get; }
    public int EntityId { get; }
    public int ClassHeight { get; }
    public IResolvedEntity? BaseEntity { get; }
    public IReadOnlyCollection<IResolvedEntity> DerivedEntities { get; }
}

