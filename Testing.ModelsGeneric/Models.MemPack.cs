using DTOMaker.Models;
using DTOMaker.Runtime;
using MemoryPack;
using System;
using System.IO;
using Testing.Models.MemPack;

namespace Testing.ModelsGeneric.MemPack;

[MemoryPackUnion(MyCoupleA.EntityId, typeof(MyCoupleA))]
public abstract partial class Couple<T1, T2> : EntityBase, ICouple<T1, T2>, IEquatable<Couple<T1, T2>>, IPackable<Couple<T1, T2>>
    where T1 : IEquatable<T1>
    where T2 : IEquatable<T2>
{
    public T1 Item1 { get; set; } = default!;
    public T2 Item2 { get; set; } = default!;

    [MemoryPackConstructor]
    public Couple() { }
    public Couple(Couple<T1, T2> source) : base(source)
    {
        Item1 = source.Item1;
        Item2 = source.Item2;
    }

    public bool Equals(Couple<T1, T2>? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;
        if (!base.Equals(other)) return false;
        if (!EqualTo(other.Item1, this.Item1)) return false;
        if (!EqualTo(other.Item2, this.Item2)) return false;
        return true;
    }

    public override bool Equals(object? obj) => obj is Couple<T1, T2> other && Equals(other);
    public override int GetHashCode()
    {
        HashCode hasher = new();
        hasher.Add(base.GetHashCode());
        hasher.Add(typeof(Couple<T1, T2>));
        hasher.Add(Item1);
        hasher.Add(Item2);
        return hasher.ToHashCode();
    }

    protected override ReadOnlyMemory<byte> OnSerialize()
    {
        return MemoryPackSerializer.Serialize<Couple<T1, T2>>(this);
    }
    public static Couple<T1, T2> DeserializeFrom(ReadOnlySpan<byte> buffer)
    {
        return MemoryPackSerializer.Deserialize<Couple<T1, T2>>(buffer) ?? throw new InvalidDataException();
    }
}

[MemoryPackable]
public sealed partial class MyCoupleA : Couple<int, string>, IMyCoupleA, IEquatable<MyCoupleA>, IPackable<MyCoupleA>
{
    public const int EntityId = 11;

    [MemoryPackConstructor]
    public MyCoupleA() { }
    public MyCoupleA(MyCoupleA source) : base(source) { }
    protected override IEntityBase OnShallowCopy() => new MyCoupleA(this);

    public bool Equals(MyCoupleA? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;
        if (!base.Equals(other)) return false;
        return true;
    }

    public override bool Equals(object? obj) => obj is MyCoupleA other && Equals(other);
    public override int GetHashCode()
    {
        HashCode hasher = new();
        hasher.Add(base.GetHashCode());
        hasher.Add(typeof(MyCoupleA));
        return hasher.ToHashCode();
    }

    protected override ReadOnlyMemory<byte> OnSerialize()
    {
        return MemoryPackSerializer.Serialize<MyCoupleA>(this);
    }
    public static MyCoupleA DeserializeFrom(ReadOnlySpan<byte> buffer)
    {
        return MemoryPackSerializer.Deserialize<MyCoupleA>(buffer) ?? throw new InvalidDataException();
    }
}

