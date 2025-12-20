using System;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace DTOMaker.Runtime.JsonSystemText
{
    public abstract class EntityBase : IEntityBase, IEquatable<EntityBase>
    {
        protected abstract int OnGetEntityId();
        public int GetEntityId() => OnGetEntityId();

        public EntityBase() { }
        public EntityBase(IEntityBase _) { }
        public EntityBase(EntityBase _) { }

        private volatile bool _frozen;
        [JsonIgnore] public bool IsFrozen => _frozen;
        protected virtual void OnFreeze() { }
        public void Freeze()
        {
            if (_frozen) return;
            OnFreeze();
            _frozen = true;
        }
        protected abstract IEntityBase OnPartCopy();
        public IEntityBase PartCopy() => OnPartCopy();

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void ThrowIsFrozenException(string? methodName) => throw new InvalidOperationException($"Cannot set {methodName} when frozen.");

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected T IfNotFrozen<T>(T value, [CallerMemberName] string? methodName = null)
        {
            if (_frozen) ThrowIsFrozenException(methodName);
            return value;
        }

        public bool Equals(EntityBase? other) => true;
        public override bool Equals(object? obj) => obj is EntityBase;
        public override int GetHashCode() => HashCode.Combine<Type>(typeof(EntityBase));

        protected static bool BinaryValuesAreEqual(byte[]? left, byte[]? right)
        {
            if (left is null) return (right is null);
            if (right is null) return false;
            return left.AsSpan().SequenceEqual(right.AsSpan());
        }

    }
}
