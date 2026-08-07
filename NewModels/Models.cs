using DataFac.Storage;
using DTOMaker.Models;
using MessagePack;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
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

namespace DTOMaker.Runtime.Classes
{
    public abstract class EntityBase : IEntityBase, IEquatable<EntityBase>
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

        /// <inheritdoc/>
        public bool Equals(EntityBase? other) => true;

        /// <inheritdoc/>
        public override bool Equals(object? obj) => obj is EntityBase;

        /// <inheritdoc/>
        public override int GetHashCode() => HashCode.Combine<Type>(typeof(EntityBase));

    }
}

namespace DTOMaker.Runtime.Records
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
}

namespace T_AncestorNameSpace_
{
    [Entity(1)]
    public interface IT_AncestorImplName_ : IEntityBase
    {
    }
}

namespace T_AbstractNameSpace_
{
    [Entity(4)]
    public interface IT_AbstractImplName_ : T_AncestorNameSpace_.IT_AncestorImplName_
    {
    }
}

namespace T_ConcreteNameSpace_
{
    [Entity(5)]
    public interface IT_ConcreteImplName_ : T_AbstractNameSpace_.IT_AbstractImplName_
    {
        [Member(1)][Name("val")] string Value { get; }
    }
}

// <generated>
// todo in Domain.g.cs
namespace T_AncestorNameSpace_
{
    public interface IT_AncestorImplName__Writable : IT_AncestorImplName_, IEntityBase_Writable { }
}
namespace T_AbstractNameSpace_
{
    public interface IT_AbstractImplName__Writable : IT_AbstractImplName_, T_AncestorNameSpace_.IT_AncestorImplName__Writable { }
}
namespace T_ConcreteNameSpace_
{
    public interface IT_ConcreteImplName__Writable : IT_ConcreteImplName_, T_AbstractNameSpace_.IT_AbstractImplName__Writable
    {
        new string Value { set; }
    }
}
// </generated>

namespace T_AncestorNameSpace_.Records
{
    public abstract record T_AncestorImplName_ : DTOMaker.Runtime.Records.EntityBase, IT_AncestorImplName_
    {
        public T_AncestorImplName_() { }
        public T_AncestorImplName_(T_AncestorImplName_ source) : base(source) { }
        public T_AncestorImplName_(IT_AncestorImplName_ source) : base(source) { }
    }
}

namespace T_AbstractNameSpace_.Records
{
    public abstract record T_AbstractImplName_ : T_AncestorNameSpace_.Records.T_AncestorImplName_, IT_AbstractImplName_
    {
        public T_AbstractImplName_() { }
        public T_AbstractImplName_(T_AbstractImplName_ source) : base(source) { }
        public T_AbstractImplName_(IT_AbstractImplName_ source) : base(source) { }
    }
}

namespace T_ConcreteNameSpace_.Records
{
    public sealed record T_ConcreteImplName_ : T_AbstractNameSpace_.Records.T_AbstractImplName_, IT_ConcreteImplName_
    {
        public string Value { get; init; } = string.Empty;
        public T_ConcreteImplName_() { }
        public T_ConcreteImplName_(T_ConcreteImplName_ source) : base(source) { Value = source.Value; }
        public T_ConcreteImplName_(IT_ConcreteImplName_ source) : base(source) { Value = source.Value; }
        protected override DTOMaker.Runtime.Records.EntityBase OnShallowCopy() => this;
    }
}

namespace T_AncestorNameSpace_.Classes
{
    public abstract class T_AncestorImplName_ : DTOMaker.Runtime.Classes.EntityBase, IT_AncestorImplName__Writable
    {
        protected override void OnFreeze() { base.OnFreeze(); }
        public T_AncestorImplName_() { }
        public T_AncestorImplName_(T_AncestorImplName_ source) : base(source) { }
        public T_AncestorImplName_(IT_AncestorImplName_ source) : base(source) { }
    }
}

