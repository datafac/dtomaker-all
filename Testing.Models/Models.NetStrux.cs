using DataFac.Memory;
using DataFac.Storage;
using DTOMaker.Models;
using DTOMaker.Runtime;
using DTOMaker.Runtime.NetStrux;
using System;
using System.Collections.Immutable;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Testing.Models.NetStrux;

public sealed partial class Required_String : DTOMaker.Runtime.NetStrux.EntityBase, Testing.Models.IRequired_String, IEquatable<Required_String>
{
    protected override IEntityBase OnPartCopy() => new Required_String(this);

    protected override void OnFreeze() => base.OnFreeze();

    private const long StructureCode = 0x00000071;
    private const int EntityId = 3;
    private const int ClassHeight = 1;
    private const int BlockLength = 64;

    private static readonly EntityMetadata _metadata = new EntityMetadata(EntityId, StructureCode);

    private BlockB064 _block;

    protected override void OnGetBuffers(Span<ReadOnlyMemory<byte>> buffers)
    {
        base.OnGetBuffers(buffers);
        Memory<byte> buffer = new byte[_block.BlockSize];
        _block.WriteTo(buffer.Span);
        buffers[ClassHeight] = buffer;
    }

    public Required_String() : base(_metadata)
    {
        _Field = string.Empty;
    }
    public Required_String(Required_String source) : base(source, _metadata)
    {
        _Field = source._Field;
    }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public Required_String(EntityContent content) : base(_metadata, content)
    {
        if (!_block.TryRead(content.Buffers[ClassHeight].Span)) throw new InvalidDataException($"buffers[{ClassHeight}].Length must be at least {_block.BlockSize} bytes");
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    protected override async ValueTask OnPack(IDataStore dataStore)
    {
        await base.OnPack(dataStore);
        await Field_Pack(dataStore);
    }
    protected override async ValueTask OnUnpack(IDataStore dataStore, int depth)
    {
        await base.OnUnpack(dataStore, depth);
        await Field_Unpack(dataStore);
    }

    // -------------------- Field --------------------
    private async ValueTask Field_Pack(IDataStore dataStore)
    {
        BlobIdV1 blobId = await dataStore.PutBlob(_Field);
        _block = blobId.Block;
    }
    private async ValueTask Field_Unpack(IDataStore dataStore)
    {
        BlobIdV1 blobId = BlobIdV1.FromBlock(_block);
        var blob = await dataStore.GetBlob(blobId);
#if NET8_0_OR_GREATER
        _Field = blob is null ? string.Empty : System.Text.Encoding.UTF8.GetString(blob.Value);
#else
        _Field = blob is null ? string.Empty : System.Text.Encoding.UTF8.GetString(blob.Value.ToArray());
#endif
    }
    private string _Field;
    /// <inheritdoc/>
    public string Field
    {
        get
        {
            ThrowIfNotUnpacked();
            return _Field;
        }

        set
        {
            ThrowIfFrozen();
            _Field = value;
        }
    }

    /// <inheritdoc/>
    public bool Equals(Required_String? that)
    {
        if (ReferenceEquals(this, that)) return true;
        if (that is null) return false;
        if (!base.Equals(that)) return false;
        if (that._block != _block) return false;
        return true;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is Required_String other && Equals(other);
    /// <inheritdoc/>
    public override int GetHashCode() => base.GetHashCode();
    /// <inheritdoc/>
    public static bool operator ==(Required_String? left, Required_String? right) => left is not null ? left.Equals(right) : (right is null);
    /// <inheritdoc/>
    public static bool operator !=(Required_String? left, Required_String? right) => left is not null ? !left.Equals(right) : (right is not null);

}
