using DTOMaker.Models;
using DTOMaker.Models.BinaryTree;
using System;

namespace TestOrg.TestApp.Models
{
    [Entity(2)]
    public interface INamedVariantSet : IEntityBase
    {
        [Member(1)] INamedVariantNode RootNode { get; set; }
    }

    [Entity(3)]
    public interface INamedVariantNode : IEntityBase
    {
        [Member(1)] int Count { get; set; }
        [Member(2)] byte Depth { get; set; }
        [Member(3)] string Key { get; set; }
        [Member(4)] IVarBase Value { get; set; }
        [Member(5)] INamedVariantNode? Left { get; set; }
        [Member(6)] INamedVariantNode? Right { get; set; }
    }

    [Entity(4)]
    public interface IVarBase : IEntityBase
    {
    }

    [Entity(5)]
    public interface IVarBoolean : IVarBase
    {
        [Member(1)] Boolean Value { get; set; }
    }

    [Entity(6)]
    public interface IVarString : IVarBase
    {
        [Member(1)] String Value { get; set; }
    }

    [Entity(7)]
    public interface IVarInt64 : IVarBase
    {
        [Member(1)] Int64 Value { get; set; }
    }
}

namespace TestOrg.TestApp.Models.JsonSystemText
{
    public partial class NamedVariantNode : IBinaryTree<string, IVarBase, NamedVariantNode>
    {
        IVarBase IBinaryTree<string, IVarBase, NamedVariantNode>.Value
        {
            get => Value;
            set => Value = value is VarBase concrete
                    ? VarBase.CreateFrom(concrete)
                    : value is IVarBase contract
                        ? VarBase.CreateFrom(contract)
                        : throw new ArgumentException($"Unexpected argument: '{value}' [{value.GetType().Name}]", nameof(value));
        }
    }
}
namespace TestOrg.TestApp.Models.MsgPack2
{
    public partial class NamedVariantNode : IBinaryTree<string, IVarBase, NamedVariantNode>
    {
        IVarBase IBinaryTree<string, IVarBase, NamedVariantNode>.Value
        {
            get => Value;
            set => Value = value is VarBase concrete
                    ? VarBase.CreateFrom(concrete)
                    : value is IVarBase contract
                        ? VarBase.CreateFrom(contract)
                        : throw new ArgumentException($"Unexpected argument: '{value}' [{value.GetType().Name}]", nameof(value));
        }
    }
}
namespace TestOrg.TestApp.Models.MemBlocks
{
    public partial class NamedVariantNode : IBinaryTree<string, IVarBase, NamedVariantNode>
    {
        IVarBase IBinaryTree<string, IVarBase, NamedVariantNode>.Value
        {
            get => Value;
            set => Value = value is VarBase concrete
                    ? VarBase.CreateRequired(concrete)
                    : value is IVarBase contract
                        ? VarBase.CreateRequired(contract)
                        : throw new ArgumentException($"Unexpected argument: '{value}' [{value.GetType().Name}]", nameof(value));
        }
    }
}

