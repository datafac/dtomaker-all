using DataFac.Storage;
using DTOMaker.Models;
using DTOMaker.Runtime;
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

    protected static bool EqualTo<T>(T a, T b) where T : IEquatable<T>
    {
        if (ReferenceEquals(a, b)) return true;
        if (a is null) return false;
        if (b is null) return false;
        return a.Equals(b);
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

    protected abstract ReadOnlyMemory<byte> OnSerialize();
    public ReadOnlyMemory<byte> SerializeSync() => OnSerialize();
    public void DeserializeSync(ReadOnlySpan<byte> buffer) => throw new NotImplementedException();
}

