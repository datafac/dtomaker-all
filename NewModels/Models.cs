using DataFac.Storage;
using DTOMaker.Models;
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
    public interface IT_BaseImplName_ : IEntityBase
    {
    }

    [Entity(4)]
    public interface IT_AbstractEntity_ : IT_BaseImplName_
    {
    }

    [Entity(5)]
    public interface IT_ConcreteEntity_ : IT_AbstractEntity_
    {
        [Member(1)][Name("val")] string Value { get; }
    }
}

// <generated>
// in Domain.g.cs
namespace NewModels
{
    public interface IEntityBase_Writable : IEntityBase { }
    public interface IT_BaseImplName__Writable : IT_BaseImplName_, IEntityBase_Writable { }
    public interface IT_AbstractEntity__Writable : IT_AbstractEntity_, IT_BaseImplName__Writable { }
    public interface IT_ConcreteEntity__Writable : IT_ConcreteEntity_, IT_AbstractEntity__Writable
    {
        new string Value { set; }
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

    public abstract record T_BaseImplName_ : EntityBase, IT_BaseImplName_
    {
        public T_BaseImplName_() { }
        public T_BaseImplName_(T_BaseImplName_ source) : base(source) { }
        public T_BaseImplName_(IT_BaseImplName_ source) : base(source) { }
    }

    public abstract record T_AbstractEntity_ : T_BaseImplName_, IT_AbstractEntity_
    {
        public T_AbstractEntity_() { }
        public T_AbstractEntity_(T_AbstractEntity_ source) : base(source) {  }
        public T_AbstractEntity_(IT_AbstractEntity_ source) : base(source) { }
    }

    public sealed record T_ConcreteEntity_ : T_AbstractEntity_, IT_ConcreteEntity_
    {
        public string Value { get; init; } = string.Empty;
        public T_ConcreteEntity_() { }
        public T_ConcreteEntity_(T_ConcreteEntity_ source) : base(source) { Value = source.Value; }
        public T_ConcreteEntity_(IT_ConcreteEntity_ source) : base(source) { Value = source.Value; }
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

    public abstract class T_BaseImplName_ : EntityBase, IT_BaseImplName__Writable
    {
        protected override void OnFreeze() { base.OnFreeze(); }
        public T_BaseImplName_() { }
        public T_BaseImplName_(T_BaseImplName_ source) : base(source) { }
        public T_BaseImplName_(IT_BaseImplName_ source) : base(source) { }
    }

    public abstract class T_AbstractEntity_ : T_BaseImplName_, IT_AbstractEntity__Writable
    {
        protected override void OnFreeze() { base.OnFreeze(); }
        public T_AbstractEntity_() { }
        public T_AbstractEntity_(T_AbstractEntity_ source) : base(source) { }
        public T_AbstractEntity_(IT_AbstractEntity_ source) : base(source) { }
    }

    public sealed class T_ConcreteEntity_ : T_AbstractEntity_, IT_ConcreteEntity__Writable
    {
        protected override EntityBase OnShallowCopy() => new T_ConcreteEntity_(this);
        protected override void OnFreeze() { base.OnFreeze(); }
        public string Value { get; set { CheckNotFrozen(); field = value; } } = string.Empty;
        public T_ConcreteEntity_() { }
        public T_ConcreteEntity_(T_ConcreteEntity_ source) : base(source) { Value = source.Value; }
        public T_ConcreteEntity_(IT_ConcreteEntity_ source) : base(source) { Value = source.Value; }
    }
}

namespace NewModels.MsgPack3
{
    using DTOMaker.Runtime.MsgPack3;
    using MessagePack;
    using System.Threading;

    [MessagePackObject(SuppressSourceGeneration = true)]
    [Union(5, typeof(T_ConcreteEntity_))]
    public abstract class T_BaseImplName_ : EntityBase, IT_BaseImplName__Writable
    {
        protected override void OnFreeze() { base.OnFreeze(); }
        public T_BaseImplName_() { }
        public T_BaseImplName_(T_BaseImplName_ source) : base(source) { }
        public T_BaseImplName_(IT_BaseImplName_ source) : base(source) { }
    }

