using DataFac.Memory;
using DataFac.Storage;
using DataFac.UnsafeHelpers;
using DTOMaker.Models;
using System;
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
}

namespace Testing.Models.NetStrux
{
    public sealed partial class Required_Int64 : DTOMaker.Runtime.NetStrux.EntityBase, IRequired_Int64, IEquatable<Required_Int64>
    {
        private BlockB008 _block = default;
        public new static readonly int BlockOffset = DTOMaker.Runtime.NetStrux.EntityBase.TotalLength;
        public new static readonly int BlockLength = 8; // todo _block.StaticSize
        public new static readonly int TotalLength = BlockOffset + BlockLength;

        public Int64 Field
        {
            get { return _block.Int64ValueLE; }
            set { _block.Int64ValueLE = IfNotFrozen(value); }
        }

        public Required_Int64() : base(TotalLength) { }
        protected Required_Int64(int outerLength) : base(outerLength) { }
        public Required_Int64(ReadOnlySpan<byte> source) : base(TotalLength, source)
        {
            _block.TryRead(source.Slice(BlockOffset, BlockLength));
        }
        protected Required_Int64(int outerLength, ReadOnlySpan<byte> source) : base(outerLength, source)
        {
            _block.TryRead(source.Slice(BlockOffset, BlockLength));
        }

        protected override void OnSerialize(Span<byte> target)
        {
            base.OnSerialize(target);
            _block.WriteTo(target.Slice(BlockOffset, BlockLength));
        }

        public bool Equals(Required_Int64? other)
        {
            if (ReferenceEquals(this, other)) return true;
            if (other is null) return false;
            if (!base.Equals(other)) return false;
            if (other.Field != this.Field) return false;
            return true;
        }

        public override bool Equals(object? obj) => obj is Required_Int64 other && Equals(other);
        public override int GetHashCode()
        {
            HashCode hasher = new();
            hasher.Add(base.GetHashCode());
            hasher.Add(typeof(Required_Int64));
            hasher.Add(Field);
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
