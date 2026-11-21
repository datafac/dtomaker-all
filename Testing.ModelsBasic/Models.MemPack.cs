using DTOMaker.Models;
using DTOMaker.Runtime;
using MemoryPack;
using System;
using System.IO;
using Testing.Models.MemPack;

namespace Testing.ModelsBasic.MemPack;

[MemoryPackable]
public partial class Nothing : EntityBase, INothing, IEquatable<Nothing>, IPackable<Nothing>
{
    protected override IEntityBase OnShallowCopy() => new Nothing() { };

    public bool Equals(Nothing? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;
        if (!base.Equals(other)) return false;
        return true;
    }

    public override bool Equals(object? obj) => obj is Nothing other && Equals(other);
    public override int GetHashCode()
    {
        HashCode hasher = new();
        hasher.Add(typeof(Nothing));
        return hasher.ToHashCode();
    }

    protected override ReadOnlyMemory<byte> OnSerialize()
    {
        return MemoryPackSerializer.Serialize<Nothing>(this);
    }
    public static Nothing DeserializeFrom(ReadOnlySpan<byte> buffer)
    {
        return MemoryPackSerializer.Deserialize<Nothing>(buffer) ?? throw new InvalidDataException();
    }
}

[MemoryPackable]
public partial class Simple : EntityBase, ISimple, IEquatable<Simple>, IPackable<Simple>
{
    protected override IEntityBase OnShallowCopy() => new Simple() { Field1 = this.Field1, Field2 = this.Field2, Field3 = this.Field3 };
    [MemoryPackInclude] public Int64 Field1 { get; set; }
    [MemoryPackInclude] public Int64? Field2 { get; set; }
    [MemoryPackInclude] public String? Field3 { get; set; }

    public bool Equals(Simple? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;
        if (!base.Equals(other)) return false;
        if (Field1 != other.Field1) return false;
        if (Field2 != other.Field2) return false;
        if (Field3 != other.Field3) return false;
        return true;
    }

    public override bool Equals(object? obj) => obj is Simple other && Equals(other);
    public override int GetHashCode()
    {
        HashCode hasher = new();
        hasher.Add(typeof(Simple));
        hasher.Add(Field1);
        hasher.Add(Field2);
        hasher.Add(Field3);
        return hasher.ToHashCode();
    }

    protected override ReadOnlyMemory<byte> OnSerialize()
    {
        return MemoryPackSerializer.Serialize<Simple>(this);
    }
    public static Simple DeserializeFrom(ReadOnlySpan<byte> buffer)
    {
        return MemoryPackSerializer.Deserialize<Simple>(buffer) ?? throw new InvalidDataException();
    }
}

