using DataFac.Memory;
using DataFac.Storage;
using DTOMaker.Models;
using MessagePack;
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace DTOMaker.Runtime.MsgPack2
{
    /// <summary>
    /// Provides a base class for MsgPack2 generated entities, supporting identity, equality comparison, 
    /// and a frozen state to prevent further modification.
    /// </summary>
    public abstract class EntityBase : IPackable, IEquatable<EntityBase>
    {
        /// <summary>
        /// Represents the default entity identifier value.
        /// </summary>
        public const int EntityId = 0;
        /// <summary>
        /// When implemented in a derived class, retrieves the unique identifier for the associated entity.
        /// </summary>
        protected abstract int OnGetEntityId();
        /// <summary>
        /// Retrieves the unique identifier for the current entity.
        /// </summary>
        public int GetEntityId() => OnGetEntityId();

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public EntityBase() { }
        /// <summary>
        /// Initializes a new instance of the class by copying values from any instance implementing
        /// the entity interface.
        /// </summary>
        public EntityBase(IEntityBase _) { }
        /// <summary>
        /// Initializes a new instance of the class by copying values from an existing instance.
        /// </summary>
        public EntityBase(EntityBase _) { }

        [IgnoreMember]
        private volatile bool _frozen;
        /// <summary>
        /// Gets a value indicating whether the entity is in a frozen (immutable) state.
        /// </summary>
        [IgnoreMember]
        public bool IsFrozen => _frozen;
        /// <summary>
        /// Provides a callback method that is invoked when the entity is being frozen.
        /// </summary>
        protected virtual void OnFreeze() { }
        /// <summary>
        /// Prevents further modifications by marking the entity as frozen.
        /// </summary>
        public void Freeze()
        {
            if (_frozen) return;
            OnFreeze();
            _frozen = true;
        }
        /// <summary>
        /// Creates a shallow copy of the entity.
        /// </summary>
        protected abstract IEntityBase OnShallowCopy();
        /// <summary>
        /// Creates a shallow copy of the entity.
        /// </summary>
        public IEntityBase ShallowCopy() => OnShallowCopy();

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void ThrowIsFrozenException(string? methodName) => throw new InvalidOperationException($"Cannot set {methodName} when frozen.");

        /// <summary>
        /// Ensures that the entity is not frozen before allowing modification.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected T IfNotFrozen<T>(T value, [CallerMemberName] string? methodName = null)
        {
            if (_frozen) ThrowIsFrozenException(methodName);
            return value;
        }

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

        #region IPackable implementation
        [MethodImpl(MethodImplOptions.NoInlining)]
        private void ThrowIsNotPackedException(string? methodName) => throw new InvalidOperationException($"Cannot call {methodName} when not packed.");

        /// <summary>
        /// Ensures that the entity is packed and throws an exception if it is not.
        /// </summary>
        /// <param name="methodName"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void ThrowIfNotPacked([CallerMemberName] string? methodName = null)
        {
            if (!_packed) ThrowIsNotPackedException(methodName);
        }

        /// <summary>
        /// When implemented in a derived class, serializes the entity's data into a byte array.
        /// </summary>
        /// <returns></returns>
        protected virtual ReadOnlyMemory<byte> OnSerialize() => ReadOnlyMemory<byte>.Empty;
        /// <inheritdoc/>
        public ReadOnlyMemory<byte> Serialize(CancellationToken cancellation)
        {
            ThrowIfNotPacked();
            return OnSerialize();
        }

        private volatile bool _packed;
        /// <inheritdoc/>
        [IgnoreMember]
        public bool IsPacked => _packed;
        protected virtual ValueTask OnPack(IDataStore dataStore, CancellationToken cancellation) => default;
        public async ValueTask Pack(IDataStore dataStore, CancellationToken cancellation)
        {
            if (_frozen) return;
            if (_packed) return;
            await OnPack(dataStore, cancellation);
            _packed = true;
            OnFreeze();
            _frozen = true;
            _unpacked = true;
        }

        private volatile bool _unpacked;
        /// <inheritdoc/>
        [IgnoreMember]
        public bool IsUnpacked => _unpacked;
        protected virtual ValueTask OnUnpack(IDataStore dataStore, int depth, CancellationToken cancellation) => default;
        public async ValueTask Unpack(IDataStore dataStore, int depth, CancellationToken cancellation)
        {
            ThrowIfNotPacked();
            if (depth < 0) return;
            if (_unpacked) return;
            await OnUnpack(dataStore, depth, cancellation);
            _unpacked = true;
        }
        public ValueTask UnpackAll(IDataStore dataStore, CancellationToken cancellation) => Unpack(dataStore, int.MaxValue, cancellation);
        #endregion
    }
}