    [MessagePackObject(SuppressSourceGeneration = true)]
    [Union(5, typeof(T_ConcreteEntity_))]
    public abstract class T_AbstractEntity_ : T_BaseImplName_, IT_AbstractEntity__Writable
    {
        protected override void OnFreeze() { base.OnFreeze(); }
        public T_AbstractEntity_() { }
        public T_AbstractEntity_(T_AbstractEntity_ source) : base(source) { }
        public T_AbstractEntity_(IT_AbstractEntity_ source) : base(source) { }
    }

    [MessagePackObject(SuppressSourceGeneration = true)]
    public sealed class T_ConcreteEntity_ : T_AbstractEntity_, IT_ConcreteEntity__Writable
    {
        protected override int OnGetEntityId() => 5;
        protected override IEntityBase OnShallowCopy() => new T_ConcreteEntity_(this);
        protected override void OnFreeze() { base.OnFreeze(); }
        [Key(1)]
        public string Value { get; set { CheckNotFrozen(); field = value; } } = string.Empty;
        public T_ConcreteEntity_() { }
        public T_ConcreteEntity_(T_ConcreteEntity_ source) : base(source) { Value = source.Value; }
        public T_ConcreteEntity_(IT_ConcreteEntity_ source) : base(source) { Value = source.Value; }

        protected override ValueTask OnPack(IDataStore dataStore, CancellationToken cancellation) => base.OnPack(dataStore, cancellation);
        protected override ValueTask OnUnpack(IDataStore dataStore, int depth, CancellationToken cancellation) => base.OnUnpack(dataStore, depth, cancellation);
    }
}

namespace NewModels.MemBlox2
{
    using DTOMaker.Runtime.MemBlocks;
    using System.IO;
    using System.Threading;

    public abstract class T_BaseImplName_ : EntityBase, IT_BaseImplName__Writable
    {
        //##if(false) {
        private const int T_ClassHeight_ = 1;
        private const int T_EntityId_ = 4;
        private const bool T_MemberObsoleteIsError_ = false;
        private const long T_BlockStructureCode_ = 0x0000 + 0x0001;
        //##}
        private const long BlockStructureCode = T_BlockStructureCode_;
        private const int ClassHeight = T_ClassHeight_;
        private readonly Memory<byte> _writableLocalBlock;
        private readonly ReadOnlyMemory<byte> _readonlyLocalBlock;

        private static readonly EntityMetadata _metadata = new EntityMetadata(T_EntityId_, BlockStructureCode);

        private static readonly int BlockOffset = _metadata.LocalBlockOffset;
        private static readonly int BlockLength = _metadata.LocalBlockLength;

        public static T_BaseImplName_ DeserializeFrom(ReadOnlyMemory<byte> buffer)
        {
            int entityId = EntityMetadata.GetEntityId(buffer);
            return entityId switch
            {
                //##foreach(var derived in entity.DerivedEntities) {
                //##using var _ = NewScope(derived);
                T_ConcreteEntity_.EntityId => new T_ConcreteEntity_(buffer),
                //##}
                _ => throw new InvalidDataException($"Header contains unexpected entity id: {entityId}")
            };
        }

        protected override void OnFreeze() { base.OnFreeze(); }

        protected T_BaseImplName_(EntityMetadata metadata) : base(metadata)
        {
            _readonlyLocalBlock = _readonlyGlobalBlock.Slice(BlockOffset, BlockLength);
            _writableLocalBlock = _writableGlobalBlock.Slice(BlockOffset, BlockLength);
        }
        protected T_BaseImplName_(EntityMetadata metadata, T_BaseImplName_ source) : base(metadata, source)
        {
            _readonlyLocalBlock = _readonlyGlobalBlock.Slice(BlockOffset, BlockLength);
            _writableLocalBlock = _writableGlobalBlock.Slice(BlockOffset, BlockLength);
        }
        protected T_BaseImplName_(EntityMetadata metadata, IT_BaseImplName_ source) : base(metadata, source)
        {
            _readonlyLocalBlock = _readonlyGlobalBlock.Slice(BlockOffset, BlockLength);
            _writableLocalBlock = _writableGlobalBlock.Slice(BlockOffset, BlockLength);
        }
        protected T_BaseImplName_(EntityMetadata metadata, ReadOnlyMemory<byte> buffer) : base(metadata, buffer)
        {
            _readonlyLocalBlock = _readonlyGlobalBlock.Slice(BlockOffset, BlockLength);
            _writableLocalBlock = Memory<byte>.Empty;
        }
    }

