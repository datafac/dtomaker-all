using DataFac.Memory;
using MessagePack;
using System;
using System.Runtime.CompilerServices;

namespace DTOMaker.Runtime.MsgPack2
{
    [MessagePackObject]
    public abstract class EntityBase : IEntityBase, IEquatable<EntityBase>
    {
        public const int EntityId = 0;
        protected abstract int OnGetEntityId();
        public int GetEntityId() => OnGetEntityId();

        public EntityBase() { }
        public EntityBase(IEntityBase notUsed) { }
        public EntityBase(EntityBase notUsed) { }

        [IgnoreMember]
        private volatile bool _frozen;
        [IgnoreMember]
        public bool IsFrozen => _frozen;
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

        protected static bool BinaryValuesAreEqual(Octets? left, Octets? right)
        {
            if (left is null) return (right is null);
            if (right is null) return false;
            return left.Equals(right);
        }

    }
}
