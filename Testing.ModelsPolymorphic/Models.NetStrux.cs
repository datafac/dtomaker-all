using DataFac.Memory;
using DataFac.UnsafeHelpers;
using DTOMaker.Models;
using DTOMaker.Runtime;
using DTOMaker.Runtime.NetStrux;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Testing.ModelsPolymorphic.NetStrux;

public abstract partial class NumberBase : DTOMaker.Runtime.NetStrux.EntityBase, INumberBase, IEquatable<NumberBase>, IPackable<NumberBase>
{
    private BlockB008 _block = default; // todo placeholder
    public new static readonly int BlockOffset = DTOMaker.Runtime.NetStrux.EntityBase.TotalLength;
    public new static readonly int BlockLength = 8; // todo _block.StaticSize
    public new static readonly int TotalLength = BlockOffset + BlockLength;

    public NumberBase(int entityId) : base(entityId) { }
    public NumberBase(NumberBase source) : base(source) { _block = source._block; }

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

    public static NumberBase DeserializeFrom(ReadOnlySpan<byte> buffer)
    {
        var header = GetHeader(buffer);
        var entityId = header.EntityId;
        NumberBase result = entityId switch
        {
            FloatNumber.EntityId => new FloatNumber(),
            WholeNumber.EntityId => new WholeNumber(),
            _ => throw new InvalidDataException($"Unexpected entity id: {entityId}")
        };
        result.DeserializeSync(buffer);
        return result;
    }

    public bool Equals(NumberBase? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;
        if (!base.Equals(other)) return false;
        return true;
    }

    public override bool Equals(object? obj) => obj is NumberBase other && Equals(other);
    public override int GetHashCode()
    {
        HashCode hasher = new();
        hasher.Add(base.GetHashCode());
        hasher.Add(typeof(NumberBase));
        return hasher.ToHashCode();
    }
}

public sealed partial class FloatNumber : NumberBase, IFloatNumber, IEquatable<FloatNumber>, IPackable<FloatNumber>
{
    public const int EntityId = 3;
    private BlockB008 _block = default;
    public new static readonly int BlockOffset = NumberBase.TotalLength;
    public new static readonly int BlockLength = 8; // todo _block.StaticSize
    public new static readonly int TotalLength = BlockOffset + BlockLength;

    // ==================== fields begin ========================================
    private static readonly BlockMap Field000_Map = BlockMap.Empty;

    // -------------------- Field001 ----------------------------------------
    private static readonly BlockMap Field001_Map = Field000_Map.AddField<Double>();
    private static readonly FieldDef Field001_Def = Field001_Map.LastAdded;

    public Double Value
    {
        get
        {
            var blockSpan = BlockHelper.AsReadOnlySpan(ref _block);
            var valueSpan = blockSpan.Slice(Field001_Def.Offset, Field001_Def.Length);
            return Codec_Double_LE.ReadFromSpan(valueSpan);
        }
        set
        {
            ThrowIfFrozen();
            var blockSpan = BlockHelper.AsWritableSpan(ref _block);
            var valueSpan = blockSpan.Slice(Field001_Def.Offset, Field001_Def.Length);
            Codec_Double_LE.WriteToSpan(valueSpan, value);
        }
    }

    // -------------------- fields end ----------------------------------------
    private static readonly BlockMap Field999_Map = Field001_Map;

    //private static readonly int DeclaredBlockSize = _block.BlockSize;
    private static readonly int ComputedBlockSize = Field999_Map.BlockSize;

    // ==================== fields end ========================================

    protected override IEntityBase OnShallowCopy() => new FloatNumber(this);

    public FloatNumber() : base(FloatNumber.EntityId) { }
    public FloatNumber(FloatNumber source) : base(source) { _block = source._block; }

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

