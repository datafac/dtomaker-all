using System;
using System.Collections.Generic;

namespace DTOMaker.Models.BinaryTree;

public interface IBinaryTree<TKey, TValue, TNode> : IEntityBase
    where TNode : class, IBinaryTree<TKey, TValue, TNode>
{
    int Count { get; set; }
    byte Depth { get; set; }
    TKey Key { get; set; }
    TValue Value { get; set; }
    TNode? Left { get; set; }
    TNode? Right { get; set; }
}
