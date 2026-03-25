using DataFac.Memory;
using DataFac.Storage;
using DTOMaker.Models;
using System;
using System.Collections.Immutable;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace DTOMaker.Runtime.MemBlox2;

public abstract class EntityBase : IMemBlox2EntityBase, IEquatable<EntityBase>
{
    private readonly BlockHeader _header;
    protected readonly ImmutableArray<ReadOnlyMemory<byte>> _readonlyBuffers;
    protected readonly ImmutableArray<Memory<byte>> _writableBuffers;

    public ReadOnlyBuffers GetBuffers()
    {
        ThrowIfNotFrozen();
        return new ReadOnlyBuffers(_header, _readonlyBuffers);
    }

    protected abstract IEntityBase OnPartCopy();
    public IEntityBase PartCopy() => OnPartCopy();

    /// <summary>
    /// Constructor for entity of height 1.
    /// </summary>
    protected EntityBase(BlockHeader header, Memory<byte> block1)
    {
        _header = header;
        _writableBuffers = [Memory<byte>.Empty, block1];
        _readonlyBuffers = [header.Memory, block1];
    }

    protected EntityBase(EntityBase source, BlockHeader header, Memory<byte> block1) : this(header, block1) { }

    /// <summary>
    /// Returns the block size for the code.
    /// </summary>
    /// <param name="blockSizeCode">Must always be between 0 and 15 inclusive.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int GetBlockSize(int blockSizeCode)
    {
        if (blockSizeCode == 0) return 0;
        int blockSize = 1;
        for (int n = 1; n < blockSizeCode; n++)
        {
            blockSize *= 2;
        }
        return blockSize;
    }

    protected EntityBase(BlockHeader header, ReadOnlyBuffers received)
    {
        if (received.Header != header) throw new InvalidDataException($"Header invalid: Expected {header} but received {received.Header}");
        _header = header;
        _readonlyBuffers = received.Buffersqqq;
        _writableBuffers = ImmutableArray<Memory<byte>>.Empty;
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
        if (that._readonlyBuffers.Length != _readonlyBuffers.Length) return false;
        for (int b = 0; b < _readonlyBuffers.Length; b++)
        {
            var thatSpan = that._readonlyBuffers[b].Span;
            var thisSpan = this._readonlyBuffers[b].Span;
            if (!thatSpan.SequenceEqual(thisSpan)) return false;
        }
        return true;
    }

    public override bool Equals(object? obj) => obj is EntityBase;

    private int CalcHashCode()
    {
        HashCode result = new HashCode();
        result.Add(_readonlyBuffers.Length);
        for (int b = 0; b < _readonlyBuffers.Length; b++)
        {
            var thisSpan = this._readonlyBuffers[b].Span;
#if NET8_0_OR_GREATER
            result.AddBytes(thisSpan);
#else
            for (int i = 0; i < thisSpan.Length; i++)
            {
                result.Add(thisSpan[i]);
            }
#endif
        }
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
    #endregion
}
