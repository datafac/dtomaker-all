using DataFac.Memory;
using DataFac.Storage;
using DTOMaker.Models;
using System;
using System.Collections.Immutable;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace DTOMaker.Runtime.NetStrux;

public abstract class EntityBase : IMemoryBlockEntity, IEquatable<EntityBase>
{
    private readonly EntityMetadata _metadata;

    protected virtual void OnGetBuffers(Span<ReadOnlyMemory<byte>> buffers)
    {
        buffers[0] = _metadata.Memory;
    }

    public EntityContent GetBuffers()
    {
        ThrowIfNotFrozen();
        var buffers = new ReadOnlyMemory<byte>[_metadata.ClassHeight + 1];
        OnGetBuffers(buffers);
        return new EntityContent(_metadata, ImmutableArray<ReadOnlyMemory<byte>>.Empty.AddRange(buffers));
    }

    protected abstract IEntityBase OnPartCopy();
    public IEntityBase PartCopy() => OnPartCopy();

    /// <summary>
    /// Constructor for entity of height 1.
    /// </summary>
    protected EntityBase(EntityMetadata metadata)
    {
        _metadata = metadata;
    }

    protected EntityBase(EntityBase source, EntityMetadata metadata) : this(metadata) { }

    protected EntityBase(EntityMetadata metadata, EntityContent content)
    {
        // todo structure checks
        if (content.Buffers.Length != (metadata.ClassHeight + 1)) throw new InvalidDataException($"Expected {metadata.ClassHeight + 1} buffers but received {content.Buffers.Length}");
        _metadata = metadata;
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
        if (that._metadata != _metadata) return false;
        return true;
    }

    public override bool Equals(object? obj) => obj is EntityBase;

    private int CalcHashCode() => _metadata.GetHashCode();

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