    public abstract class T_AbstractEntity_ : T_BaseImplName_, IT_AbstractEntity__Writable
    {
        //##if(false) {
        private const int T_ClassHeight_ = 2;
        private const int T_EntityId_ = 4;
        private const bool T_MemberObsoleteIsError_ = false;
        private const long T_BlockStructureCode_ = 0x0000 + 0x0000 + 0x0002;
        //##}
        private const long BlockStructureCode = T_BlockStructureCode_;
        private const int ClassHeight = T_ClassHeight_;
        private readonly Memory<byte> _writableLocalBlock;
        private readonly ReadOnlyMemory<byte> _readonlyLocalBlock;

        private static readonly EntityMetadata _metadata = new EntityMetadata(T_EntityId_, BlockStructureCode);

        private static readonly int BlockOffset = _metadata.LocalBlockOffset;
        private static readonly int BlockLength = _metadata.LocalBlockLength;

        public new static T_AbstractEntity_ DeserializeFrom(ReadOnlyMemory<byte> buffer)
        {
            int entityId = EntityMetadata.GetEntityId(buffer);
            return entityId switch
            {
                //##foreach(var derived in entity.DerivedEntities) {
                //##using var _ = NewScope(derived);
                T_ConcreteEntity_.EntityId => new T_ConcreteEntity_(buffer),
                //##}
                _ => throw new InvalidDataException($"Header contains unexpected entity id: {entityId}")
            };
        }

        protected override void OnFreeze() { base.OnFreeze(); }

        protected T_AbstractEntity_(EntityMetadata metadata) : base(metadata)
        {
            _readonlyLocalBlock = _readonlyGlobalBlock.Slice(BlockOffset, BlockLength);
            _writableLocalBlock = _writableGlobalBlock.Slice(BlockOffset, BlockLength);
        }
        protected T_AbstractEntity_(EntityMetadata metadata, T_AbstractEntity_ source) : base(metadata, source)
        {
            _readonlyLocalBlock = _readonlyGlobalBlock.Slice(BlockOffset, BlockLength);
            _writableLocalBlock = _writableGlobalBlock.Slice(BlockOffset, BlockLength);
        }
        protected T_AbstractEntity_(EntityMetadata metadata, IT_AbstractEntity_ source) : base(metadata, source)
        {
            _readonlyLocalBlock = _readonlyGlobalBlock.Slice(BlockOffset, BlockLength);
            _writableLocalBlock = _writableGlobalBlock.Slice(BlockOffset, BlockLength);
        }
        protected T_AbstractEntity_(EntityMetadata metadata, ReadOnlyMemory<byte> buffer) : base(metadata, buffer)
        {
            _readonlyLocalBlock = _readonlyGlobalBlock.Slice(BlockOffset, BlockLength);
            _writableLocalBlock = Memory<byte>.Empty;
        }

    }

    public sealed class T_ConcreteEntity_ : T_AbstractEntity_, IT_ConcreteEntity__Writable
    {
        //##if(false) {
        private const int T_ClassHeight_ = 3;
        private const int T_EntityId_ = 5;
        private const bool T_MemberObsoleteIsError_ = false;
        private const long T_BlockStructureCode_ = 0x7000 + 0x0000 + 0x0000 + 0x0003;
        //##}
        private const long BlockStructureCode = T_BlockStructureCode_;
        private const int ClassHeight = T_ClassHeight_;
        private readonly Memory<byte> _writableLocalBlock;
        private readonly ReadOnlyMemory<byte> _readonlyLocalBlock;

