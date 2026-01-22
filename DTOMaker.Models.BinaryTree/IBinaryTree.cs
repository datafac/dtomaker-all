using System;
using System.Collections.Generic;

namespace DTOMaker.Models.BinaryTree;

public interface IBinaryTree<TKey, TValue> : IEntityBase
{
    int Count { get; set; }
    byte Depth { get; set; }
    TKey Key { get; set; }
    TValue Value { get; set; }
    IBinaryTree<TKey, TValue>? Left { get; set; }
    IBinaryTree<TKey, TValue>? Right { get; set; }
}
