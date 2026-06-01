using DataFac.Storage;
using DTOMaker.Models;
using DTOMaker.Runtime;
using MessagePack;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

// Todo:
// - Support records with init-only properties
// - do we need a builder pattern?
// - Support MessagePack 3.x

#if NET6_0_OR_GREATER
#else
namespace System.Runtime.CompilerServices
{
    /// <summary>
    /// Adding this fixes CS0518 errors.
    /// </summary>
    internal static class IsExternalInit { }
}
#endif

namespace DTOMaker.Runtime
{
}

namespace NewModels
{
    [Entity(1)]
    public interface IDomainBase : IEntityBase
    {
    }

    [Entity(2)]
    public interface IVarSet : IDomainBase
    {
        [Member(1)][Name("set")] IVarSetNode? Root { get; }
    }

    [Entity(3)]
    public interface IVarSetNode : IDomainBase
    {
        [Member(1)][Name("N")] int Count { get; }
        [Member(2)][Name("D")] byte Depth { get; }
        [Member(3)][Name("key")] string Key { get; }
        [Member(4)][Name("val")] IVarBase Value { get; }
        [Member(5)][Name("L")] IVarSetNode? Left { get; }
        [Member(6)][Name("R")] IVarSetNode? Right { get; }
    }

    [Entity(4)]
    public interface IVarBase : IDomainBase
    {
    }

    [Entity(5)]
    public interface IVarBoolean : IVarBase
    {
        [Member(1)][Name("val")] Boolean Value { get; }
    }

    [Entity(6)]
    public interface IVarString : IVarBase
    {
        [Member(1)][Name("val")] String Value { get; }
    }

    [Entity(7)]
    public interface IVarInt64 : IVarBase
    {
        [Member(1)][Name("val")] Int64 Value { get; }
    }
}

// <generated>
// in Domain.g.cs
namespace NewModels
{
    public interface IEntityBase_Writable : IEntityBase { }
    public interface IDomainBase_Writable : IDomainBase, IEntityBase_Writable { }
    public interface IVarBase_Writable : IVarBase, IDomainBase_Writable { }
    public interface IVarBoolean_Writable : IVarBoolean, IVarBase_Writable
    {
        new Boolean Value { set; }
    }
    public interface IVarString_Writable : IVarString, IVarBase_Writable
    {
        new String Value { set; }
    }
    public interface IVarInt64_Writable : IVarInt64, IVarBase_Writable
    {
        new Int64 Value { set; }
    }
}
// </generated>

namespace NewModels.Records
{
    public abstract record EntityBase : IEntityBase
    {
        public bool IsFrozen => true;
        public void Freeze() { }
        protected abstract EntityBase OnShallowCopy();
        public IEntityBase ShallowCopy() => OnShallowCopy();

        public EntityBase() { }
        public EntityBase(EntityBase source) { }
        public EntityBase(IEntityBase source) { }
    }

    public abstract record DomainBase : EntityBase, IDomainBase
    {
        public DomainBase() { }
        public DomainBase(DomainBase source) : base(source) { }
        public DomainBase(IDomainBase source) : base(source) { }
    }

    public abstract record VarBase : DomainBase, IVarBase
    {
        public VarBase() { }
        public VarBase(VarBase source) : base(source) {  }
        public VarBase(IVarBase source) : base(source) { }
    }

    public sealed record VarBoolean : VarBase, IVarBoolean
    {
        public Boolean Value { get; init; }
        public VarBoolean() { }
        public VarBoolean(VarBoolean source) : base(source) { Value = source.Value; }
        public VarBoolean(IVarBoolean source) : base(source) { Value = source.Value; }
        protected override EntityBase OnShallowCopy() => this;
    }

    public sealed record VarString : VarBase, IVarString
    {
        public String Value { get; init; } = String.Empty;
        public VarString() { }
        public VarString(VarString source) : base(source) { Value = source.Value; }
        public VarString(IVarString source) : base(source) { Value = source.Value; }
        protected override EntityBase OnShallowCopy() => this;
    }

