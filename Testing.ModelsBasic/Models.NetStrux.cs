using DataFac.Memory;
using DataFac.UnsafeHelpers;
using DTOMaker.Models;
using DTOMaker.Runtime;
using DTOMaker.Runtime.NetStrux;
using System;
using System.Buffers;
using System.Text;

namespace Testing.ModelsBasic.NetStrux;

public sealed partial class Nothing : EntityBase, INothing, IEquatable<Nothing>, IPackable<Nothing>
{
    public const int EntityId = 1;
    private BlockB008 _block = default;
    public new static readonly int BlockOffset = DTOMaker.Runtime.NetStrux.EntityBase.TotalLength;
    public new static readonly int BlockLength = 8; // todo _block.StaticSize
    public new static readonly int TotalLength = BlockOffset + BlockLength;

    // ==================== fields begin ========================================
    private static readonly BlockMap Field000_Map = BlockMap.Empty;

    // -------------------- fields end ----------------------------------------
    private static readonly BlockMap Field999_Map = Field000_Map;

    //private static readonly int DeclaredBlockSize = _block.BlockSize;
    private static readonly int ComputedBlockSize = Field999_Map.BlockSize;

    // ==================== fields end ========================================

    protected override IEntityBase OnShallowCopy() => new Nothing(this);

    public Nothing() : base(Nothing.EntityId) { }
    public Nothing(Nothing source) : base(source) { _block = source._block; }

    protected override void OnSerialize(Span<byte> target)
    {
        _block.WriteTo(target.Slice(BlockOffset, BlockLength));
    }

    protected override void OnDeserialize(ReadOnlySpan<byte> source)
    {
        _block.TryRead(source.Slice(BlockOffset, BlockLength));
    }

    protected override ReadOnlyMemory<byte> OnOuterSerialize()
    {
        Memory<byte> buffer = new byte[TotalLength];
        OnSerialize(buffer.Span);
        return buffer;
    }

    public static Nothing DeserializeFrom(ReadOnlySpan<byte> buffer)
    {
        // todo polymorphic deserialization
        var result = new Nothing();
        result.DeserializeSync(buffer);
        return result;
    }

    public bool Equals(Nothing? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;
        if (!base.Equals(other)) return false;
        if (other._block != this._block) return false;
        return true;
    }

    public override bool Equals(object? obj) => obj is Nothing other && Equals(other);
    public override int GetHashCode()
    {
        HashCode hasher = new();
        hasher.Add(base.GetHashCode());
        hasher.Add(typeof(Nothing));
        hasher.AddBytes(BlockHelper.AsReadOnlySpan(ref _block));
        return hasher.ToHashCode();
    }
}

public sealed partial class Simple : DTOMaker.Runtime.NetStrux.EntityBase, ISimple, IEquatable<Simple>, IPackable<Simple>
{
    public const int EntityId = 2;
    private BlockB064 _block = default;
    public new static readonly int BlockOffset = DTOMaker.Runtime.NetStrux.EntityBase.TotalLength;
    public new static readonly int BlockLength = 64; // todo _block.StaticSize
    public new static readonly int TotalLength = BlockOffset + BlockLength;

    // ==================== fields begin ========================================
    private static readonly BlockMap Field000_Map = BlockMap.Empty;

    // -------------------- Field001 ----------------------------------------
    private static readonly BlockMap Field001_Map = Field000_Map.AddField<Int64>();
    private static readonly FieldDefn<Int64> Field001_Def = (FieldDefn<Int64>)Field001_Map.LastAdded;

    public Int64 Field1
    {
        get => Field001_Def.ReadFromSpan(BlockHelper.AsReadOnlySpan(ref _block));
        set => Field001_Def.WriteToSpan(BlockHelper.AsWritableSpan(ref _block), IfNotFrozen(value));
    }

    // -------------------- Field002 ----------------------------------------
    private static readonly BlockMap Field002_Map = Field001_Map.AddField<Int64?>();
    private static readonly FieldDefn<Int64?> Field002_Def = (FieldDefn<Int64?>)Field002_Map.LastAdded;

    public Int64? Field2
    {
        get => Field002_Def.ReadFromSpan(BlockHelper.AsReadOnlySpan(ref _block));
        set => Field002_Def.WriteToSpan(BlockHelper.AsWritableSpan(ref _block), IfNotFrozen(value));
    }

    // -------------------- Field003 ----------------------------------------
    private static readonly BlockMap Field003_Map = Field002_Map.AddField<String?>();
    private static readonly FieldDefn<String?> Field003_Def = (FieldDefn<String?>)Field003_Map.LastAdded;

    public String? Field3
    {
        get => Field003_Def.ReadFromSpan(BlockHelper.AsReadOnlySpan(ref _block));
        set => Field003_Def.WriteToSpan(BlockHelper.AsWritableSpan(ref _block), IfNotFrozen(value));
    }

    // -------------------- fields end ----------------------------------------
    private static readonly BlockMap Field999_Map = Field003_Map;

    //private static readonly int DeclaredBlockSize = _block.BlockSize;
    private static readonly int ComputedBlockSize = Field999_Map.BlockSize;

    // ==================== fields end ========================================

    protected override IEntityBase OnShallowCopy() => new Simple(this);

    public Simple() : base(Simple.EntityId) { }
    public Simple(Simple source) : base(source) { _block = source._block; }

    protected override void OnSerialize(Span<byte> target)
    {
        _block.WriteTo(target.Slice(BlockOffset, BlockLength));
    }

    protected override void OnDeserialize(ReadOnlySpan<byte> source)
    {
        _block.TryRead(source.Slice(BlockOffset, BlockLength));
    }

    protected override ReadOnlyMemory<byte> OnOuterSerialize()
    {
        Memory<byte> buffer = new byte[TotalLength];
        OnSerialize(buffer.Span);
        return buffer;
    }

    public static Simple DeserializeFrom(ReadOnlySpan<byte> buffer)
    {
        // todo polymorphic deserialization
        var result = new Simple();
        result.DeserializeSync(buffer);
        return result;
    }

    public bool Equals(Simple? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;
        if (!base.Equals(other)) return false;
        if (other._block != this._block) return false;
        return true;
    }

    public override bool Equals(object? obj) => obj is Simple other && Equals(other);
    public override int GetHashCode()
    {
        HashCode hasher = new();
        hasher.Add(base.GetHashCode());
        hasher.Add(typeof(Simple));
        hasher.AddBytes(BlockHelper.AsReadOnlySpan(ref _block));
        return hasher.ToHashCode();
    }
}

