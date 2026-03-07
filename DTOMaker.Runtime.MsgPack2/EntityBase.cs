using DataFac.Memory;
using DTOMaker.Models;
using MessagePack;
using System;
using System.Runtime.CompilerServices;

namespace DTOMaker.Runtime.MsgPack2
{
    /// <summary>
    /// Provides a base class for MsgPack2 generated entities, supporting identity, equality comparison, 
    /// and a frozen state to prevent further modification.
    /// </summary>
    public abstract class EntityBase : IEntityBase, IEquatable<EntityBase>
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
        protected abstract IEntityBase OnPartCopy();
        /// <summary>
        /// Creates a shallow copy of the entity.
        /// </summary>
        public IEntityBase PartCopy() => OnPartCopy();

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

    }
}
