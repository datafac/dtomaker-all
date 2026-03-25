using DataFac.Storage;
using DTOMaker.Models;
using DTOMaker.Runtime;
using DTOMaker.Runtime.MemBlox2;
using System;
using System.Collections.Immutable;
using System.Text;
using System.Threading.Tasks;

namespace Testing.Models.MemBlox2;

public sealed partial class Required_String : DTOMaker.Runtime.MemBlox2.EntityBase, Testing.Models.IRequired_String, IEquatable<Required_String>
{
    protected override IEntityBase OnPartCopy() => new Required_String(this);

    protected override void OnFreeze() => base.OnFreeze();

    private const long StructureCode = 0x00000071;
    private const int EntityId = 3;
    private const int ClassHeight = 1;
    private const int BlockLength = 64;

    private static readonly EntityMetadata _metadata = new EntityMetadata(EntityId, StructureCode);

    public Required_String() : base(_metadata, new byte[BlockLength])
    {
        _Field = string.Empty;
    }
    public Required_String(Required_String source) : base(source, _metadata, new byte[BlockLength])
    {
        _Field = source._Field;
    }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public Required_String(EntityContent buffers) : base(_metadata, buffers)
    {
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
        blobId.WriteTo(_writableBuffers[ClassHeight].Slice(0, 64).Span);
    }
    private async ValueTask Field_Unpack(IDataStore dataStore)
    {
        BlobIdV1 blobId = BlobIdV1.FromSpan(_readonlyBuffers[ClassHeight].Slice(0, 64).Span);
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
    public bool Equals(Required_String? other) => base.Equals(other);
    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is Required_String other && Equals(other);
    /// <inheritdoc/>
    public override int GetHashCode() => base.GetHashCode();
    /// <inheritdoc/>
    public static bool operator ==(Required_String? left, Required_String? right) => left is not null ? left.Equals(right) : (right is null);
    /// <inheritdoc/>
    public static bool operator !=(Required_String? left, Required_String? right) => left is not null ? !left.Equals(right) : (right is not null);

}
