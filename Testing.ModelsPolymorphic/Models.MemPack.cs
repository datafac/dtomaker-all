using DTOMaker.Models;
using DTOMaker.Runtime;
using MemoryPack;
using System;
using System.IO;
using Testing.Models.MemPack;

namespace Testing.ModelsPolymorphic.MemPack;

[MemoryPackable]
[MemoryPackUnion(4, typeof(FloatNumber))]
[MemoryPackUnion(5, typeof(WholeNumber))]
public abstract partial class NumberBase : EntityBase, INumberBase, IEquatable<NumberBase>, IPackable<NumberBase>
{
    [MemoryPackConstructor]
    public NumberBase() { }
    public NumberBase(NumberBase source) : base(source)
    {
    }

    public bool Equals(NumberBase? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;
        if (!base.Equals(other)) return false;
        return true;
    }

    public override bool Equals(object? obj) => obj is NumberBase other && Equals(other);
    public override int GetHashCode()
    {
        HashCode hasher = new();
        hasher.Add(base.GetHashCode());
        hasher.Add(typeof(NumberBase));
        return hasher.ToHashCode();
    }

    protected override ReadOnlyMemory<byte> OnSerialize()
    {
        return MemoryPackSerializer.Serialize<NumberBase>(this);
    }
    public static NumberBase DeserializeFrom(ReadOnlySpan<byte> buffer)
    {
        return MemoryPackSerializer.Deserialize<NumberBase>(buffer) ?? throw new InvalidDataException();
    }
}

[MemoryPackable]
public sealed partial class FloatNumber : NumberBase, IFloatNumber, IEquatable<FloatNumber>, IPackable<FloatNumber>
{
    [MemoryPackInclude] public Double Value { get; set; }

    [MemoryPackConstructor]
    public FloatNumber() { }
    public FloatNumber(FloatNumber source) : base(source)
    {
        Value = source.Value;
    }

    protected override IEntityBase OnShallowCopy()
    {
        return new FloatNumber(this);
    }

    public bool Equals(FloatNumber? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;
        if (!base.Equals(other)) return false;
        if (Value != other.Value) return false;
        return true;
    }

    public override bool Equals(object? obj) => obj is FloatNumber other && Equals(other);
    public override int GetHashCode()
    {
        HashCode hasher = new();
        hasher.Add(base.GetHashCode());
        hasher.Add(typeof(FloatNumber));
        hasher.Add(Value);
        return hasher.ToHashCode();
    }

    protected override ReadOnlyMemory<byte> OnSerialize()
    {
        return MemoryPackSerializer.Serialize<FloatNumber>(this);
    }
    public static FloatNumber DeserializeFrom(ReadOnlySpan<byte> buffer)
    {
        return MemoryPackSerializer.Deserialize<FloatNumber>(buffer) ?? throw new InvalidDataException();
    }
}

[MemoryPackable]
public sealed partial class WholeNumber : NumberBase, IWholeNumber, IEquatable<WholeNumber>, IPackable<WholeNumber>
{
    [MemoryPackInclude] public Int64 Value { get; set; }

    [MemoryPackConstructor]
    public WholeNumber() { }
    public WholeNumber(WholeNumber source) : base(source)
    {
        Value = source.Value;
    }

    protected override IEntityBase OnShallowCopy()
    {
        return new WholeNumber(this);
    }

    public bool Equals(WholeNumber? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;
        if (!base.Equals(other)) return false;
        if (Value != other.Value) return false;
        return true;
    }

    public override bool Equals(object? obj) => obj is WholeNumber other && Equals(other);
    public override int GetHashCode()
    {
        HashCode hasher = new();
        hasher.Add(base.GetHashCode());
        hasher.Add(typeof(WholeNumber));
        hasher.Add(Value);
        return hasher.ToHashCode();
    }

    protected override ReadOnlyMemory<byte> OnSerialize()
    {
        return MemoryPackSerializer.Serialize<WholeNumber>(this);
    }
    public static WholeNumber DeserializeFrom(ReadOnlySpan<byte> buffer)
    {
        return MemoryPackSerializer.Deserialize<WholeNumber>(buffer) ?? throw new InvalidDataException();
    }
}
