using DataFac.Memory;
using DataFac.Storage;
using DataFac.UnsafeHelpers;
using DTOMaker.Models;
using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Collections;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace DTOMaker.Runtime.NetStrux
{
    public abstract class EntityBase : IEntityBase, IPackable, IEquatable<EntityBase>
    {
        [MethodImpl(MethodImplOptions.NoInlining)] private static void ThrowIsFrozenException(string? methodName) => throw new InvalidOperationException($"Cannot set {methodName} when frozen.");
        [MethodImpl(MethodImplOptions.NoInlining)] private static void ThrowIsNotFrozenException(string? methodName) => throw new InvalidOperationException($"Cannot call {methodName} when not frozen.");
        [MethodImpl(MethodImplOptions.NoInlining)] private static void ThrowIsNotPackedException(string? methodName) => throw new InvalidOperationException($"Cannot call {methodName} when not packed.");

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void ThrowIfFrozen([CallerMemberName] string? methodName = null)
        {
            if (_frozen) EntityBase.ThrowIsFrozenException(methodName);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void ThrowIfNotFrozen([CallerMemberName] string? methodName = null)
        {
            if (!_frozen) EntityBase.ThrowIsNotFrozenException(methodName);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void ThrowIfNotPacked([CallerMemberName] string? methodName = null)
        {
            if (!_packed) ThrowIsNotPackedException(methodName);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected T IfNotFrozen<T>(in T value, [CallerMemberName] string? methodName = null)
        {
            if (_frozen) EntityBase.ThrowIsFrozenException(methodName);
            return value;
        }

        private volatile bool _frozen;
        public bool IsFrozen() => _frozen;
        protected virtual void OnFreeze() { }
        public void Freeze()
        {
            if (_frozen) return;
            if (!_packed) EntityBase.ThrowIsNotPackedException(nameof(Freeze));
            OnFreeze();
            _frozen = true;
        }

        protected abstract IEntityBase OnShallowCopy();
        public IEntityBase ShallowCopy() => OnShallowCopy();

        // todo? reserve header
        private BlockB004 _block = default;
        public static readonly int BlockOffset = 0;
        public static readonly int BlockLength = 4; // todo _block.StaticSize
        public static readonly int TotalLength = BlockOffset + BlockLength;

        protected readonly struct Header
        {
            public readonly int EntityId;
            public Header(int entityId)
            {
                EntityId = entityId;
            }
        }
        protected static Header GetHeader(ReadOnlySpan<byte> buffer)
        {
            int entityId = BinaryPrimitives.ReadInt32LittleEndian(buffer);
            return new Header(entityId);
        }

        public int GetEntityId() => _block.Int32ValueLE;
        protected EntityBase(int entityId) { _block.Int32ValueLE = entityId; }
        protected EntityBase(EntityBase source) { _block.Int32ValueLE = source.GetEntityId(); }

        /// <summary>
        /// When implemented in a derived class, serializes the entity's data into a byte array.
        /// </summary>
        /// <returns></returns>
        protected virtual void OnSerialize(Span<byte> target)
        {
            _block.WriteTo(target.Slice(BlockOffset, BlockLength));
        }

        /// <summary>
        /// Restores the state of the object from the provided serialized data.
        /// </summary>
        /// <remarks>Override to customize deserialization behavior for derived types. The base
        /// implementation does nothing.</remarks>
        /// <param name="source">The serialized data used to restore the object's state.</param>
        protected abstract void OnDeserialize(ReadOnlySpan<byte> source);

        protected abstract ReadOnlyMemory<byte> OnOuterSerialize();

        public ReadOnlyMemory<byte> SerializeSync()
        {
            ThrowIfNotPacked();
            var buffer = OnOuterSerialize();
            return buffer;
        }

        public void DeserializeSync(ReadOnlySpan<byte> buffer)
        {
            ThrowIfFrozen();
            OnDeserialize(buffer);
        }

        private volatile bool _packed;
        /// <inheritdoc/>
        public bool IsPacked() => _packed;
        /// <inheritdoc/>
        protected virtual ValueTask OnPack(IBlobStore blobStore, CancellationToken cancellation) => default;
        /// <inheritdoc/>
        public async ValueTask Pack(IBlobStore blobStore, CancellationToken cancellation)
        {
            if (_frozen) return;
            if (_packed) return;
            await OnPack(blobStore, cancellation);
            _packed = true;
            OnFreeze();
            _frozen = true;
            _unpacked = true;
        }

        private volatile bool _unpacked;
        /// <inheritdoc/>
        public bool IsUnpacked() => _unpacked;
        /// <inheritdoc/>
        protected virtual ValueTask OnUnpack(IBlobStore blobStore, int depth, CancellationToken cancellation) => default;
        /// <inheritdoc/>
        public async ValueTask Unpack(IBlobStore blobStore, int depth, CancellationToken cancellation)
        {
            ThrowIfNotFrozen();
            if (depth < 0) return;
            if (_unpacked) return;
            await OnUnpack(blobStore, depth, cancellation);
            _unpacked = true;
        }
        /// <inheritdoc/>
        public ValueTask UnpackAll(IBlobStore blobStore, CancellationToken cancellation) => Unpack(blobStore, int.MaxValue, cancellation);

        /// <summary>
        /// Determines whether the this entity is equal to another entity of the same type.
        /// </summary>
        public bool Equals(EntityBase? other) => true;
        /// <summary>
        /// Determines whether the this entity is equal to another entity of the same type.
        /// </summary>
        public override bool Equals(object? obj) => obj is EntityBase;
        /// <summary>
        /// The hash function for the current entity.
        /// </summary>
        public override int GetHashCode() => HashCode.Combine<Type>(typeof(EntityBase));
    }

    public enum Fill
    {
        Zero,
        Part,
        Full
    }
    public sealed class BinaryMap
    {
        public Fill State { get; }
        public BinaryMap? A { get; }
        public BinaryMap? B { get; }

        private readonly sbyte _lenLog2;
        public int SizeInBits => PowerOf2(_lenLog2);
        public (int ByteCount, int BitsRemain) Sizes => (SizeInBits / 8, SizeInBits % 8);

        private readonly struct Result
        {
            public readonly bool Found;
            public readonly BinaryMap? NewMap;
            public readonly int Offset;
            public Result(bool found, BinaryMap? newMap, int offset)
            {
                Found = found;
                NewMap = newMap;
                Offset = offset;
            }
        }

        /// <summary>
        /// Default initial map contains 2^3 or 8 bits (1 byte)
        /// </summary>
        public static BinaryMap Empty { get; } = new BinaryMap(3, Fill.Zero, null, null);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int PowerOf2(sbyte x)
        {
            return 1 << x;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte Log2(int x)
        {
            sbyte result = 0;
            int test = 1;
            while (test < x)
            {
                result += 1;
                test *= 2;
            }
            return result;
        }

        private BinaryMap(sbyte lenLog2, Fill fill, BinaryMap? a, BinaryMap? b)
        {
            if (lenLog2 < 0)
                throw new ArgumentOutOfRangeException(nameof(lenLog2), lenLog2, $"Must be >= 0");
            _lenLog2 = lenLog2;
            A = a;
            B = b;
            State = fill;
        }

        public BinaryMap Expand()
        {
            var currentSizeInBytes = PowerOf2(_lenLog2) / 8;
            if (_lenLog2 >= 16)
                throw new InvalidOperationException($"Cannot expand block map beyond 8KiB (yet)");
            switch (State)
            {
                case Fill.Zero:
                    return new BinaryMap((sbyte)(_lenLog2 + 1), Fill.Zero, null, null);
                case Fill.Part:
                    return new BinaryMap((sbyte)(_lenLog2 + 1), Fill.Part, this, new BinaryMap(_lenLog2, Fill.Zero, null, null));
                case Fill.Full:
                    return new BinaryMap((sbyte)(_lenLog2 + 1), Fill.Part, this, new BinaryMap(_lenLog2, Fill.Zero, null, null));
                default:
                    throw new ArgumentOutOfRangeException(nameof(State), State, null);
            }
        }

        private static Fill CalcFill(Fill? a, Fill? b)
        {
            Fill ax = a ?? Fill.Zero;
            Fill bx = b ?? Fill.Zero;
            if (ax == Fill.Zero && bx == Fill.Zero)
                return Fill.Zero;
            if (ax == Fill.Full && bx == Fill.Full)
                return Fill.Full;
            return Fill.Part;
        }

        private Result assignField(sbyte bitlenLog2)
        {
            if (bitlenLog2 > _lenLog2)
                return new Result(false, null, 0);
            if (State == Fill.Full)
                return new Result(false, null, 0);
            if (bitlenLog2 == _lenLog2)
            {
                if (State == Fill.Zero)
                    return new Result(true, new BinaryMap(_lenLog2, Fill.Full, null, null), 0);
                else
                    return new Result(false, null, 0);
            }
            // check legs
            int half = 0;
            {
                var a = A ?? new BinaryMap((sbyte)(_lenLog2 - 1), Fill.Zero, null, null);
                var x = a.assignField(bitlenLog2);
                if (x.Found)
                    return new Result(true, new BinaryMap(_lenLog2, CalcFill(x.NewMap?.State, B?.State), x.NewMap, B), x.Offset);
                half = a.SizeInBits;
            }
            {
                var b = B ?? new BinaryMap((sbyte)(_lenLog2 - 1), Fill.Zero, null, null);
                var x = b.assignField(bitlenLog2);
                if (x.Found)
                    return new Result(true, new BinaryMap(_lenLog2, CalcFill(A?.State, x.NewMap?.State), A, x.NewMap), half + x.Offset);
            }
            return new Result(false, null, 0);
        }

        /// <summary>
        /// Finds and assigns a field of the given size (in bits).
        /// </summary>
        /// <param name="lengthInBits">The length of the field to assign (eg. byte = 8 bits, Int32 = 32 bits)</param>
        /// <returns>An updated map conatining the assignment.</returns>
        public (BinaryMap, int) AssignField(int lengthInBits)
        {
            const int maxLengthInBits = 8 * 1024 * 8; // 64KiB * 8 bits
            if (lengthInBits <= 0)
                throw new ArgumentOutOfRangeException(nameof(lengthInBits), lengthInBits, $"Must be > 0");
            if (lengthInBits > maxLengthInBits)
                throw new InvalidOperationException($"Cannot add fields larger than 8KiB (yet)");
            sbyte lengthLog2 = Log2(lengthInBits);
            bool found = false;
            BinaryMap? map = this;
            int offset = 0;
            while (!found)
            {
                var x = map!.assignField(lengthLog2);
                found = x.Found;
                if (found)
                {
                    map = x.NewMap;
                    offset = x.Offset;
                }
                else
                {
                    map = map.Expand();
                }
            }
            return (map!, offset);
        }
    }
    public sealed class BitAddress
    {
        public int Instance { get; }
        public int Position { get; }
        public int FieldOffset { get; }
        public int FieldLength { get; }
        public BitAddress(int instance, int position, int fieldOffset, int fieldLength)
        {
            Instance = instance;
            Position = position;
            FieldOffset = fieldOffset;
            FieldLength = fieldLength;
        }
    }
    internal interface ISpanReaderWriter<T>
    {
        T ReadFromSpan(ReadOnlySpan<byte> blockSpan, int valueOffset, int valueLength, int flagsOffset);
        void WriteToSpan(Span<byte> blockSpan, int valueOffset, int valueLength, int flagsOffset, T value);
    }
    internal sealed class SpanReaderWriter_OptionalBoolean : ISpanReaderWriter<Boolean?>
    {
        private static readonly SpanReaderWriter_OptionalBoolean _instance = new();
        public static SpanReaderWriter_OptionalBoolean Instance => _instance;
        public Boolean? ReadFromSpan(ReadOnlySpan<byte> blockSpan, int valueOffset, int valueLength, int flagsOffset)
        {
            return blockSpan[flagsOffset] == 0 ? null : (blockSpan[valueOffset] != 0);
        }

        public void WriteToSpan(Span<byte> blockSpan, int valueOffset, int valueLength, int flagsOffset, Boolean? value)
        {
            if (value.HasValue)
            {
                blockSpan[flagsOffset] = (byte)1;
                blockSpan[valueOffset] = value.Value ? (byte)1 : (byte)0;
            }
            else
            {
                blockSpan[flagsOffset] = (byte)0;
                blockSpan[valueOffset] = (byte)0;
            }
        }
    }
    internal sealed class SpanReaderWriter_RequiredBoolean : ISpanReaderWriter<Boolean>
    {
        private static readonly SpanReaderWriter_RequiredBoolean _instance = new();
        public static SpanReaderWriter_RequiredBoolean Instance => _instance;
        public Boolean ReadFromSpan(ReadOnlySpan<byte> blockSpan, int valueOffset, int valueLength, int flagsOffset)
        {
            return (blockSpan[valueOffset] != 0);
        }

        public void WriteToSpan(Span<byte> blockSpan, int valueOffset, int valueLength, int flagsOffset, Boolean value)
        {
            blockSpan[valueOffset] = value ? (byte)1 : (byte)0;
        }
    }
    internal sealed class SpanReaderWriter_OptionalByte : ISpanReaderWriter<Byte?>
    {
        private static readonly SpanReaderWriter_OptionalByte _instance = new();
        public static SpanReaderWriter_OptionalByte Instance => _instance;
        public Byte? ReadFromSpan(ReadOnlySpan<byte> blockSpan, int valueOffset, int valueLength, int flagsOffset)
        {
            return blockSpan[flagsOffset] == 0 ? null : (Byte)blockSpan[valueOffset];
        }

        public void WriteToSpan(Span<byte> blockSpan, int valueOffset, int valueLength, int flagsOffset, Byte? value)
        {
            if (value.HasValue)
            {
                blockSpan[flagsOffset] = (byte)1;
                blockSpan[valueOffset] = (byte)value.Value;
            }
            else
            {
                blockSpan[flagsOffset] = (byte)0;
                blockSpan[valueOffset] = (byte)0;
            }
        }
    }
    internal sealed class SpanReaderWriter_RequiredByte : ISpanReaderWriter<Byte>
    {
        private static readonly SpanReaderWriter_RequiredByte _instance = new();
        public static SpanReaderWriter_RequiredByte Instance => _instance;
        public Byte ReadFromSpan(ReadOnlySpan<byte> blockSpan, int valueOffset, int valueLength, int flagsOffset)
        {
            return (Byte)blockSpan[valueOffset];
        }

        public void WriteToSpan(Span<byte> blockSpan, int valueOffset, int valueLength, int flagsOffset, Byte value)
        {
            blockSpan[valueOffset] = (byte)value;
        }
    }
    internal sealed class SpanReaderWriter_OptionalSByte : ISpanReaderWriter<SByte?>
    {
        private static readonly SpanReaderWriter_OptionalSByte _instance = new();
        public static SpanReaderWriter_OptionalSByte Instance => _instance;
        public SByte? ReadFromSpan(ReadOnlySpan<byte> blockSpan, int valueOffset, int valueLength, int flagsOffset)
        {
            return blockSpan[flagsOffset] == 0 ? null : (SByte)blockSpan[valueOffset];
        }

        public void WriteToSpan(Span<byte> blockSpan, int valueOffset, int valueLength, int flagsOffset, SByte? value)
        {
            if (value.HasValue)
            {
                blockSpan[flagsOffset] = (byte)1;
                blockSpan[valueOffset] = (byte)value.Value;
            }
            else
            {
                blockSpan[flagsOffset] = (byte)0;
                blockSpan[valueOffset] = (byte)0;
            }
        }
    }
    internal sealed class SpanReaderWriter_RequiredSByte : ISpanReaderWriter<SByte>
    {
        private static readonly SpanReaderWriter_RequiredSByte _instance = new();
        public static SpanReaderWriter_RequiredSByte Instance => _instance;
        public SByte ReadFromSpan(ReadOnlySpan<byte> blockSpan, int valueOffset, int valueLength, int flagsOffset)
        {
            return (SByte)blockSpan[valueOffset];
        }

        public void WriteToSpan(Span<byte> blockSpan, int valueOffset, int valueLength, int flagsOffset, SByte value)
        {
            blockSpan[valueOffset] = (byte)value;
        }
    }
    internal sealed class SpanReaderWriter_OptionalInt16 : ISpanReaderWriter<Int16?>
    {
        private static readonly SpanReaderWriter_OptionalInt16 _instance = new();
        public static SpanReaderWriter_OptionalInt16 Instance => _instance;
        public Int16? ReadFromSpan(ReadOnlySpan<byte> blockSpan, int valueOffset, int valueLength, int flagsOffset)
        {
            return blockSpan[flagsOffset] == 0 ? null : BinaryPrimitives.ReadInt16LittleEndian(blockSpan.Slice(valueOffset, valueLength));
        }

        public void WriteToSpan(Span<byte> blockSpan, int valueOffset, int valueLength, int flagsOffset, Int16? value)
        {
            if (value.HasValue)
            {
                blockSpan[flagsOffset] = (byte)1;
                BinaryPrimitives.WriteInt16LittleEndian(blockSpan.Slice(valueOffset, valueLength), value.Value);
            }
            else
            {
                blockSpan[flagsOffset] = (byte)0;
                blockSpan.Slice(valueOffset, valueLength).Clear();
            }
        }
    }
    internal sealed class SpanReaderWriter_RequiredInt16 : ISpanReaderWriter<Int16>
    {
        private static readonly SpanReaderWriter_RequiredInt16 _instance = new();
        public static SpanReaderWriter_RequiredInt16 Instance => _instance;
        public Int16 ReadFromSpan(ReadOnlySpan<byte> blockSpan, int valueOffset, int valueLength, int flagsOffset)
        {
            return BinaryPrimitives.ReadInt16LittleEndian(blockSpan.Slice(valueOffset, valueLength));
        }

        public void WriteToSpan(Span<byte> blockSpan, int valueOffset, int valueLength, int flagsOffset, Int16 value)
        {
            BinaryPrimitives.WriteInt16LittleEndian(blockSpan.Slice(valueOffset, valueLength), value);
        }
    }
    internal sealed class SpanReaderWriter_OptionalInt32 : ISpanReaderWriter<Int32?>
    {
        private static readonly SpanReaderWriter_OptionalInt32 _instance = new();
        public static SpanReaderWriter_OptionalInt32 Instance => _instance;
        public Int32? ReadFromSpan(ReadOnlySpan<byte> blockSpan, int valueOffset, int valueLength, int flagsOffset)
        {
            return blockSpan[flagsOffset] == 0 ? null : BinaryPrimitives.ReadInt32LittleEndian(blockSpan.Slice(valueOffset, valueLength));
        }

        public void WriteToSpan(Span<byte> blockSpan, int valueOffset, int valueLength, int flagsOffset, Int32? value)
        {
            if (value.HasValue)
            {
                blockSpan[flagsOffset] = (byte)1;
                BinaryPrimitives.WriteInt32LittleEndian(blockSpan.Slice(valueOffset, valueLength), value.Value);
            }
            else
            {
                blockSpan[flagsOffset] = (byte)0;
                blockSpan.Slice(valueOffset, valueLength).Clear();
            }
        }
    }
    internal sealed class SpanReaderWriter_RequiredInt32 : ISpanReaderWriter<Int32>
    {
        private static readonly SpanReaderWriter_RequiredInt32 _instance = new();
        public static SpanReaderWriter_RequiredInt32 Instance => _instance;
        public Int32 ReadFromSpan(ReadOnlySpan<byte> blockSpan, int valueOffset, int valueLength, int flagsOffset)
        {
            return BinaryPrimitives.ReadInt32LittleEndian(blockSpan.Slice(valueOffset, valueLength));
        }

        public void WriteToSpan(Span<byte> blockSpan, int valueOffset, int valueLength, int flagsOffset, Int32 value)
        {
            BinaryPrimitives.WriteInt32LittleEndian(blockSpan.Slice(valueOffset, valueLength), value);
        }
    }
    internal sealed class SpanReaderWriter_OptionalInt64 : ISpanReaderWriter<Int64?>
    {
        private static readonly SpanReaderWriter_OptionalInt64 _instance = new();
        public static SpanReaderWriter_OptionalInt64 Instance => _instance;
        public Int64? ReadFromSpan(ReadOnlySpan<byte> blockSpan, int valueOffset, int valueLength, int flagsOffset)
        {
            return blockSpan[flagsOffset] == 0 ? null : BinaryPrimitives.ReadInt64LittleEndian(blockSpan.Slice(valueOffset, valueLength));
        }

        public void WriteToSpan(Span<byte> blockSpan, int valueOffset, int valueLength, int flagsOffset, Int64? value)
        {
            if (value.HasValue)
            {
                blockSpan[flagsOffset] = (byte)1;
                BinaryPrimitives.WriteInt64LittleEndian(blockSpan.Slice(valueOffset, valueLength), value.Value);
            }
            else
            {
                blockSpan[flagsOffset] = (byte)0;
                blockSpan.Slice(valueOffset, valueLength).Clear();
            }
        }
    }
    internal sealed class SpanReaderWriter_RequiredInt64 : ISpanReaderWriter<Int64>
    {
        private static readonly SpanReaderWriter_RequiredInt64 _instance = new();
        public static SpanReaderWriter_RequiredInt64 Instance => _instance;
        public Int64 ReadFromSpan(ReadOnlySpan<byte> blockSpan, int valueOffset, int valueLength, int flagsOffset)
        {
            return BinaryPrimitives.ReadInt64LittleEndian(blockSpan.Slice(valueOffset, valueLength));
        }

        public void WriteToSpan(Span<byte> blockSpan, int valueOffset, int valueLength, int flagsOffset, Int64 value)
        {
            BinaryPrimitives.WriteInt64LittleEndian(blockSpan.Slice(valueOffset, valueLength), value);
        }
    }
    internal static class CodecFactory
    {
        public static ISpanReaderWriter<T> GetCodec<T>()
        {
            if (typeof(T) == typeof(Boolean)) return (ISpanReaderWriter<T>)(ISpanReaderWriter<Boolean>)SpanReaderWriter_RequiredBoolean.Instance;
            if (typeof(T) == typeof(Boolean?)) return (ISpanReaderWriter<T>)(ISpanReaderWriter<Boolean?>)SpanReaderWriter_OptionalBoolean.Instance;
            if (typeof(T) == typeof(SByte)) return (ISpanReaderWriter<T>)(ISpanReaderWriter<SByte>)SpanReaderWriter_RequiredSByte.Instance;
            if (typeof(T) == typeof(SByte?)) return (ISpanReaderWriter<T>)(ISpanReaderWriter<SByte?>)SpanReaderWriter_OptionalSByte.Instance;
            if (typeof(T) == typeof(Int16)) return (ISpanReaderWriter<T>)(ISpanReaderWriter<Int16>)SpanReaderWriter_RequiredInt16.Instance;
            if (typeof(T) == typeof(Int16?)) return (ISpanReaderWriter<T>)(ISpanReaderWriter<Int16?>)SpanReaderWriter_OptionalInt16.Instance;
            if (typeof(T) == typeof(Int32)) return (ISpanReaderWriter<T>)(ISpanReaderWriter<Int32>)SpanReaderWriter_RequiredInt32.Instance;
            if (typeof(T) == typeof(Int32?)) return (ISpanReaderWriter<T>)(ISpanReaderWriter<Int32?>)SpanReaderWriter_OptionalInt32.Instance;
            if (typeof(T) == typeof(Int64)) return (ISpanReaderWriter<T>)(ISpanReaderWriter<Int64>)SpanReaderWriter_RequiredInt64.Instance;
            if (typeof(T) == typeof(Int64?)) return (ISpanReaderWriter<T>)(ISpanReaderWriter<Int64?>)SpanReaderWriter_OptionalInt64.Instance;
            if (typeof(T) == typeof(Byte)) return (ISpanReaderWriter<T>)(ISpanReaderWriter<Byte>)SpanReaderWriter_RequiredByte.Instance;
            if (typeof(T) == typeof(Byte?)) return (ISpanReaderWriter<T>)(ISpanReaderWriter<Byte?>)SpanReaderWriter_OptionalByte.Instance;
            throw new NotImplementedException($"Unsupported type: {typeof(T).FullName}"); // todo remove this
            throw new NotSupportedException($"Unsupported type: {typeof(T).FullName}");
        }
    }
    public abstract class FieldDefnBase
    {
        public int ValueOffset { get; }
        public int ValueLength { get; }
        public int FlagsOffset { get; }
        protected FieldDefnBase(int valueOffset, int valueLength, int flagsOffset)
        {
            ValueOffset = valueOffset;
            ValueLength = valueLength;
            FlagsOffset = flagsOffset;
        }
    }
    public sealed class FieldDefn<T> : FieldDefnBase
    {
        private readonly ISpanReaderWriter<T> _codec;
        private FieldDefn(int valueOffset, int valueLength, int flagsOffset, ISpanReaderWriter<T> codec)
            : base(valueOffset, valueLength, flagsOffset)
        {
            _codec = codec;
        }
        public FieldDefn(int valueOffset, int valueLength, int flagsOffset)
            : this(valueOffset, valueLength, flagsOffset, CodecFactory.GetCodec<T>())
        {
        }
        public FieldDefn<T> WithValueOffset(int offset) => new FieldDefn<T>(offset, ValueLength, FlagsOffset, _codec);
        public T ReadFromSpan(ReadOnlySpan<byte> blockSpan)
        {
            return _codec.ReadFromSpan(blockSpan, ValueOffset, ValueLength, FlagsOffset);
        }
        public void WriteToSpan(Span<byte> blockSpan, T value)
        {
            _codec.WriteToSpan(blockSpan, ValueOffset, ValueLength, FlagsOffset, value);
        }
    }
    public sealed class BlockMap
    {
        public static bool Vacant(BitArray map, int position, int length)
        {
            for (int i = position; i < (position + length); i++)
            {
                if (map[i]) return false;
            }

            return true;
        }

        public static void Reserve(BitArray map, int position, int length)
        {
            for (int i = position; i < position + length; i++)
            {
                if (map[i]) throw new InvalidOperationException($"Cannot reserve map[{position}..{position + length - 1}] - map[{i}] is already reserved");
                map[i] = true;
            }
        }

        private readonly int _blockSize;
        private readonly BinaryMap _map;
        private readonly ImmutableArray<FieldDefnBase> _fields;

        public int BlockSize => _blockSize;
        public ImmutableArray<FieldDefnBase> Fields => _fields;
        public FieldDefnBase LastAdded => _fields[_fields.Length - 1];


        private BlockMap(int blockSize, BinaryMap map, ImmutableArray<FieldDefnBase> fields)
        {
            _blockSize = blockSize;
            _map = map;
            _fields = fields;
        }

        private static readonly BlockMap _empty = new BlockMap(0, BinaryMap.Empty, ImmutableArray<FieldDefnBase>.Empty);
        public static BlockMap Empty => _empty;

        private string? GetFirstValidationError()
        {
            var map = new BitArray(BlockSize);
            foreach (FieldDefnBase field in Fields)
            {
                // check value alignment
                if (field.ValueOffset % field.ValueLength != 0)
                    return $"Value incorrectly aligned (Offset={field.ValueOffset},Length={field.ValueLength})";

                // check memory overlaps
                if (Vacant(map, field.ValueOffset, field.ValueLength))
                {
                    Reserve(map, field.ValueOffset, field.ValueLength);
                }
                else
                {
                    return $"Value incorrectly mapped (Offset={field.ValueOffset},Length={field.ValueLength})";
                }

                // check flags byte
                if (field.FlagsOffset >= 0)
                {
                    // check memory overlaps
                    if (Vacant(map, field.FlagsOffset, 1))
                    {
                        Reserve(map, field.FlagsOffset, 1);
                    }
                    else
                    {
                        return $"Flags incorrectly mapped (Offset={field.FlagsOffset})";
                    }
                }
            }

            return null;
        }

        public bool IsValid(bool throwOnError = false)
        {
            string? reason = GetFirstValidationError();
            if (reason is null)
                return true;
            if (throwOnError)
                throw new InvalidOperationException(reason);
            return false;
        }

        /// <summary>
        /// Valid fixed field sizes are: 8, 16, 32, 64, 128 and 256
        /// </summary>
        /// <param name="stringSize"></param>
        /// <returns></returns>
        private static bool IsValidFixedSize(int fixedSize)
        {
            return fixedSize switch
            {
                8 => true,
                16 => true,
                32 => true,
                64 => true,
                128 => true,
                256 => true,
                _ => false
            };
        }

        private static (bool , int ) GetFieldLength<T>(int? fixedSize = null)
        {
            // todo: Half, U/Int128

            // check for Nullable<T>
            if (typeof(T) == typeof(byte?)) return (true, 1);
            if (typeof(T) == typeof(sbyte?)) return (true, 1);
            if (typeof(T) == typeof(bool?)) return (true, 1);

            if (typeof(T) == typeof(short?)) return (true, 2);
            if (typeof(T) == typeof(ushort?)) return (true, 2);
            if (typeof(T) == typeof(char?)) return (true, 2);

            if (typeof(T) == typeof(int?)) return (true, 4);
            if (typeof(T) == typeof(uint?)) return (true, 4);
            if (typeof(T) == typeof(float?)) return (true, 4);

            if (typeof(T) == typeof(long?)) return (true, 8);
            if (typeof(T) == typeof(ulong?)) return (true, 8);
            if (typeof(T) == typeof(double?)) return (true, 8);

            if (typeof(T) == typeof(Guid?)) return (true, 16);

            // check for value types
            if (typeof(T) == typeof(byte)) return (false, 1);
            if (typeof(T) == typeof(sbyte)) return (false, 1);
            if (typeof(T) == typeof(bool)) return (false, 1);

            if (typeof(T) == typeof(short)) return (false, 2);
            if (typeof(T) == typeof(ushort)) return (false, 2);
            if (typeof(T) == typeof(char)) return (false, 2);

            if (typeof(T) == typeof(int)) return (false, 4);
            if (typeof(T) == typeof(uint)) return (false, 4);
            if (typeof(T) == typeof(float)) return (false, 4);

            if (typeof(T) == typeof(long)) return (false, 8);
            if (typeof(T) == typeof(ulong)) return (false, 8);
            if (typeof(T) == typeof(double)) return (false, 8);

            if (typeof(T) == typeof(Guid)) return (false, 16);

            // check for reference types
            if (typeof(T) == typeof(string))
            {
                if (fixedSize.HasValue && IsValidFixedSize(fixedSize.Value))
                {
                    return (false, fixedSize.Value);
                }
                else
                    return (false, 64);
            }
            if (typeof(T) == typeof(Octets))
            {
                if (fixedSize.HasValue && IsValidFixedSize(fixedSize.Value))
                {
                    return (false, fixedSize.Value);
                }
                else
                    return (false, 64);
            }

            // todo don't throw - generate error messages
            throw new InvalidOperationException($"Cannot determine length of type: {typeof(T).FullName}");
        }

        public BlockMap AddField<T>(int? sizeInBytes = null)
        {
            (bool isNullable, int valueLength) = GetFieldLength<T>(sizeInBytes);

            BinaryMap newMap = _map;
            int flagsOffset = -1;
            if (isNullable)
            {
                // assign a field for the flags byte
                (newMap, int bitOffset) = newMap.AssignField(1 * 8);
                var bitsRemain = bitOffset % 8;
                if (bitsRemain != 0)
                    throw new InvalidOperationException($"Can't do bit offsets yet");

                if (newMap.Sizes.BitsRemain != 0)
                    throw new InvalidOperationException($"Cannot build maps with bit remainders (yet)");

                flagsOffset = bitOffset / 8;
            }

            // assign a field for the value
            int valueOffset;
            {
                (newMap, int bitOffset) = newMap.AssignField(valueLength * 8);
                var bitsRemain = bitOffset % 8;
                if (bitsRemain != 0)
                    throw new InvalidOperationException($"Can't do bit offsets yet");

                if (newMap.Sizes.BitsRemain != 0)
                    throw new InvalidOperationException($"Cannot build maps with bit remainders (yet)");

                valueOffset = bitOffset / 8;
            }

            var fieldDef = new FieldDefn<T>(valueOffset, valueLength, flagsOffset);
            var newFields = _fields.Add(fieldDef);
            return new BlockMap(newMap.Sizes.ByteCount, newMap, newFields);
        }
    }
}

