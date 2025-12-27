using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace DTOMaker.SrcGen.Core;

public record class Phase2Entity
{
    public TypeFullName TFN { get; init; } = new();
    public int EntityId { get; init; }
    public int ClassHeight { get; init; }
    public EquatableArray<OutputMember> Members { get; init; } = EquatableArray<OutputMember>.Empty;
    public Phase1Entity? BaseEntity { get; init; }
    public EquatableArray<Phase1Entity> DerivedEntities { get; init; } = EquatableArray<Phase1Entity>.Empty;
    public EquatableArray<Diagnostic> Diagnostics { get; init; } = EquatableArray<Diagnostic>.Empty;
    public int KeyOffset { get; init; }
    public int BlockLength { get; init; }
    public long BlockStructureCode { get; init; }

    public override string ToString() => $"{TFN} [{EntityId}] ({Members.Count} members)";

}

public record class OutputEntity
{
    public TypeFullName TFN { get; init; } = new();
    public int EntityId { get; init; }
    public int ClassHeight { get; init; }
    public EquatableArray<OutputMember> Members { get; init; } = EquatableArray<OutputMember>.Empty;
    public Phase2Entity? BaseEntity { get; init; }
    public EquatableArray<Phase2Entity> DerivedEntities { get; init; } = EquatableArray<Phase2Entity>.Empty;
    public EquatableArray<Diagnostic> Diagnostics { get; init; } = EquatableArray<Diagnostic>.Empty;

    /// <summary>
    /// The MessagePack key offset for this entity. This value will be added to each 
    /// member's sequence number, replacing the default calculation of (ClassHeight - 1) * 100.
    /// </summary>
    public int KeyOffset { get; init; }

    /// <summary>
    /// The MemBlocks entity block length in bytes.
    /// </summary>
    public int BlockLength { get; init; }

    /// <summary>
    /// The MemBlocks entity structure code.
    /// </summary>
    public long BlockStructureCode { get; init; }

    public override string ToString() => $"{TFN} [{EntityId}] ({Members.Count} members)";

}

//public interface IResolvedEntityNotUsed
//{
//    public TypeFullName TFN { get; }
//    public int EntityId { get; }
//    public int KeyOffset { get; }
//    public int ClassHeight { get; }
//    public IResolvedEntity? BaseEntity { get; }
//    public IReadOnlyCollection<IResolvedEntity> DerivedEntities { get; }
//    public IReadOnlyCollection<Diagnostic> Diagnostics { get; }
//}
