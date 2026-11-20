using DataFac.Storage;
using DTOMaker.Models;
using DTOMaker.Runtime;
using MemoryPack;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Testing.Models.HyperSer;

namespace Testing.Models.MemPack;

public abstract class EntityBase : IEntityBase, IPackable, IEquatable<EntityBase>
{
    [MethodImpl(MethodImplOptions.NoInlining)] private static void ThrowIsFrozenException(string? methodName) => throw new InvalidOperationException($"Cannot set {methodName} when frozen.");
    [MethodImpl(MethodImplOptions.NoInlining)] private static void ThrowIsNotFrozenException(string? methodName) => throw new InvalidOperationException($"Cannot call {methodName} when not frozen.");
    [MethodImpl(MethodImplOptions.NoInlining)] private static void ThrowIsNotPackedException(string? methodName) => throw new InvalidOperationException($"Cannot call {methodName} when not packed.");

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected void ThrowIfFrozen([CallerMemberName] string? methodName = null)
    {
        if (_frozen) EntityBase.ThrowIsFrozenException(methodName);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected void ThrowIfNotFrozen([CallerMemberName] string? methodName = null)
    {
        if (!_frozen) EntityBase.ThrowIsNotFrozenException(methodName);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected void ThrowIfNotPacked([CallerMemberName] string? methodName = null)
    {
        if (!_packed) ThrowIsNotPackedException(methodName);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected T IfNotFrozen<T>(in T value, [CallerMemberName] string? methodName = null)
    {
        if (_frozen) EntityBase.ThrowIsFrozenException(methodName);
        return value;
    }

    public EntityBase() { }
    public EntityBase(EntityBase source) { }

    private volatile bool _frozen;
    public bool IsFrozen() => _frozen;
    protected virtual void OnFreeze() { }
    public void Freeze()
    {
        if (_frozen) return;
        if (!_packed) EntityBase.ThrowIsNotPackedException(nameof(Freeze));
        OnFreeze();
        _frozen = true;
    }
    protected abstract IEntityBase OnShallowCopy();
    public IEntityBase ShallowCopy() => OnShallowCopy();

    private volatile bool _packed;
    /// <inheritdoc/>
    public bool IsPacked() => _packed;
    /// <inheritdoc/>
    protected virtual ValueTask OnPack(IBlobStore blobStore, CancellationToken cancellation) => default;
    /// <inheritdoc/>
    public async ValueTask Pack(IBlobStore blobStore, CancellationToken cancellation)
    {
        if (_frozen) return;
        if (_packed) return;
        await OnPack(blobStore, cancellation);
        _packed = true;
        OnFreeze();
        _frozen = true;
        _unpacked = true;
    }

    private volatile bool _unpacked;
    /// <inheritdoc/>
    public bool IsUnpacked() => _unpacked;
    /// <inheritdoc/>
    protected virtual ValueTask OnUnpack(IBlobStore blobStore, int depth, CancellationToken cancellation) => default;
    /// <inheritdoc/>
    public async ValueTask Unpack(IBlobStore blobStore, int depth, CancellationToken cancellation)
    {
        ThrowIfNotFrozen();
        if (depth < 0) return;
        if (_unpacked) return;
        await OnUnpack(blobStore, depth, cancellation);
        _unpacked = true;
    }
    /// <inheritdoc/>
    public ValueTask UnpackAll(IBlobStore blobStore, CancellationToken cancellation) => Unpack(blobStore, int.MaxValue, cancellation);

    /// <summary>
    /// Determines whether the this entity is equal to another entity of the same type.
    /// </summary>
    public bool Equals(EntityBase? other) => true;
    /// <summary>
    /// Determines whether the this entity is equal to another entity of the same type.
    /// </summary>
    public override bool Equals(object? obj) => obj is EntityBase;
    /// <summary>
    /// The hash function for the current entity.
    /// </summary>
    public override int GetHashCode() => HashCode.Combine<Type>(typeof(EntityBase));

    protected abstract ReadOnlyMemory<byte> OnSerialize();
    public ReadOnlyMemory<byte> SerializeSync() => OnSerialize();
    public void DeserializeSync(ReadOnlySpan<byte> buffer) => throw new NotImplementedException();
}

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