namespace T_AbstractNameSpace_.Classes
{
    public abstract class T_AbstractImplName_ : T_AncestorNameSpace_.Classes.T_AncestorImplName_, IT_AbstractImplName__Writable
    {
        protected override void OnFreeze() { base.OnFreeze(); }
        public T_AbstractImplName_() { }
        public T_AbstractImplName_(T_AbstractImplName_ source) : base(source) { }
        public T_AbstractImplName_(IT_AbstractImplName_ source) : base(source) { }
    }
}

namespace T_ConcreteNameSpace_.Classes
{
    public sealed class T_ConcreteImplName_ : T_AbstractNameSpace_.Classes.T_AbstractImplName_, IT_ConcreteImplName__Writable
    {
        protected override DTOMaker.Runtime.Classes.EntityBase OnShallowCopy() => new T_ConcreteImplName_(this);
        protected override void OnFreeze() { base.OnFreeze(); }
        public string Value { get; set { CheckNotFrozen(); field = value; } } = string.Empty;
        public T_ConcreteImplName_() { }
        public T_ConcreteImplName_(T_ConcreteImplName_ source) : base(source) { Value = source.Value; }
        public T_ConcreteImplName_(IT_ConcreteImplName_ source) : base(source) { Value = source.Value; }
    }
}

namespace T_AncestorNameSpace_.MsgPack3
{
    [MessagePackObject(SuppressSourceGeneration = true)]
    [Union(5, typeof(T_ConcreteNameSpace_.MsgPack3.T_ConcreteImplName_))]
    public abstract class T_AncestorImplName_ : DTOMaker.Runtime.MsgPack3.EntityBase, IT_AncestorImplName__Writable
    {
        protected override void OnFreeze() { base.OnFreeze(); }
        public T_AncestorImplName_() { }
        public T_AncestorImplName_(T_AncestorImplName_ source) : base(source) { }
        public T_AncestorImplName_(IT_AncestorImplName_ source) : base(source) { }
    }
}

namespace T_AbstractNameSpace_.MsgPack3
{
    [MessagePackObject(SuppressSourceGeneration = true)]
    [Union(5, typeof(T_ConcreteNameSpace_.MsgPack3.T_ConcreteImplName_))]
    public abstract class T_AbstractImplName_ : T_AncestorNameSpace_.MsgPack3.T_AncestorImplName_, IT_AbstractImplName__Writable
    {
        protected override void OnFreeze() { base.OnFreeze(); }
        public T_AbstractImplName_() { }
        public T_AbstractImplName_(T_AbstractImplName_ source) : base(source) { }
        public T_AbstractImplName_(IT_AbstractImplName_ source) : base(source) { }
    }
}

namespace T_ConcreteNameSpace_.MsgPack3
{
    [MessagePackObject(SuppressSourceGeneration = true)]
    public sealed class T_ConcreteImplName_ : T_AbstractNameSpace_.MsgPack3.T_AbstractImplName_, IT_ConcreteImplName__Writable
    {
        protected override int OnGetEntityId() => 5;
        protected override IEntityBase OnShallowCopy() => new T_ConcreteImplName_(this);
        protected override void OnFreeze() { base.OnFreeze(); }
        [Key(1)]
        public string Value { get; set { CheckNotFrozen(); field = value; } } = string.Empty;
        public T_ConcreteImplName_() { }
        public T_ConcreteImplName_(T_ConcreteImplName_ source) : base(source) { Value = source.Value; }
        public T_ConcreteImplName_(IT_ConcreteImplName_ source) : base(source) { Value = source.Value; }

        protected override ValueTask OnPack(IBlobStore blobStore, CancellationToken cancellation) => base.OnPack(blobStore, cancellation);
        protected override ValueTask OnUnpack(IBlobStore blobStore, int depth, CancellationToken cancellation) => base.OnUnpack(blobStore, depth, cancellation);
    }
}

namespace T_AncestorNameSpace_.MemBlox2
{
    using DTOMaker.Runtime.MemBlocks;
    public abstract class T_AncestorImplName_ : EntityBase, T_AncestorNameSpace_.IT_AncestorImplName__Writable
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

