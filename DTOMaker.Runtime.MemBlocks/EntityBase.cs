using DataFac.Memory;
using DataFac.Storage;
using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace DTOMaker.Runtime.MemBlocks;

public interface IMemBlocksEntityBase : IEntityBase
{
    ReadOnlySequence<byte> GetBuffers();
    ValueTask Pack(IDataStore dataStore);
    ValueTask Unpack(IDataStore dataStore, int depth = 0);
    ValueTask UnpackAll(IDataStore dataStore);
}

public interface IMemBlocksEntityFactory<T>
{
    T CreateInstance(ReadOnlySequence<byte> buffers);
}

public interface IMemBlocksEntity<T> : IMemBlocksEntityBase
{
#if NET8_0_OR_GREATER
    static abstract T CreateInstance(ReadOnlySequence<byte> buffers);
#endif
    IMemBlocksEntityFactory<T> GetFactory();
}

public abstract class EntityBase : IMemBlocksEntityBase, IEquatable<EntityBase>
{
    #region Static Helpers
    public static async ValueTask<T> CreateEmpty<T>(IDataStore dataStore) where T : class, IMemBlocksEntityBase, IEntityBase, new()
    {
        var empty = new T();
        await empty.Pack(dataStore);
        empty.Freeze();
        return empty;
    }
    #endregion

    private const int ClassHeight = 0;
    private const int BlockLength = BlockHeader.HeaderSize; // V1.0
    private readonly Memory<byte> _writableLocalBlock;
    private readonly ReadOnlyMemory<byte> _readonlyLocalBlock;

    protected abstract int OnGetEntityId();
    public int GetEntityId() => OnGetEntityId();

    protected EntityBase(BlockHeader blockHeader)
    {
        _readonlyLocalBlock = _writableLocalBlock = new byte[BlockLength];
        blockHeader.Memory.CopyTo(_writableLocalBlock);
    }
    protected EntityBase(BlockHeader blockHeader, object source)
    {
        _readonlyLocalBlock = _writableLocalBlock = new byte[BlockLength];
        blockHeader.Memory.CopyTo(_writableLocalBlock);
    }
    protected EntityBase(BlockHeader blockHeader, SourceBlocks sourceBlocks)
    {
        if (sourceBlocks.Header != blockHeader) throw new NotSupportedException("Entity evolution not supported yet!");
        _readonlyLocalBlock = blockHeader.Memory;
        _writableLocalBlock = Memory<byte>.Empty;
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

    protected abstract int OnGetClassHeight();
    protected virtual ReadOnlySequenceBuilder<byte> OnSequenceBuilder(ReadOnlySequenceBuilder<byte> builder) => builder.Append(_readonlyLocalBlock);
    public ReadOnlySequence<byte> GetBuffers()
    {
        ThrowIfNotFrozen();
        return OnSequenceBuilder(new ReadOnlySequenceBuilder<byte>()).Build();
    }

    //public void LoadBuffer(ReadOnlyMemory<byte> buffer)
    //{
    //    ThrowIfFrozen();
    //    var buffers = DataFac.MemBlocks.Protocol.SplitBuffers(buffer);
    //    OnLoadBuffers(buffers);
    //}

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

    public bool Equals(EntityBase? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;
        if (!_readonlyLocalBlock.Span.SequenceEqual(other._readonlyLocalBlock.Span)) return false;
        return true;
    }

    public override bool Equals(object? obj) => obj is EntityBase;

    private int CalcHashCode()
    {
        HashCode result = new HashCode();
        result.Add(GetType());
        result.Add(_readonlyLocalBlock.Length);
#if NET8_0_OR_GREATER
            result.AddBytes(_readonlyLocalBlock.Span);
#else
        var byteSpan = _readonlyLocalBlock.Span;
        for (int i = 0; i < byteSpan.Length; i++)
        {
            result.Add(byteSpan[i]);
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