    public sealed record VarInt64 : VarBase, IVarInt64
    {
        public Int64 Value { get; init; }
        public VarInt64() { }
        public VarInt64(VarInt64 source) : base(source) { Value = source.Value; }
        public VarInt64(IVarInt64 source) : base(source) { Value = source.Value; }
        protected override EntityBase OnShallowCopy() => this;
    }

}

namespace NewModels.Classes
{
    public abstract class EntityBase : IEntityBase
    {
        public EntityBase() { }
        public EntityBase(EntityBase source) { }
        public EntityBase(IEntityBase source) { }

        protected abstract EntityBase OnShallowCopy();
        public IEntityBase ShallowCopy() => OnShallowCopy();

        #region IFreezable implementation
        private volatile bool _frozen = false;
        public bool IsFrozen => _frozen;
        protected virtual void OnFreeze() { }
        public void Freeze()
        {
            if (_frozen) return;
            _frozen = true;
            OnFreeze();
        }
        [MethodImpl(MethodImplOptions.NoInlining)]
        private void ThrowIsFrozen(string? memberName)
        {
            throw new InvalidOperationException($"Cannot call {memberName} when frozen.");
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void CheckNotFrozen([CallerMemberName] string? memberName = null)
        {
            if (_frozen) ThrowIsFrozen(memberName);
        }
        [MethodImpl(MethodImplOptions.NoInlining)]
        private void ThrowIsNotFrozenException(string? methodName) => throw new InvalidOperationException($"Cannot call {methodName} when not frozen.");

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void ThrowIfNotFrozen([CallerMemberName] string? methodName = null)
        {
            if (!_frozen) ThrowIsNotFrozenException(methodName);
        }
        #endregion
    }

    public abstract class DomainBase : EntityBase, IDomainBase_Writable
    {
        protected override void OnFreeze() { base.OnFreeze(); }
        public DomainBase() { }
        public DomainBase(DomainBase source) : base(source) { }
        public DomainBase(IDomainBase source) : base(source) { }
    }

    public abstract class VarBase : DomainBase, IVarBase_Writable
    {
        protected override void OnFreeze() { base.OnFreeze(); }
        public VarBase() { }
        public VarBase(VarBase source) : base(source) { }
        public VarBase(IVarBase source) : base(source) { }
    }

    public sealed class VarBoolean : VarBase, IVarBoolean_Writable
    {
        protected override EntityBase OnShallowCopy() => new VarBoolean(this);
        protected override void OnFreeze() { base.OnFreeze(); }
        public Boolean Value { get; set { CheckNotFrozen(); field = value; } }
        public VarBoolean() { }
        public VarBoolean(VarBoolean source) : base(source) { Value = source.Value; }
        public VarBoolean(IVarBoolean source) : base(source) { Value = source.Value; }
    }

    public sealed class VarString : VarBase, IVarString_Writable
    {
        protected override EntityBase OnShallowCopy() => new VarString(this);
        protected override void OnFreeze() { base.OnFreeze(); }
        public String Value { get; set { CheckNotFrozen(); field = value; } } = string.Empty;
        public VarString() { }
        public VarString(VarString source) : base(source) { Value = source.Value; }
        public VarString(IVarString source) : base(source) { Value = source.Value; }
    }

    public sealed class VarInt64 : VarBase, IVarInt64_Writable
    {
        protected override EntityBase OnShallowCopy() => new VarInt64(this);
        protected override void OnFreeze() { base.OnFreeze(); }
        public Int64 Value { get; set { CheckNotFrozen(); field = value; } }
        public VarInt64() { }
        public VarInt64(VarInt64 source) : base(source) { Value = source.Value; }
        public VarInt64(IVarInt64 source) : base(source) { Value = source.Value; }
    }
}

namespace NewModels.MsgPack3
{
    using DTOMaker.Runtime.MsgPack3;
    using MessagePack;

    [MessagePackObject(SuppressSourceGeneration = true)]
    [Union(5, typeof(VarBoolean))]
    [Union(6, typeof(VarString))]
    [Union(7, typeof(VarInt64))]
    public abstract class DomainBase : EntityBase, IDomainBase_Writable
    {
        protected override void OnFreeze() { base.OnFreeze(); }
        public DomainBase() { }
        public DomainBase(DomainBase source) : base(source) { }
        public DomainBase(IDomainBase source) : base(source) { }
    }