    protected override void OnSerialize3(IBufferWriter<byte> bufferWriter)
    {
        base.OnSerialize3(bufferWriter);
        var source = BlockHelper.AsReadOnlySpan(ref _block);
        var target = bufferWriter.GetSpan(source.Length);
        source.CopyTo(target);
        bufferWriter.Advance(source.Length);
    }

    public static FloatNumber DeserializeFrom(ReadOnlySpan<byte> buffer)
    {
        var result = new FloatNumber();
        result.DeserializeSync(buffer);
        return result;
    }

    public bool Equals(FloatNumber? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;
        if (!base.Equals(other)) return false;
        if (other._block != this._block) return false;
        return true;
    }

    public override bool Equals(object? obj) => obj is FloatNumber other && Equals(other);
    public override int GetHashCode()
    {
        HashCode hasher = new();
        hasher.Add(base.GetHashCode());
        hasher.Add(typeof(FloatNumber));
        hasher.AddBytes(BlockHelper.AsReadOnlySpan(ref _block));
        return hasher.ToHashCode();
    }
}

public sealed partial class WholeNumber : NumberBase, IWholeNumber, IEquatable<WholeNumber>, IPackable<WholeNumber>
{
    public const int EntityId = 4;
    private BlockB008 _block = default;
    public new static readonly int BlockOffset = NumberBase.TotalLength;
    public new static readonly int BlockLength = 8; // todo _block.StaticSize
    public new static readonly int TotalLength = BlockOffset + BlockLength;

    // ==================== fields begin ========================================
    private static readonly BlockMap Field000_Map = BlockMap.Empty;

    // -------------------- Field001 ----------------------------------------
    private static readonly BlockMap Field001_Map = Field000_Map.AddField<Int64>();
    private static readonly FieldDef Field001_Def = Field001_Map.LastAdded;

    public Int64 Value
    {
        get
        {
            var blockSpan = BlockHelper.AsReadOnlySpan(ref _block);
            var valueSpan = blockSpan.Slice(Field001_Def.Offset, Field001_Def.Length);
            return Codec_Int64_LE.ReadFromSpan(valueSpan);
        }
        set
        {
            ThrowIfFrozen();
            var blockSpan = BlockHelper.AsWritableSpan(ref _block);
            var valueSpan = blockSpan.Slice(Field001_Def.Offset, Field001_Def.Length);
            Codec_Int64_LE.WriteToSpan(valueSpan, value);
        }
    }

    // -------------------- fields end ----------------------------------------
    private static readonly BlockMap Field999_Map = Field001_Map;

    //private static readonly int DeclaredBlockSize = _block.BlockSize;
    private static readonly int ComputedBlockSize = Field999_Map.BlockSize;

    // ==================== fields end ========================================

    protected override IEntityBase OnShallowCopy() => new WholeNumber(this);

    public WholeNumber() : base(WholeNumber.EntityId) { }
    public WholeNumber(WholeNumber source) : base(source) { _block = source._block; }

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

    protected override void OnSerialize3(IBufferWriter<byte> bufferWriter)
    {
        base.OnSerialize3(bufferWriter);
        var source = BlockHelper.AsReadOnlySpan(ref _block);
        var target = bufferWriter.GetSpan(source.Length);
        source.CopyTo(target);
        bufferWriter.Advance(source.Length);
    }

    public static WholeNumber DeserializeFrom(ReadOnlySpan<byte> buffer)
    {
        var result = new WholeNumber();
        result.DeserializeSync(buffer);
        return result;
    }

    public bool Equals(WholeNumber? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;
        if (!base.Equals(other)) return false;
        if (other._block != this._block) return false;
        return true;
    }

    public override bool Equals(object? obj) => obj is WholeNumber other && Equals(other);
    public override int GetHashCode()
    {
        HashCode hasher = new();
        hasher.Add(base.GetHashCode());
        hasher.Add(typeof(WholeNumber));
        hasher.AddBytes(BlockHelper.AsReadOnlySpan(ref _block));
        return hasher.ToHashCode();
    }
}
