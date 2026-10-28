using DTOMaker.Models;
using DTOMaker.Models.BinaryTree;
using System.Collections.Generic;

namespace TestOrg.TestApp.Models
{
    [Entity(1)]
    public interface IMyBinaryTree : IEntityBase
    {
        [Member(1)] int Count { get; set; }
        [Member(2)] byte Depth { get; set; }
        [Member(3)] string Key { get; set; }
        [Member(4)] long Value { get; set; }
        [Member(5)] IMyBinaryTree? Left { get; set; }
        [Member(6)] IMyBinaryTree? Right { get; set; }
    }

    //[Entity(11)]
    //public interface IMySparseList : IEntityBase
    //{
    //}

    //[Entity(12)]
    //public interface IMySparseListItems : IMySparseList
    //{
    //    [Member(1)] int Count { get; set; }
    //    [Member(2)] string Item0 { get; set; }
    //    [Member(3)] string Item1 { get; set; }
    //    [Member(4)] string Item2 { get; set; }
    //    [Member(5)] string Item3 { get; set; }
    //    [Member(6)] string Item4 { get; set; }
    //    [Member(7)] string Item5 { get; set; }
    //    [Member(8)] string Item6 { get; set; }
    //    [Member(9)] string Item7 { get; set; }
    //}

    //[Entity(13)]
    //public interface IMySparseListNodes : IMySparseList
    //{
    //    [Member(1)] int Count { get; set; }
    //    [Member(2)] IMySparseList Tree0 { get; set; }
    //    [Member(3)] IMySparseList Tree1 { get; set; }
    //    [Member(4)] IMySparseList Tree2 { get; set; }
    //    [Member(5)] IMySparseList Tree3 { get; set; }
    //    [Member(6)] IMySparseList Tree4 { get; set; }
    //    [Member(7)] IMySparseList Tree5 { get; set; }
    //    [Member(8)] IMySparseList Tree6 { get; set; }
    //    [Member(9)] IMySparseList Tree7 { get; set; }
    //}

}

namespace TestOrg.TestApp.Models.JsonSystemText
{
    public partial class MyBinaryTree : IBinaryTree<string, long, MyBinaryTree> { }
    //public partial class MySparseListItems : ISparseListItems<string>
    //{
    //    private const int Capacity = 8;
    //    public string GetItem(int index)
    //    {
    //        if (index < 0 || index >= Capacity || index >= this.Count) throw new System.IndexOutOfRangeException("Index out of range!");
    //        return index switch
    //        {
    //            0 => this.Item0,
    //            1 => this.Item1,
    //            2 => this.Item2,
    //            3 => this.Item3,
    //            4 => this.Item4,
    //            5 => this.Item5,
    //            6 => this.Item6,
    //            7 => this.Item7,
    //            _ => throw new System.IndexOutOfRangeException("Index out of range!")
    //        };
    //    }
    //}
    //public partial class MySparseListNodes : ISparseListNode<string>
    //{
    //    private const int Capacity = 8;
    //    public string GetItem(int index)
    //    {
    //        if (index < 0 || index >= this.Count) throw new System.IndexOutOfRangeException("Index out of range!");

    //        int subIndex = index;
    //        foreach(ISparseList<string> tree in this.GetTrees())
    //        {
    //            if (subIndex < tree.Count) return tree.GetItem(subIndex);
    //            subIndex = subIndex - tree.Count;
    //        }

    //        throw new System.IndexOutOfRangeException("Index out of range!");
    //    }

    //}
}
namespace TestOrg.TestApp.Models.MsgPack2
{
    public partial class MyBinaryTree : IBinaryTree<string, long, MyBinaryTree> { }
}
namespace TestOrg.TestApp.Models.MemBlocks
{
    public partial class MyBinaryTree : IBinaryTree<string, long, MyBinaryTree> { }
}
