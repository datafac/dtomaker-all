using DataFac.Memory;
using DataFac.Storage;
using DTOMaker.Models;
using System;
using System.Collections.Immutable;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace DTOMaker.Runtime.NetStrux;

public abstract class EntityBase : INetStruxEntityBase, IEquatable<EntityBase>
{
    private readonly EntityInfo _entityInfo;

    protected virtual void OnGetBuffers(Span<ReadOnlyMemory<byte>> buffers)
    {
        buffers[0] = _entityInfo.Memory;
    }

    public ImmutableArray<ReadOnlyMemory<byte>> GetBuffers()
    {
        ThrowIfNotFrozen();
        var buffers = new ReadOnlyMemory<byte>[_entityInfo.ClassHeight + 1];
        OnGetBuffers(buffers);
        return ImmutableArray<ReadOnlyMemory<byte>>.Empty.AddRange(buffers);
    }

    protected abstract IEntityBase OnPartCopy();
    public IEntityBase PartCopy() => OnPartCopy();

    /// <summary>
    /// Constructor for entity of height 1.
    /// </summary>
    protected EntityBase(EntityInfo entityInfo)
    {
        _entityInfo = entityInfo;
    }

    protected EntityBase(EntityBase source, EntityInfo entityInfo) : this(entityInfo) { }

    protected EntityBase(EntityInfo entityInfo, ImmutableArray<ReadOnlyMemory<byte>> buffers)
    {
        // todo structure checks
        if (buffers.Length != (entityInfo.ClassHeight + 1)) throw new InvalidDataException($"Expected {entityInfo.ClassHeight + 1} buffers but received {buffers.Length}");
        _entityInfo = entityInfo;
        _frozen = true;
    }

    #region helpers
    [MethodImpl(MethodImplOptions.NoInlining)]
    private void ThrowIsNotFrozenException(string? methodName) => throw new InvalidOperationException($"Cannot {methodName} when not frozen.");

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected void ThrowIfNotFrozen([CallerMemberName] string? methodName = null)
    {
        if (!_frozen) ThrowIsNotFrozenException(methodName);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void ThrowIsFrozenException(string? methodName) => throw new InvalidOperationException($"Cannot {methodName} when frozen.");

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected void ThrowIfFrozen([CallerMemberName] string? methodName = null)
    {
        if (_frozen) ThrowIsFrozenException(methodName);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void ThrowIsNotPackedException(string? methodName) => throw new InvalidOperationException($"Cannot {methodName} before packing.");

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected void ThrowIfNotPacked([CallerMemberName] string? methodName = null)
    {
        if (!_packed) ThrowIsNotPackedException(methodName);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void ThrowIsNotUnpackedException(string? methodName) => throw new InvalidOperationException($"Cannot {methodName} before unpacking.");

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected void ThrowIfNotUnpacked([CallerMemberName] string? methodName = null)
    {
        if (_frozen && !_unpacked) ThrowIsNotUnpackedException(methodName);
    }
    #endregion

    #region freezing
    private volatile bool _frozen;
    public bool IsFrozen => _frozen;
    protected virtual void OnFreeze() { }
    public void Freeze()
    {
        if (_frozen) return;
        ThrowIfNotPacked();
        OnFreeze();
        _frozen = true;
    }
    #endregion

    #region packing
    private volatile bool _packed;
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
    #endregion

    #region unpacking
    private volatile bool _unpacked;
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

    #region equality
    public bool Equals(EntityBase? that)
    {
        if (ReferenceEquals(this, that)) return true;
        if (that is null) return false;
        if (that._entityInfo != _entityInfo) return false;
        return true;
    }

    public override bool Equals(object? obj) => obj is EntityBase;

    private int CalcHashCode() => _entityInfo.GetHashCode();

    private int? _hashCode;
    public override int GetHashCode()
    {
        if (!_frozen) return CalcHashCode();
        if (_hashCode.HasValue) return _hashCode.Value;
        _hashCode = CalcHashCode();
        return _hashCode.Value;
    }
    #endregion
}
