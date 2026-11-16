using DataFac.Storage;
using DTOMaker.Models;
using DTOMaker.Runtime;
using MemoryPack;
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

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

    public ValueTask<ReadOnlyMemory<byte>> Serialize(CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }

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

}


[MemoryPackable]
public partial class Simple : EntityBase, ISimple, IEquatable<Simple>
{
    protected override IEntityBase OnShallowCopy() => new Simple() { Field1 = this.Field1, Field2 = this.Field2 };
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
public partial class Required_String : EntityBase, IRequired_String, IEquatable<Required_String>
{
    protected override IEntityBase OnShallowCopy() => new Required_String() { Field = this.Field };

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
