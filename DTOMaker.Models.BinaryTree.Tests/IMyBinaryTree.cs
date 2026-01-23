using DTOMaker.Models;
using DTOMaker.Models.BinaryTree;

namespace TestOrg.TestApp.Models
{
    [Entity(1, LayoutMethod.Linear)]
    public interface IMyBinaryTree : IEntityBase
    {
        [Member(1)] int Count { get; set; }
        [Member(2)] byte Depth { get; set; }
        [Member(3)] string Key { get; set; }
        [Member(4)] long Value { get; set; }
        [Member(5)] IMyBinaryTree? Left { get; set; }
        [Member(6)] IMyBinaryTree? Right { get; set; }
    }
}

namespace TestOrg.TestApp.Models.JsonSystemText
{
    public partial class MyBinaryTree : IBinaryTree<string, long>
    {
        IBinaryTree<string, long>? IBinaryTree<string, long>.Left
        {
            get => Left;
            set => Left = value is null ? null : new MyBinaryTree((IMyBinaryTree)value);
        }
        IBinaryTree<string, long>? IBinaryTree<string, long>.Right
        {
            get => Right;
            set => Right = value is null ? null : new MyBinaryTree((IMyBinaryTree)value);
        }
    }
}
namespace TestOrg.TestApp.Models.MsgPack2
{
    public partial class MyBinaryTree : IBinaryTree<string, long>
    {
        IBinaryTree<string, long>? IBinaryTree<string, long>.Left
        {
            get => Left;
            set => Left = value is null ? null : new MyBinaryTree((IMyBinaryTree)value);
        }
        IBinaryTree<string, long>? IBinaryTree<string, long>.Right
        {
            get => Right;
            set => Right = value is null ? null : new MyBinaryTree((IMyBinaryTree)value);
        }
    }
}