        private static readonly EntityMetadata _metadata = new EntityMetadata(T_EntityId_, BlockStructureCode);

        private static readonly int BlockOffset = _metadata.LocalBlockOffset;
        private static readonly int BlockLength = _metadata.LocalBlockLength;

        public new static T_ConcreteEntity_ DeserializeFrom(ReadOnlyMemory<byte> buffer)
        {
            int entityId = EntityMetadata.GetEntityId(buffer);
            return entityId switch
            {
                //##foreach(var derived in entity.DerivedEntities) {
                //##using var _ = NewScope(derived);
                T_ConcreteEntity_.EntityId => new T_ConcreteEntity_(buffer),
                //##}
                _ => throw new InvalidDataException($"Header contains unexpected entity id: {entityId}")
            };
        }

        public const int EntityId = T_EntityId_;
        protected override int OnGetEntityId() => T_EntityId_;
        protected override IEntityBase OnShallowCopy() => new T_ConcreteEntity_(this);
        protected override void OnFreeze() { base.OnFreeze(); }

        public T_ConcreteEntity_() : base(_metadata)
        {
            _readonlyLocalBlock = _readonlyGlobalBlock.Slice(BlockOffset, BlockLength);
            _writableLocalBlock = _writableGlobalBlock.Slice(BlockOffset, BlockLength);
        }
        public T_ConcreteEntity_(T_ConcreteEntity_ source) : base(_metadata, source)
        {
            _readonlyLocalBlock = _readonlyGlobalBlock.Slice(BlockOffset, BlockLength);
            _writableLocalBlock = _writableGlobalBlock.Slice(BlockOffset, BlockLength);
            this.Value = source.Value;
        }
        public T_ConcreteEntity_(IT_ConcreteEntity_ source) : base(_metadata, source)
        {
            _readonlyLocalBlock = _readonlyGlobalBlock.Slice(BlockOffset, BlockLength);
            _writableLocalBlock = _writableGlobalBlock.Slice(BlockOffset, BlockLength);
            this.Value = source.Value;
        }
        public T_ConcreteEntity_(ReadOnlyMemory<byte> buffer) : base(_metadata, buffer)
        {
            _readonlyLocalBlock = _readonlyGlobalBlock.Slice(BlockOffset, BlockLength);
            _writableLocalBlock = Memory<byte>.Empty;
        }

        private string _Value = string.Empty;
        public string Value
        {
            get { return _Value; }
            set { ThrowIfFrozen(); _Value = value; }
        }
        private const int Value_FieldOffset = 0;
        private async ValueTask Value_Pack(IDataStore dataStore, CancellationToken cancellation)
        {
            var writableField = _writableLocalBlock.Slice(Value_FieldOffset, 64);
            await PackText(_Value, writableField, dataStore);
        }
        private async ValueTask Value_Unpack(IDataStore dataStore, CancellationToken cancellation)
        {
            var readonlyField = _readonlyLocalBlock.Slice(Value_FieldOffset, 64);
            var data = await UnpackData(readonlyField, dataStore);
#if NET8_0_OR_GREATER
            _Value = data.HasValue ? System.Text.Encoding.UTF8.GetString(data.Value.Span) : string.Empty;
#else
            _Value = data.HasValue ? System.Text.Encoding.UTF8.GetString(data.Value.ToArray()) : string.Empty;
#endif
        }

        /// <inheritdoc/>
        protected override async ValueTask OnPack(IDataStore dataStore, CancellationToken cancellation)
        {
            await base.OnPack(dataStore, cancellation);
            await Value_Pack(dataStore, cancellation);
        }

        /// <inheritdoc/>
        protected override async ValueTask OnUnpack(IDataStore dataStore, int depth, CancellationToken cancellation)
        {
            await base.OnUnpack(dataStore, depth, cancellation);
            await Value_Unpack(dataStore, cancellation);
        }

    }
}