    [MessagePackObject(SuppressSourceGeneration = true)]
    [Union(5, typeof(VarBoolean))]
    [Union(6, typeof(VarString))]
    [Union(7, typeof(VarInt64))]
    public abstract class VarBase : DomainBase, IVarBase_Writable
    {
        protected override void OnFreeze() { base.OnFreeze(); }
        public VarBase() { }
        public VarBase(VarBase source) : base(source) { }
        public VarBase(IVarBase source) : base(source) { }
    }

    [MessagePackObject(SuppressSourceGeneration = true)]
    public sealed class VarBoolean : VarBase, IVarBoolean_Writable, IPackable<VarBoolean>
    {
        protected override int OnGetEntityId() => 5;
        protected override IEntityBase OnShallowCopy() => new VarBoolean(this);
        protected override void OnFreeze() { base.OnFreeze(); }
        [Key(1)]
        public Boolean Value { get; set { CheckNotFrozen(); field = value; } }
        public VarBoolean() { }
        public VarBoolean(VarBoolean source) : base(source) { Value = source.Value; }
        public VarBoolean(IVarBoolean source) : base(source) { Value = source.Value; }

        protected override ValueTask OnPack(IDataStore dataStore) => base.OnPack(dataStore);
        protected override ValueTask OnUnpack(IDataStore dataStore, int depth) => base.OnUnpack(dataStore, depth);
        protected override ReadOnlyMemory<byte> OnSerialize() => MessagePackSerializer.Serialize<VarBoolean>(this);
        public static VarBoolean Deserialize(ReadOnlyMemory<byte> buffer)
        {
            var result = MessagePackSerializer.Deserialize<VarBoolean>(buffer);
            result.Freeze();
            return result;
        }
    }

    [MessagePackObject(SuppressSourceGeneration = true)]
    public sealed class VarString : VarBase, IVarString_Writable, IPackable<VarString>
    {
        protected override int OnGetEntityId() => 6;
        protected override IEntityBase OnShallowCopy() => new VarString(this);
        protected override void OnFreeze() { base.OnFreeze(); }
        [Key(1)]
        public String Value { get; set { CheckNotFrozen(); field = value; } } = string.Empty;
        public VarString() { }
        public VarString(VarString source) : base(source) { Value = source.Value; }
        public VarString(IVarString source) : base(source) { Value = source.Value; }

        protected override ValueTask OnPack(IDataStore dataStore) => base.OnPack(dataStore);
        protected override ValueTask OnUnpack(IDataStore dataStore, int depth) => base.OnUnpack(dataStore, depth);
        protected override ReadOnlyMemory<byte> OnSerialize() => MessagePackSerializer.Serialize<VarString>(this);
        public static VarString Deserialize(ReadOnlyMemory<byte> buffer)
        {
            var result = MessagePackSerializer.Deserialize<VarString>(buffer);
            result.Freeze();
            return result;
        }
    }

    [MessagePackObject(SuppressSourceGeneration = true)]
    public sealed class VarInt64 : VarBase, IVarInt64_Writable, IPackable<VarInt64>
    {
        protected override int OnGetEntityId() => 7;
        protected override IEntityBase OnShallowCopy() => new VarInt64(this);
        protected override void OnFreeze() { base.OnFreeze(); }
        [Key(1)]
        public Int64 Value { get; set { CheckNotFrozen(); field = value; } }
        public VarInt64() { }
        public VarInt64(VarInt64 source) : base(source) { Value = source.Value; }
        public VarInt64(IVarInt64 source) : base(source) { Value = source.Value; }

        protected override ValueTask OnPack(IDataStore dataStore) => base.OnPack(dataStore);
        protected override ValueTask OnUnpack(IDataStore dataStore, int depth) => base.OnUnpack(dataStore, depth);
        protected override ReadOnlyMemory<byte> OnSerialize() => MessagePackSerializer.Serialize<VarInt64>(this);
        public static VarInt64 Deserialize(ReadOnlyMemory<byte> buffer)
        {
            var result = MessagePackSerializer.Deserialize<VarInt64>(buffer);
            result.Freeze();
            return result;
        }
    }
}
