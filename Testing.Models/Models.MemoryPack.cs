using DTOMaker.Models;
using System;
using MemoryPack;

namespace Testing.Models.MemPack;

[MemoryPackable]
public partial class Simple : ISimple, IEquatable<Simple>
{
    public bool IsFrozen() => false;
    public void Freeze() { }
    public IEntityBase ShallowCopy() => new Simple() { Field1 = this.Field1, Field2 = this.Field2 };
    [MemoryPackInclude] public Int64 Field1 { get; set; }
    [MemoryPackInclude] public Int64? Field2 { get; set; }

    public bool Equals(Simple? other) => other is not null;
    public override bool Equals(object? obj) => obj is Simple other && Equals(other);
    public override int GetHashCode()
    {
        HashCode hasher = new();
        hasher.Add(typeof(Simple));
        hasher.Add(Field1);
        hasher.Add(Field2);
        return hasher.ToHashCode();
    }
}

[MemoryPackable]
public partial class Required_String : IRequired_String, IEquatable<Required_String>
{
    public bool IsFrozen() => false;
    public void Freeze() { }
    public IEntityBase ShallowCopy() => new Required_String() { Field = this.Field };

    [MemoryPackInclude] public String Field { get; set; } = string.Empty;

    public bool Equals(Required_String? other) => other is not null;
    public override bool Equals(object? obj) => obj is Required_String other && Equals(other);
    public override int GetHashCode()
    {
        HashCode hasher = new();
        hasher.Add(typeof(Required_String));
        hasher.Add(Field);
        return hasher.ToHashCode();
    }
}
