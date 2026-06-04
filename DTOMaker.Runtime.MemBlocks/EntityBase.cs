using DataFac.MemBlox2;
using DataFac.Storage;
using DTOMaker.Models;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace DTOMaker.Runtime.MemBlocks;

public abstract class EntityBase : IEntityBase, IPackable, IEquatable<EntityBase>
{
    public static async ValueTask<T> CreateEmpty<T>(IDataStore dataStore, CancellationToken cancellation) where T : class, IPackable, IEntityBase, new()
    {
        var empty = new T();
        await empty.Pack(dataStore, cancellation);
        empty.Freeze();
        return empty;
    }

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
        _packed = true;
    }

    private volatile bool _frozen;
    public bool IsFrozen => _frozen;
    protected virtual void OnFreeze() { }
    public void Freeze()
    {
        if (_frozen) return;
        if (!_packed) EntityBase.ThrowIsNotPackedException(nameof(Freeze));
        OnFreeze();
        _frozen = true;
    }

    public ReadOnlyMemory<byte> Serialize(CancellationToken cancellation)
    {
        if (!_packed) EntityBase.ThrowIsNotPackedException(nameof(Serialize));
        return _readonlyGlobalBlock;
    }

    protected virtual IEntityBase OnShallowCopy() => throw new NotImplementedException();
    public IEntityBase ShallowCopy() => OnShallowCopy();

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void ThrowIsFrozenException(string? methodName) => throw new InvalidOperationException($"Cannot set {methodName} when frozen.");

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected void ThrowIfFrozen([CallerMemberName] string? methodName = null)
    {
        if (_frozen) EntityBase.ThrowIsFrozenException(methodName);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected T IfNotFrozen<T>(T value, [CallerMemberName] string? methodName = null)
    {
        if (_frozen) EntityBase.ThrowIsFrozenException(methodName);
        return value;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void ThrowIsNotFrozenException(string? methodName) => throw new InvalidOperationException($"Cannot call {methodName} when not frozen.");

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected void ThrowIfNotFrozen([CallerMemberName] string? methodName = null)
    {
        if (!_frozen) EntityBase.ThrowIsNotFrozenException(methodName);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void ThrowIsNotPackedException(string? methodName) => throw new InvalidOperationException($"Cannot call {methodName} when not packed.");

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void ThrowIsNotUnpackedException(string? methodName) => throw new InvalidOperationException($"Cannot call {methodName} before unpacking.");

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected void ThrowIfNotUnpacked([CallerMemberName] string? methodName = null)
    {
        if (_frozen && !_unpacked) EntityBase.ThrowIsNotUnpackedException(methodName);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected T IfUnpacked<T>(T value, [CallerMemberName] string? methodName = null)
    {
        if (_frozen && !_unpacked) EntityBase.ThrowIsNotUnpackedException(methodName);
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static async ValueTask PackData(ReadOnlyMemory<byte>? buffer, Memory<byte> fieldMemory, IDataStore dataStore)
    {
        if (!buffer.HasValue)
        {
            fieldMemory.Span.Clear();
        }
        else
        {
            (bool embedded, ReadOnlyMemory<byte> compressed) = BlobHelpers.CompressData(buffer.Value, fieldMemory.Span);
            if (embedded) return;
            await dataStore.PutBlob(BlobKey.From(fieldMemory), BlobData.From(compressed));
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static async ValueTask<ReadOnlyMemory<byte>?> UnpackData(ReadOnlyMemory<byte> fieldMemory, IDataStore dataStore)
    {
        (bool embedded, ReadOnlyMemory<byte>? embeddedData) = BlobHelpers.TryGetEmbedded(fieldMemory);
        if (embedded) return embeddedData;

        BlobData data = await dataStore.GetBlob(BlobKey.From(fieldMemory));
        return data.HasValue 
            ? BlobHelpers.DecompressData(fieldMemory.Span, data.Bytes) 
            : (ReadOnlyMemory<byte>?)null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static async ValueTask PackText(string? text, Memory<byte> fieldMemory, IDataStore dataStore)
    {
        if (text is null)
        {
            fieldMemory.Span.Clear();
        }
        else
        {
            (bool embedded, ReadOnlyMemory<byte> compressed) = BlobHelpers.CompressText(text, fieldMemory.Span);
            if (embedded) return;
            await dataStore.PutBlob(BlobKey.From(fieldMemory), BlobData.From(compressed));
        }
    }

    private volatile bool _packed;
    /// <inheritdoc/>
    public bool IsPacked => _packed;
    /// <inheritdoc/>
    protected virtual ValueTask OnPack(IDataStore dataStore, CancellationToken cancellation) => default;
    /// <inheritdoc/>
    public async ValueTask Pack(IDataStore dataStore, CancellationToken cancellation)
    {
        if (_frozen) return;
        if (_packed) return;
        await OnPack(dataStore, cancellation);
        _packed = true;
        OnFreeze();
        _frozen = true;
        _unpacked = true;
    }

    private volatile bool _unpacked;
    /// <inheritdoc/>
    public bool IsUnpacked => _unpacked;
    /// <inheritdoc/>
    protected virtual ValueTask OnUnpack(IDataStore dataStore, int depth, CancellationToken cancellation) => default;
    /// <inheritdoc/>
    public async ValueTask Unpack(IDataStore dataStore, int depth, CancellationToken cancellation)
    {
        ThrowIfNotFrozen();
        if (depth < 0) return;
        if (_unpacked) return;
        await OnUnpack(dataStore, depth, cancellation);
        _unpacked = true;
    }
    /// <inheritdoc/>
    public ValueTask UnpackAll(IDataStore dataStore, CancellationToken cancellation) => Unpack(dataStore, int.MaxValue, cancellation);
}
