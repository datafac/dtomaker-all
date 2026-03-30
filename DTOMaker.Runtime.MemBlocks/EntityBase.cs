using DataFac.Memory;
using DataFac.Storage;
using DTOMaker.Models;
using System;
using System.Buffers;
using System.Collections.Immutable;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace DTOMaker.Runtime.MemBlocks;

public abstract class EntityBase : IEntityBase, IMemoryBlockEntity, IEquatable<EntityBase>
{
    #region Static Helpers
    public static async ValueTask<T> CreateEmpty<T>(IDataStore dataStore) where T : class, IMemoryBlockEntity, IEntityBase, new()
    {
        var empty = new T();
        await empty.Pack(dataStore);
        empty.Freeze();
        return empty;
    }
    #endregion

    private const int ClassHeight = 0;
    private const int BlockLength = EntityMetadata.HeaderSize; // V1.0

    protected readonly ReadOnlyMemory<byte> _readonlyGlobalBlock;
    protected readonly Memory<byte> _writableGlobalBlock;

    protected abstract int OnGetEntityId();
    public int GetEntityId() => OnGetEntityId();

    /// <summary>
    /// Constructor for an empty, unfrozen entity.
    /// </summary>
    protected EntityBase(EntityMetadata metadata)
    {
        _readonlyGlobalBlock = _writableGlobalBlock = new byte[metadata.TotalLength];
        metadata.WriteTo(_writableGlobalBlock.Span);
    }

    /// <summary>
    /// Constructor for an unfrozen entity with content copied from source.
    /// </summary>
    protected EntityBase(EntityMetadata metadata, EntityBase source) : this(metadata) { }

    /// <summary>
    /// Constructor for an unfrozen entity with content copied from source.
    /// </summary>
    protected EntityBase(EntityMetadata metadata, IEntityBase source) : this(metadata)
    {
    }

    /// <summary>
    /// Constructor for a frozen entity with content copied from source.
    /// </summary>
    protected EntityBase(EntityMetadata metadata, ReadOnlyMemory<byte> buffer)
    {
        // get incoming header and compare
        EntityMetadata receivedMetadata = new EntityMetadata(buffer);
        var receivedLength = receivedMetadata.TotalLength;
        if (receivedLength != buffer.Length) throw new InvalidDataException($"Received header length ({receivedLength}) does not match actual buffer length ({buffer.Length})");
        if (receivedMetadata != metadata) throw new InvalidDataException($"Received [{receivedMetadata}] does not match current metadata [{metadata}]");
        var currentLength = metadata.TotalLength;
        if (receivedLength != currentLength) throw new InvalidDataException($"Received length ({receivedLength}) does not match current length ({currentLength})");
        _readonlyGlobalBlock = buffer;
        _writableGlobalBlock = Memory<byte>.Empty;
        _frozen = true;
    }

    private volatile bool _frozen;
    public bool IsFrozen => _frozen;
    protected virtual void OnFreeze() { }
    public void Freeze()
    {
        if (_frozen) return;
        if (!_packed) ThrowIsNotPackedException();
        OnFreeze();
        _frozen = true;
    }

    public ReadOnlyMemory<byte> GetBuffer()
    {
        ThrowIfNotFrozen();
        return _readonlyGlobalBlock;
    }

    protected virtual IEntityBase OnPartCopy() => throw new NotImplementedException();
    public IEntityBase PartCopy() => OnPartCopy();

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void ThrowIsFrozenException(string? methodName) => throw new InvalidOperationException($"Cannot set {methodName} when frozen.");

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected void ThrowIfFrozen([CallerMemberName] string? methodName = null)
    {
        if (_frozen) ThrowIsFrozenException(methodName);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected T IfNotFrozen<T>(T value, [CallerMemberName] string? methodName = null)
    {
        if (_frozen) ThrowIsFrozenException(methodName);
        return value;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void ThrowIsNotFrozenException(string? methodName) => throw new InvalidOperationException($"Cannot call {methodName} when not frozen.");

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected void ThrowIfNotFrozen([CallerMemberName] string? methodName = null)
    {
        if (!_frozen) ThrowIsNotFrozenException(methodName);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void ThrowIsNotPackedException() => throw new InvalidOperationException($"Cannot freeze before packing.");

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void ThrowIsNotUnpackedException(string? methodName) => throw new InvalidOperationException($"Cannot call {methodName} before unpacking.");

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected void ThrowIfNotUnpacked([CallerMemberName] string? methodName = null)
    {
        if (_frozen && !_unpacked) ThrowIsNotUnpackedException(methodName);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected T IfUnpacked<T>(T value, [CallerMemberName] string? methodName = null)
    {
        if (_frozen && !_unpacked) ThrowIsNotUnpackedException(methodName);
        return value;
    }

    protected static T IfNotNull<T>(T? value, [CallerMemberName] string? methodName = null) where T : class
    {
        if (value is not null) return value;
        throw new InvalidOperationException($"Cannot call {methodName} when not set.");
    }

    public bool Equals(EntityBase? that)
    {
        if (ReferenceEquals(this, that)) return true;
        if (that is null) return false;
        if (!_readonlyGlobalBlock.Span.SequenceEqual(that._readonlyGlobalBlock.Span)) return false;
        return true;
    }

    public override bool Equals(object? obj) => obj is EntityBase other && Equals(other);

    private int CalcHashCode()
    {
        HashCode result = new HashCode();
        var span = _readonlyGlobalBlock.Span;
        result.Add(span.Length);
#if NET8_0_OR_GREATER
            result.AddBytes(span);
#else
        for (int i = 0; i < span.Length; i++)
        {
            result.Add(span[i]);
        }
#endif
        return result.ToHashCode();
    }

    private int? _hashCode;
    public override int GetHashCode()
    {
        if (!_frozen) return CalcHashCode();
        if (_hashCode.HasValue) return _hashCode.Value;
        _hashCode = CalcHashCode();
        return _hashCode.Value;
    }

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
}