        public static T_AncestorImplName_ DeserializeFrom(ReadOnlyMemory<byte> buffer)
        {
            int entityId = EntityMetadata.GetEntityId(buffer);
            return entityId switch
            {
                //##foreach(var derived in entity.DerivedEntities) {
                //##using var _ = NewScope(derived);
                T_ConcreteNameSpace_.MemBlox2.T_ConcreteImplName_.EntityId => new T_ConcreteNameSpace_.MemBlox2.T_ConcreteImplName_(buffer),
                //##}
                _ => throw new InvalidDataException($"Header contains unexpected entity id: {entityId}")
            };
        }

        protected override void OnFreeze() { base.OnFreeze(); }

        protected T_AncestorImplName_(EntityMetadata metadata) : base(metadata)
        {
            _readonlyLocalBlock = _readonlyGlobalBlock.Slice(BlockOffset, BlockLength);
            _writableLocalBlock = _writableGlobalBlock.Slice(BlockOffset, BlockLength);
        }
        protected T_AncestorImplName_(EntityMetadata metadata, T_AncestorImplName_ source) : base(metadata, source)
        {
            _readonlyLocalBlock = _readonlyGlobalBlock.Slice(BlockOffset, BlockLength);
            _writableLocalBlock = _writableGlobalBlock.Slice(BlockOffset, BlockLength);
        }
        protected T_AncestorImplName_(EntityMetadata metadata, T_AncestorNameSpace_.IT_AncestorImplName_ source) : base(metadata, source)
        {
            _readonlyLocalBlock = _readonlyGlobalBlock.Slice(BlockOffset, BlockLength);
            _writableLocalBlock = _writableGlobalBlock.Slice(BlockOffset, BlockLength);
        }
        protected T_AncestorImplName_(EntityMetadata metadata, ReadOnlyMemory<byte> buffer) : base(metadata, buffer)
        {
            _readonlyLocalBlock = _readonlyGlobalBlock.Slice(BlockOffset, BlockLength);
            _writableLocalBlock = Memory<byte>.Empty;
        }
    }
}

namespace T_AbstractNameSpace_.MemBlox2
{
    using DTOMaker.Runtime.MemBlocks;
    public abstract class T_AbstractImplName_ : T_AncestorNameSpace_.MemBlox2.T_AncestorImplName_, IT_AbstractImplName__Writable
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

        public new static T_AbstractImplName_ DeserializeFrom(ReadOnlyMemory<byte> buffer)
        {
            int entityId = EntityMetadata.GetEntityId(buffer);
            return entityId switch
            {
                //##foreach(var derived in entity.DerivedEntities) {
                //##using var _ = NewScope(derived);
                T_ConcreteNameSpace_.MemBlox2.T_ConcreteImplName_.EntityId => new T_ConcreteNameSpace_.MemBlox2.T_ConcreteImplName_(buffer),
                //##}
                _ => throw new InvalidDataException($"Header contains unexpected entity id: {entityId}")
            };
        }

        protected override void OnFreeze() { base.OnFreeze(); }

        protected T_AbstractImplName_(EntityMetadata metadata) : base(metadata)
        {
            _readonlyLocalBlock = _readonlyGlobalBlock.Slice(BlockOffset, BlockLength);
            _writableLocalBlock = _writableGlobalBlock.Slice(BlockOffset, BlockLength);
        }
        protected T_AbstractImplName_(EntityMetadata metadata, T_AbstractImplName_ source) : base(metadata, source)
        {
            _readonlyLocalBlock = _readonlyGlobalBlock.Slice(BlockOffset, BlockLength);
            _writableLocalBlock = _writableGlobalBlock.Slice(BlockOffset, BlockLength);
        }
        protected T_AbstractImplName_(EntityMetadata metadata, IT_AbstractImplName_ source) : base(metadata, source)
        {
            _readonlyLocalBlock = _readonlyGlobalBlock.Slice(BlockOffset, BlockLength);
            _writableLocalBlock = _writableGlobalBlock.Slice(BlockOffset, BlockLength);
        }
        protected T_AbstractImplName_(EntityMetadata metadata, ReadOnlyMemory<byte> buffer) : base(metadata, buffer)
        {
            _readonlyLocalBlock = _readonlyGlobalBlock.Slice(BlockOffset, BlockLength);
            _writableLocalBlock = Memory<byte>.Empty;
        }

    }
}

