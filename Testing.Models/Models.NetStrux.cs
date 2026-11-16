using DataFac.Memory;
using DataFac.Storage;
using DataFac.UnsafeHelpers;
using DTOMaker.Models;
using DTOMaker.Runtime.NetStrux;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
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

        // todo? reserve header
        private BlockB008 _block = default;
        public static readonly int BlockOffset = 0;
        public static readonly int BlockLength = 8; // todo _block.StaticSize
        public static readonly int TotalLength = BlockOffset + BlockLength;

        private readonly int _outerLength;
        protected EntityBase(int outerLength) => _outerLength = outerLength;
        protected EntityBase(int outerLength, ReadOnlySpan<byte> source) => _outerLength = outerLength;

        /// <summary>
        /// When implemented in a derived class, serializes the entity's data into a byte array.
        /// </summary>
        /// <returns></returns>
        protected virtual void OnSerialize(Span<byte> target) { }

        /// <inheritdoc/>
        public ValueTask<ReadOnlyMemory<byte>> Serialize(CancellationToken cancellation)
        {
            ThrowIfNotPacked();
            Memory<byte> target = new byte[_outerLength];
            OnSerialize(target.Span);
            return new ValueTask<ReadOnlyMemory<byte>>(target);
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

        protected virtual IEntityBase OnShallowCopy() => throw new NotImplementedException();
        public IEntityBase ShallowCopy() => OnShallowCopy();

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
    public sealed class FieldDef
    {
        public int Offset { get; }
        public int Length { get; }
        public FieldDef(int offset, int length)
        {
            Offset = offset;
            Length = length;
        }
        public FieldDef WithOffset(int offset) => new FieldDef(offset, Length);
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
        private readonly ImmutableArray<FieldDef> _fields;

        public int BlockSize => _blockSize;
        public ImmutableArray<FieldDef> Fields => _fields;
        public FieldDef LastAdded => Fields[^1];


        private BlockMap(int blockSize, BinaryMap map, ImmutableArray<FieldDef> fields)
        {
            _blockSize = blockSize;
            _map = map;
            _fields = fields;
        }

        private static readonly BlockMap _empty = new BlockMap(0, BinaryMap.Empty, ImmutableArray<FieldDef>.Empty);
        public static BlockMap Empty => _empty;

        private string? GetFirstValidationError()
        {
            var map = new BitArray(BlockSize);
            foreach (FieldDef field in Fields)
            {
                // check alignment
                if (field.Offset % field.Length != 0)
                    return
                        $"Field incorrectly aligned (Offset={field.Offset},Length={field.Length})";

                // check memory overlaps
                var bytesNeeded = field.Length;
                if (Vacant(map, field.Offset, bytesNeeded))
                {
                    Reserve(map, field.Offset, bytesNeeded);
                }
                else
                {
                    return $"Field incorrectly mapped (Offset={field.Offset},Length={field.Length})";
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

        private static int GetFieldLength<T>()
        {
            if (typeof(T) == typeof(byte)) return 1;
            if (typeof(T) == typeof(sbyte)) return 1;
            if (typeof(T) == typeof(bool)) return 1;

            if (typeof(T) == typeof(short)) return 2;
            if (typeof(T) == typeof(ushort)) return 2;
            if (typeof(T) == typeof(char)) return 2;

            if (typeof(T) == typeof(int)) return 4;
            if (typeof(T) == typeof(uint)) return 4;
            if (typeof(T) == typeof(float)) return 4;

            if (typeof(T) == typeof(long)) return 8;
            if (typeof(T) == typeof(ulong)) return 8;
            if (typeof(T) == typeof(double)) return 8;

            if (typeof(T) == typeof(Guid)) return 16;

            if (typeof(T) == typeof(string)) return 64;
            if (typeof(T) == typeof(Octets)) return 64;

            throw new InvalidOperationException($"Cannot determine length of type: {typeof(T).FullName}");
        }

        public BlockMap AddField<T>()
        {
            int fieldLength = GetFieldLength<T>();

            int lengthInBits = fieldLength * 8;
            (BinaryMap newMap, int bitOffset) = _map.AssignField(lengthInBits);
            var bitsRemain = bitOffset % 8;
            if (bitsRemain != 0)
                throw new InvalidOperationException($"Can't do bit offsets yet");

            if (newMap.Sizes.BitsRemain != 0)
                throw new InvalidOperationException($"Cannot build maps with bit remainders (yet)");

            int fieldOffset = bitOffset / 8;
            var fieldDef = new FieldDef(fieldOffset, fieldLength);
            var newFields = _fields.Add(fieldDef);
            return new BlockMap(newMap.Sizes.ByteCount, newMap, newFields);
        }
    }
}

namespace Testing.Models.NetStrux
{
    public sealed partial class Simple : DTOMaker.Runtime.NetStrux.EntityBase, ISimple, IEquatable<Simple>
    {
        private BlockB032 _block = default;
        public new static readonly int BlockOffset = DTOMaker.Runtime.NetStrux.EntityBase.TotalLength;
        public new static readonly int BlockLength = 32; // todo _block.StaticSize
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

        // -------------------- Field001 ----------------------------------------
        private static readonly BlockMap Field999_Map = Field002_Map;

        //private static readonly int DeclaredBlockSize = _block.BlockSize;
        private static readonly int ComputedBlockSize = Field999_Map.BlockSize;

        // ==================== fields end ========================================

        public Simple() : base(TotalLength) { }
        protected Simple(int outerLength) : base(outerLength) { }
        public Simple(ReadOnlySpan<byte> source) : base(TotalLength, source)
        {
            _block.TryRead(source.Slice(BlockOffset, BlockLength));
        }
        protected Simple(int outerLength, ReadOnlySpan<byte> source) : base(outerLength, source)
        {
            _block.TryRead(source.Slice(BlockOffset, BlockLength));
        }

        protected override void OnSerialize(Span<byte> target)
        {
            base.OnSerialize(target);
            _block.WriteTo(target.Slice(BlockOffset, BlockLength));
        }

        public bool Equals(Simple? other)
        {
            if (ReferenceEquals(this, other)) return true;
            if (other is null) return false;
            if (!base.Equals(other)) return false;
            if (other.Field1 != this.Field1) return false;
            if (other.Field2 != this.Field2) return false;
            return true;
        }

        public override bool Equals(object? obj) => obj is Simple other && Equals(other);
        public override int GetHashCode()
        {
            HashCode hasher = new();
            hasher.Add(base.GetHashCode());
            hasher.Add(typeof(Simple));
            hasher.Add(Field1);
            hasher.Add(Field2);
            return hasher.ToHashCode();
        }
    }

    public sealed partial class Required_String : DTOMaker.Runtime.NetStrux.EntityBase, IRequired_String, IEquatable<Required_String>
    {
        private BlockB064 _block = default;
        public new static readonly int BlockOffset = DTOMaker.Runtime.NetStrux.EntityBase.TotalLength;
        public new static readonly int BlockLength = 64; // todo _block.StaticSize
        public new static readonly int TotalLength = BlockOffset + BlockLength;

        public String Field
        {
            get
            {
                var span = BlockHelper.AsReadOnlySpan(ref _block);
                var lengthInBytes = span[0];
                return Encoding.UTF8.GetString(span.Slice(1, lengthInBytes));
            }
            set
            {
                ThrowIfFrozen();
                var span = BlockHelper.AsWritableSpan(ref _block);
                if (Encoding.UTF8.TryGetBytes(value, span.Slice(1, BlockLength - 1), out int lengthInBytes))
                {
                    span[0] = (byte)lengthInBytes;
                }
                else
                {
                    throw new ArgumentException($"String value is too long to fit in {BlockLength - 1} bytes.");
                }
            }
        }

        public Required_String() : base(TotalLength) { }
        protected Required_String(int outerLength) : base(outerLength) { }
        public Required_String(ReadOnlySpan<byte> source) : base(TotalLength, source)
        {
            _block.TryRead(source.Slice(BlockOffset, BlockLength));
        }
        protected Required_String(int outerLength, ReadOnlySpan<byte> source) : base(outerLength, source)
        {
            _block.TryRead(source.Slice(BlockOffset, BlockLength));
        }

        protected override void OnSerialize(Span<byte> target)
        {
            base.OnSerialize(target);
            _block.WriteTo(target.Slice(BlockOffset, BlockLength));
        }

        public bool Equals(Required_String? other)
        {
            if (ReferenceEquals(this, other)) return true;
            if (other is null) return false;
            if (!base.Equals(other)) return false;
            if (other.Field != this.Field) return false;
            return true;
        }

        public override bool Equals(object? obj) => obj is Required_String other && Equals(other);
        public override int GetHashCode()
        {
            HashCode hasher = new();
            hasher.Add(base.GetHashCode());
            hasher.Add(typeof(Required_String));
            hasher.Add(Field);
            return hasher.ToHashCode();
        }
    }
}
