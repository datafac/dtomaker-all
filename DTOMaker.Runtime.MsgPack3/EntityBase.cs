using DataFac.Memory;
using DataFac.Storage;
using DTOMaker.Models;
using MessagePack;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace DTOMaker.Runtime.MsgPack3;

/// <summary>
/// Provides a base class for MsgPack3 generated entities, supporting identity, equality comparison, 
/// and a frozen state to prevent further modification.
/// </summary>
public abstract class EntityBase : IPackable, IEquatable<EntityBase>
{
    /// <summary>
    /// Represents the default entity identifier value.
    /// </summary>
    public const int EntityId = 0;
    /// <summary>
    /// When implemented in a derived class, retrieves the unique identifier for the associated entity.
    /// </summary>
    protected abstract int OnGetEntityId();

    /// <inheritdoc/>
    public int GetEntityId() => OnGetEntityId();

    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    public EntityBase() { }
    /// <summary>
    /// Initializes a new instance of the class by copying values from any instance implementing
    /// the entity interface.
    /// </summary>
    public EntityBase(IEntityBase _) { }
    /// <summary>
    /// Initializes a new instance of the class by copying values from an existing instance.
    /// </summary>
    public EntityBase(EntityBase _) { }

    [IgnoreMember]
    private volatile bool _frozen;
    /// <summary>
    /// Gets a value indicating whether the entity is in a frozen (immutable) state.
    /// </summary>
    [IgnoreMember]
    public bool IsFrozen => _frozen;
    protected virtual void OnFreeze() { }

    /// <inheritdoc/>
    public void Freeze()
    {
        if (_frozen) return;
        _frozen = true;
        OnFreeze();
    }
    protected abstract IEntityBase OnShallowCopy();

    /// <inheritdoc/>
    public IEntityBase ShallowCopy() => OnShallowCopy();

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void ThrowIsFrozenException(string? methodName) => throw new InvalidOperationException($"Cannot set {methodName} when frozen.");

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void ThrowIsFrozen(string? memberName) => throw new InvalidOperationException($"Cannot call {memberName} when frozen.");

    /// <summary>
    /// Ensures that the entity is not frozen and throws an exception if it is, enforcing mutability for certain operations.
    /// </summary>
    /// <param name="memberName"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected void CheckNotFrozen([CallerMemberName] string? memberName = null)
    {
        if (_frozen) ThrowIsFrozen(memberName);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void ThrowIsNotFrozenException(string? methodName) => throw new InvalidOperationException($"Cannot call {methodName} when not frozen.");

    /// <summary>
    /// Ensures that the entity is frozen and throws an exception if it is not, enforcing immutability for certain operations.
    /// </summary>
    /// <param name="methodName"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected void ThrowIfNotFrozen([CallerMemberName] string? methodName = null)
    {
        if (!_frozen) ThrowIsNotFrozenException(methodName);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void ThrowIsNotPackedException(string? methodName) => throw new InvalidOperationException($"Cannot call {methodName} when not packed.");

    /// <summary>
    /// Ensures that the entity is packed and throws an exception if it is not.
    /// </summary>
    /// <param name="methodName"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected void ThrowIfNotPacked([CallerMemberName] string? methodName = null)
    {
        if (!_packed) ThrowIsNotPackedException(methodName);
    }

    /// <inheritdoc/>
    public bool Equals(EntityBase? other) => true;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is EntityBase;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine<Type>(typeof(EntityBase));

    #region IPackable implementation
    protected virtual ReadOnlyMemory<byte> OnSerialize() => ReadOnlyMemory<byte>.Empty;
    public ReadOnlyMemory<byte> Serialize()
    {
        ThrowIfNotPacked();
        return OnSerialize();
    }

    private volatile bool _packed;
    /// <inheritdoc/>
    [IgnoreMember]
    public bool IsPacked => _packed;
    protected virtual ValueTask OnPack(IDataStore dataStore) => default;
    public async ValueTask Pack(IDataStore dataStore)
    {
        if (_frozen) return;
        if (_packed) return;
        await OnPack(dataStore);
        _packed = true;
        OnFreeze();
        _frozen = true;
        _unpacked = true;
    }

    private volatile bool _unpacked;
    /// <inheritdoc/>
    [IgnoreMember]
    public bool IsUnpacked => _unpacked;
    protected virtual ValueTask OnUnpack(IDataStore dataStore, int depth) => default;
    public async ValueTask Unpack(IDataStore dataStore, int depth = 0)
    {
        ThrowIfNotFrozen();
        if (depth < 0) return;
        if (_unpacked) return;
        await OnUnpack(dataStore, depth);
        _unpacked = true;
    }
    public ValueTask UnpackAll(IDataStore dataStore) => Unpack(dataStore, int.MaxValue);
    #endregion

}