namespace T_ConcreteNameSpace_.MemBlox2
{
    using DTOMaker.Runtime.MemBlocks;
    public sealed class T_ConcreteImplName_ : T_AbstractNameSpace_.MemBlox2.T_AbstractImplName_, T_ConcreteNameSpace_.IT_ConcreteImplName__Writable
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

        public new static T_ConcreteImplName_ DeserializeFrom(ReadOnlyMemory<byte> buffer)
        {
            int entityId = EntityMetadata.GetEntityId(buffer);
            return entityId switch
            {
                //##foreach(var derived in entity.DerivedEntities) {
                //##using var _ = NewScope(derived);
                T_ConcreteImplName_.EntityId => new T_ConcreteImplName_(buffer),
                //##}
                _ => throw new InvalidDataException($"Header contains unexpected entity id: {entityId}")
            };
        }

        public const int EntityId = T_EntityId_;
        protected override int OnGetEntityId() => T_EntityId_;
        protected override IEntityBase OnShallowCopy() => new T_ConcreteImplName_(this);
        protected override void OnFreeze() { base.OnFreeze(); }

        public T_ConcreteImplName_() : base(_metadata)
        {
            _readonlyLocalBlock = _readonlyGlobalBlock.Slice(BlockOffset, BlockLength);
            _writableLocalBlock = _writableGlobalBlock.Slice(BlockOffset, BlockLength);
        }
        public T_ConcreteImplName_(T_ConcreteImplName_ source) : base(_metadata, source)
        {
            _readonlyLocalBlock = _readonlyGlobalBlock.Slice(BlockOffset, BlockLength);
            _writableLocalBlock = _writableGlobalBlock.Slice(BlockOffset, BlockLength);
            this.Value = source.Value;
        }
        public T_ConcreteImplName_(T_ConcreteNameSpace_.IT_ConcreteImplName_ source) : base(_metadata, source)
        {
            _readonlyLocalBlock = _readonlyGlobalBlock.Slice(BlockOffset, BlockLength);
            _writableLocalBlock = _writableGlobalBlock.Slice(BlockOffset, BlockLength);
            this.Value = source.Value;
        }
        public T_ConcreteImplName_(ReadOnlyMemory<byte> buffer) : base(_metadata, buffer)
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
        private async ValueTask Value_Pack(IBlobStore blobStore, CancellationToken cancellation)
        {
            var writableField = _writableLocalBlock.Slice(Value_FieldOffset, 64);
            await PackText(_Value, writableField, blobStore);
        }
        private async ValueTask Value_Unpack(IBlobStore blobStore, CancellationToken cancellation)
        {
            var readonlyField = _readonlyLocalBlock.Slice(Value_FieldOffset, 64);
            var data = await UnpackData(readonlyField, blobStore);
#if NET8_0_OR_GREATER
            _Value = data.HasValue ? System.Text.Encoding.UTF8.GetString(data.Value.Span) : string.Empty;
#else
            _Value = data.HasValue ? System.Text.Encoding.UTF8.GetString(data.Value.ToArray()) : string.Empty;
#endif
        }

        /// <inheritdoc/>
        protected override async ValueTask OnPack(IBlobStore blobStore, CancellationToken cancellation)
        {
            await base.OnPack(blobStore, cancellation);
            await Value_Pack(blobStore, cancellation);
        }

        /// <inheritdoc/>
        protected override async ValueTask OnUnpack(IBlobStore blobStore, int depth, CancellationToken cancellation)
        {
            await base.OnUnpack(blobStore, depth, cancellation);
            await Value_Unpack(blobStore, cancellation);
        }

    }
}
