using DTOMaker.Models;
using DTOMaker.Models.BinaryTree;
using System;

namespace TestOrg.TestApp.Models
{
    [Entity(2)]
    public interface IVarSet : IEntityBase
    {
        [Member(1)][Name("set")] IVarSetNode Root { get; set; }
    }

    [Entity(3)]
    public interface IVarSetNode : IEntityBase
    {
        [Member(1)][Name("N")] int Count { get; set; }
        [Member(2)][Name("D")] byte Depth { get; set; }
        [Member(3)][Name("key")] string Key { get; set; }
        [Member(4)][Name("val")] IVarBase Value { get; set; }
        [Member(5)][Name("L")] IVarSetNode? Left { get; set; }
        [Member(6)][Name("R")] IVarSetNode? Right { get; set; }
    }

    [Entity(4)]
    public interface IVarBase : IEntityBase
    {
    }

    [Entity(5)]
    public interface IVarBoolean : IVarBase
    {
        [Member(1)][Name("val")] Boolean Value { get; set; }
    }

    [Entity(6)]
    public interface IVarString : IVarBase
    {
        [Member(1)][Name("val")] String Value { get; set; }
    }

    [Entity(7)]
    public interface IVarInt64 : IVarBase
    {
        [Member(1)][Name("val")] Int64 Value { get; set; }
    }
}

namespace TestOrg.TestApp.Models.JsonNewtonSoft
{
    public partial class VarSetNode : IBinaryTree<string, IVarBase, VarSetNode>
    {
        IVarBase IBinaryTree<string, IVarBase, VarSetNode>.Value
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
namespace TestOrg.TestApp.Models.JsonSystemText
{
    public partial class VarSetNode : IBinaryTree<string, IVarBase, VarSetNode>
    {
        IVarBase IBinaryTree<string, IVarBase, VarSetNode>.Value
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
    public partial class VarSetNode : IBinaryTree<string, IVarBase, VarSetNode>
    {
        IVarBase IBinaryTree<string, IVarBase, VarSetNode>.Value
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
    public partial class VarSetNode : IBinaryTree<string, IVarBase, VarSetNode>
    {
        IVarBase IBinaryTree<string, IVarBase, VarSetNode>.Value
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

