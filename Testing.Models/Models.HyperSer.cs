using DataFac.Memory;
using DataFac.Storage;
using DTOMaker.Models;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DTOMaker.Runtime.HyperSer
{
    public abstract class EntityBase : IEntityBase, IPackable, IEquatable<EntityBase>
    {
        [MethodImpl(MethodImplOptions.NoInlining)] private static void ThrowIsFrozenException(string? methodName) => throw new InvalidOperationException($"Cannot set {methodName} when frozen.");
        [MethodImpl(MethodImplOptions.NoInlining)] private static void ThrowIsNotFrozenException(string? methodName) => throw new InvalidOperationException($"Cannot call {methodName} when not frozen.");
        [MethodImpl(MethodImplOptions.NoInlining)] private static void ThrowIsNotPackedException(string? methodName) => throw new InvalidOperationException($"Cannot call {methodName} when not packed.");

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void ThrowIfNotFrozen([CallerMemberName] string? methodName = null)
        {
            if (!_frozen) EntityBase.ThrowIsNotFrozenException(methodName);
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

        public bool Equals(EntityBase? other) => true;
        public override bool Equals(object? obj) => obj is EntityBase;
        public override int GetHashCode() => HashCode.Combine<Type>(typeof(EntityBase));

        public ValueTask<ReadOnlyMemory<byte>> Serialize(CancellationToken cancellation)
        {
            throw new NotImplementedException();
        }
    }
}

namespace Testing.Models.HyperSer
{
    public sealed partial class Simple : DTOMaker.Runtime.HyperSer.EntityBase, ISimple, IEquatable<Simple>
    {
        public Int64 Field1
        {
            get;
            set { field = IfNotFrozen(value); }
        }

        public Int64? Field2
        {
            get;
            set { field = IfNotFrozen(value); }
        }

        public Simple() : base() { }

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

    public sealed partial class Required_String: DTOMaker.Runtime.HyperSer.EntityBase, IRequired_String, IEquatable<Required_String>
    {
        public String Field
        {
            get;
            set { field = IfNotFrozen(value); }
        } = string.Empty;

        public Required_String() : base() { }

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
