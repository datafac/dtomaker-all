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

    protected override void OnSerialize3(IBufferWriter<byte> bufferWriter)
    {
        base.OnSerialize3(bufferWriter);
        var source = BlockHelper.AsReadOnlySpan(ref _block);
        var target = bufferWriter.GetSpan(source.Length);
        source.CopyTo(target);
        bufferWriter.Advance(source.Length);
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
    private static readonly FieldDef Field001_Def = Field001_Map.LastAdded;

    public Int64 Field1
    {
        get
        {
            var blockSpan = BlockHelper.AsReadOnlySpan(ref _block);
            var valueSpan = blockSpan.Slice(Field001_Def.Offset, Field001_Def.Length);
            return Codec_Int64_LE.ReadFromSpan(valueSpan);
        }
        set
        {
            var blockSpan = BlockHelper.AsWritableSpan(ref _block);
            var valueSpan = blockSpan.Slice(Field001_Def.Offset, Field001_Def.Length);
            Codec_Int64_LE.WriteToSpan(valueSpan, value);
        }
    }

    // -------------------- Field002 ----------------------------------------
    private static readonly BlockMap Field002Flags_Map = Field001_Map.AddField<bool>();
    private static readonly FieldDef Field002Flags_Def = Field002Flags_Map.LastAdded;
    private static readonly BlockMap Field002_Map = Field002Flags_Map.AddField<Int64>();
    private static readonly FieldDef Field002_Def = Field002_Map.LastAdded;
    public Int64? Field2
    {
        get
        {
            var blockSpan = BlockHelper.AsReadOnlySpan(ref _block);
            var flagsSpan = blockSpan.Slice(Field002Flags_Def.Offset, Field002Flags_Def.Length);
            if (!Codec_Boolean_LE.ReadFromSpan(flagsSpan)) return null;
            var valueSpan = blockSpan.Slice(Field002_Def.Offset, Field002_Def.Length);
            return Codec_Int64_LE.ReadFromSpan(valueSpan);
        }

        set
        {
            ThrowIfFrozen();
            var blockSpan = BlockHelper.AsWritableSpan(ref _block);
            var flagsSpan = blockSpan.Slice(Field002Flags_Def.Offset, Field002Flags_Def.Length);
            Codec_Boolean_LE.WriteToSpan(flagsSpan, value.HasValue);
            if (value.HasValue)
            {
                var valueSpan = blockSpan.Slice(Field002_Def.Offset, Field002_Def.Length);
                Codec_Int64_LE.WriteToSpan(valueSpan, value.Value);
            }
        }
    }

    // -------------------- Field003 ----------------------------------------
    private static readonly BlockMap Field003_Map = Field002_Map.AddField<String>(32);
    private static readonly FieldDef Field003_Def = Field003_Map.LastAdded;
    public String? Field3
    {
        get
        {
            var blockSpan = BlockHelper.AsReadOnlySpan(ref _block);
            var valueSpan = blockSpan.Slice(Field003_Def.Offset, Field003_Def.Length);
            var length = valueSpan[0];
            if (length == Byte.MaxValue) return null;
            if (length == 0) return string.Empty;
            return Encoding.UTF8.GetString(valueSpan.Slice(1, length));
        }

        set
        {
            ThrowIfFrozen();
            var blockSpan = BlockHelper.AsWritableSpan(ref _block);
            var valueSpan = blockSpan.Slice(Field003_Def.Offset, Field003_Def.Length);
            valueSpan.Clear();
            if (value is null)
            {
                valueSpan[0] = Byte.MaxValue;
                return;
            }
            if (Encoding.UTF8.TryGetBytes(value, valueSpan.Slice(1), out int lengthInBytes))
            {
                valueSpan[0] = (byte)lengthInBytes;
            }
            else
            {
                throw new ArgumentException($"String value is too long to fit in {Field003_Def.Length - 1} bytes.");
            }
        }
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

    protected override void OnSerialize3(IBufferWriter<byte> bufferWriter)
    {
        base.OnSerialize3(bufferWriter);
        var source = BlockHelper.AsReadOnlySpan(ref _block);
        var target = bufferWriter.GetSpan(source.Length);
        source.CopyTo(target);
        bufferWriter.Advance(source.Length);
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

