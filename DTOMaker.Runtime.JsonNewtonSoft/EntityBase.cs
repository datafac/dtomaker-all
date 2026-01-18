using DTOMaker.Models;
using Newtonsoft.Json;
using System;
using System.Runtime.CompilerServices;

namespace DTOMaker.Runtime.JsonNewtonSoft
{
    /// <summary>
    /// Base entity implementation for JSON.NET based DTOs.
    /// </summary>
    public abstract class EntityBase : IEntityBase, IEquatable<EntityBase>
    {
        /// <summary>
        /// Returns the unique (model-wide) identifier for the current entity.
        /// </summary>
        protected abstract int OnGetEntityId();

        /// <summary>
        /// Gets the unique (model-wide) identifier for the current entity.
        /// </summary>
        /// <returns>An integer value that uniquely identifies the entity within the context of the source model.</returns>
        public int GetEntityId() => OnGetEntityId();

        /// <summary>
        /// Initializes a new instance of the EntityBase class.
        /// </summary>
        public EntityBase() { }

        /// <summary>
        /// Initializes a new instance of the EntityBase class using the specified entity base instance.
        /// </summary>
        /// <param name="_">An instance of an object that implements the IEntityBase interface to initialize the new EntityBase
        /// instance.</param>
        public EntityBase(IEntityBase _) { }

        /// <summary>
        /// Initializes a new instance of the EntityBase class by copying values from an existing instance.
        /// </summary>
        /// <param name="_">The EntityBase instance to copy values from. Cannot be null.</param>
        public EntityBase(EntityBase _) { }

        private volatile bool _frozen;

        /// <summary>
        /// Gets a value indicating whether the object is in a frozen (read-only) state.
        /// </summary>
        [JsonIgnore] public bool IsFrozen => _frozen;

        /// <summary>
        /// Performs actions when the object is being frozen.
        /// </summary>
        protected virtual void OnFreeze() { }

        /// <summary>
        /// Prevents further modifications to the current object by marking it as frozen.
        /// </summary>
        /// <remarks>After calling this method, attempts to modify the object's state will result in 
        /// exceptions. Calling this method multiple times has no additional effect.</remarks>
        public void Freeze()
        {
            if (_frozen) return;
            OnFreeze();
            _frozen = true;
        }

        /// <summary>
        /// Creates an unfrozen shallow copy of the current object.
        /// </summary>
        protected abstract IEntityBase OnPartCopy();

        /// <summary>
        /// Creates an unfrozen shallow copy of the current object.
        /// </summary>
        public IEntityBase PartCopy() => OnPartCopy();

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void ThrowIsFrozenException(string? methodName) => throw new InvalidOperationException($"Cannot set {methodName} when frozen.");

        /// <summary>
        /// Ensures that the object is not frozen before allowing further modification or access.
        /// </summary>
        /// <remarks>If the object is frozen, this method throws an exception to prevent further
        /// modifications. This method is typically used in property setters or mutating methods to enforce immutability
        /// after the object has been frozen.</remarks>
        /// <typeparam name="T">The type of the value to be returned if the object is not frozen.</typeparam>
        /// <param name="value">The value to return if the object is not frozen.</param>
        /// <param name="methodName">The name of the calling method. This parameter is optional and is automatically provided by the compiler.</param>
        /// <returns>The specified value if the object is not frozen.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected T IfNotFrozen<T>(T value, [CallerMemberName] string? methodName = null)
        {
            if (_frozen) ThrowIsFrozenException(methodName);
            return value;
        }

        /// <summary>
        /// Determines whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the other parameter; otherwise, false.</returns>
        public bool Equals(EntityBase? other) => true;

        /// <summary>
        /// Determines whether the specified object is an instance of the EntityBase type.
        /// </summary>
        /// <param name="obj">The object to compare with the current instance.</param>
        /// <returns>true if the specified object is an instance of EntityBase; otherwise, false.</returns>
        public override bool Equals(object? obj) => obj is EntityBase;

        /// <summary>
        /// Serves as the default hash function for the EntityBase type.
        /// </summary>
        /// <remarks>This implementation returns a hash code based solely on the EntityBase type, and does
        /// not consider instance data. As a result, all instances of EntityBase will return the same hash
        /// code.</remarks>
        /// <returns>A hash code for the current EntityBase type.</returns>
        public override int GetHashCode() => HashCode.Combine<Type>(typeof(EntityBase));

        /// <summary>
        /// Determines whether two byte arrays contain the same sequence of values.
        /// </summary>
        /// <param name="left">The first byte array to compare, or null.</param>
        /// <param name="right">The second byte array to compare, or null.</param>
        /// <returns>true if both arrays are null or if they contain the same sequence of bytes; otherwise, false.</returns>
        protected static bool BinaryValuesAreEqual(byte[]? left, byte[]? right)
        {
            if (left is null) return (right is null);
            if (right is null) return false;
            return left.AsSpan().SequenceEqual(right.AsSpan());
        }

    }
}
