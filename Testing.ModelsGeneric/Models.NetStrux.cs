using DataFac.Memory;
using DataFac.UnsafeHelpers;
using DTOMaker.Models;
using DTOMaker.Runtime;
using DTOMaker.Runtime.NetStrux;
using System;
using System.Buffers;
using System.IO;
using System.Numerics;

namespace Testing.ModelsGeneric.NetStrux;

public abstract partial class Couple<T1, T2> : EntityBase, ICouple<T1, T2>, IEquatable<Couple<T1, T2>>, IPackable<Couple<T1, T2>>
{
    public const int EntityId = 2;
    private BlockB064 _block = default;
    public new static readonly int BlockOffset = DTOMaker.Runtime.NetStrux.EntityBase.TotalLength;
    public new static readonly int BlockLength = 64; // todo _block.StaticSize
    public new static readonly int TotalLength = BlockOffset + BlockLength;

    // ==================== fields begin ========================================
    private static readonly BlockMap Field000_Map = BlockMap.Empty;

    // -------------------- Field001 ----------------------------------------
    private static readonly BlockMap Field001_Map = Field000_Map.AddField<T1>();
    private static readonly FieldDefn<T1> Field001_Def = (FieldDefn<T1>)Field001_Map.LastAdded;

    public T1 Item1
    {
        get => Field001_Def.ReadFromSpan(BlockHelper.AsReadOnlySpan(ref _block));
        set => Field001_Def.WriteToSpan(BlockHelper.AsWritableSpan(ref _block), IfNotFrozen(value));
    }

    // -------------------- Field002 ----------------------------------------
    private static readonly BlockMap Field002_Map = Field001_Map.AddField<T2>();
    private static readonly FieldDefn<T2> Field002_Def = (FieldDefn<T2>)Field002_Map.LastAdded;

    public T2 Item2
    {
        get => Field002_Def.ReadFromSpan(BlockHelper.AsReadOnlySpan(ref _block));
        set => Field002_Def.WriteToSpan(BlockHelper.AsWritableSpan(ref _block), IfNotFrozen(value));
    }

    //// -------------------- Field003 ----------------------------------------
    //private static readonly BlockMap Field003_Map = Field002_Map.AddField<String>(32);
    //private static readonly FieldDef Field003_Def = Field003_Map.LastAdded;
    //public String? Field3
    //{
    //    get
    //    {
    //        var blockSpan = BlockHelper.AsReadOnlySpan(ref _block);
    //        var valueSpan = blockSpan.Slice(Field003_Def.Offset, Field003_Def.Length);
    //        var length = valueSpan[0];
    //        if (length == Byte.MaxValue) return null;
    //        if (length == 0) return string.Empty;
    //        return Encoding.UTF8.GetString(valueSpan.Slice(1, length));
    //    }

    //    set
    //    {
    //        ThrowIfFrozen();
    //        var blockSpan = BlockHelper.AsWritableSpan(ref _block);
    //        var valueSpan = blockSpan.Slice(Field003_Def.Offset, Field003_Def.Length);
    //        valueSpan.Clear();
    //        if (value is null)
    //        {
    //            valueSpan[0] = Byte.MaxValue;
    //            return;
    //        }
    //        if (Encoding.UTF8.TryGetBytes(value, valueSpan.Slice(1), out int lengthInBytes))
    //        {
    //            valueSpan[0] = (byte)lengthInBytes;
    //        }
    //        else
    //        {
    //            throw new ArgumentException($"String value is too long to fit in {Field003_Def.Length - 1} bytes.");
    //        }
    //    }
    //}

    // -------------------- fields end ----------------------------------------
    private static readonly BlockMap Field999_Map = Field002_Map;

    //private static readonly int DeclaredBlockSize = _block.BlockSize;
    private static readonly int ComputedBlockSize = Field999_Map.BlockSize;

    // ==================== fields end ========================================

    protected Couple(int entityId) : base(entityId) { }
    protected Couple(Couple<T1, T2> source) : base(source) { _block = source._block; }

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

    public static Couple<T1, T2> DeserializeFrom(ReadOnlySpan<byte> buffer)
    {
        throw new InvalidOperationException($"Cannot deserialize abstract open generic type");
    }

    public bool Equals(Couple<T1, T2>? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;
        if (!base.Equals(other)) return false;
        if (other._block != this._block) return false;
        return true;
    }

    public override bool Equals(object? obj) => obj is Couple<T1, T2> other && Equals(other);
    public override int GetHashCode()
    {
        HashCode hasher = new();
        hasher.Add(base.GetHashCode());
        hasher.Add(typeof(Couple<T1, T2>));
        hasher.AddBytes(BlockHelper.AsReadOnlySpan(ref _block));
        return hasher.ToHashCode();
    }
}

public sealed partial class MyCoupleA : Couple<int, string>, IMyCoupleA, IEquatable<MyCoupleA>, IPackable<MyCoupleA>
{
    public new const int EntityId = 11;
    private BlockB008 _block = default;
    public new static readonly int BlockOffset = Couple<int, string>.TotalLength;
    public new static readonly int BlockLength = 8; // todo _block.StaticSize
    public new static readonly int TotalLength = BlockOffset + BlockLength;

    // ==================== fields begin ========================================
    private static readonly BlockMap Field000_Map = BlockMap.Empty;

    // -------------------- fields end ----------------------------------------
    private static readonly BlockMap Field999_Map = Field000_Map;

    //private static readonly int DeclaredBlockSize = _block.BlockSize;
    private static readonly int ComputedBlockSize = Field999_Map.BlockSize;

    // ==================== fields end ========================================

    protected override IEntityBase OnShallowCopy() => new MyCoupleA(this);

    public MyCoupleA() : base(MyCoupleA.EntityId) { }
    public MyCoupleA(MyCoupleA source) : base(source) { _block = source._block; }

    protected override void OnSerialize(Span<byte> target)
    {
        base.OnSerialize(target);
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

    public static MyCoupleA DeserializeFrom(ReadOnlySpan<byte> buffer)
    {
        var result = new MyCoupleA();
        result.DeserializeSync(buffer);
        return result;
    }

    public bool Equals(MyCoupleA? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;
        if (!base.Equals(other)) return false;
        if (other._block != this._block) return false;
        return true;
    }

    public override bool Equals(object? obj) => obj is MyCoupleA other && Equals(other);
    public override int GetHashCode()
    {
        HashCode hasher = new();
        hasher.Add(base.GetHashCode());
        hasher.Add(typeof(MyCoupleA));
        hasher.AddBytes(BlockHelper.AsReadOnlySpan(ref _block));
        return hasher.ToHashCode();
    }
}
