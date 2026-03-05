using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace DTOMaker.Models.BinaryTree;

/// <summary>
/// Provides extension methods for working with data structures that implement IBinaryTree<TKey, TValue>.
/// </summary>
/// <remarks>
/// These methods enable common operations such as searching, enumeration, addition, and removal
/// of nodes in a binary tree. When addition or removal operations are performed tree balance is maintained via
/// node rotatations so that the depth difference between left and right subtrees is never more than 1.
/// </remarks>
public static class BinaryTreeExtensions
{
    public static TNode? Get<TKey, TValue, TNode>(this TNode? tree, TKey key)
        where TKey : notnull, IComparable<TKey>
        where TNode : class, IBinaryTree<TKey, TValue, TNode>
    {
        if (tree is null) return null;
        int comparison = tree.Key.CompareTo(key);
        if (comparison == 0) return tree; // found
        return (comparison > 0)
            ? tree.Left.Get<TKey, TValue, TNode>(key) // go left
            : tree.Right.Get<TKey, TValue, TNode>(key); // go right
    }

    public static IEnumerable<KeyValuePair<TKey, TValue>> GetKeyValuePairs<TKey, TValue, TNode>(this TNode? tree, bool reverse = false)
        where TKey : notnull, IComparable<TKey>
        where TNode : class, IBinaryTree<TKey, TValue, TNode>
    {
        if (tree is null) yield break;
        if (reverse)
        {
            foreach (var kvp in tree.Right.GetKeyValuePairs<TKey, TValue, TNode>(true)) yield return kvp;
            yield return new KeyValuePair<TKey, TValue>(tree.Key, tree.Value);
            foreach (var kvp in tree.Left.GetKeyValuePairs<TKey, TValue, TNode>(true)) yield return kvp;
        }
        else
        {
            foreach (var kvp in tree.Left.GetKeyValuePairs<TKey, TValue, TNode>(false)) yield return kvp;
            yield return new KeyValuePair<TKey, TValue>(tree.Key, tree.Value);
            foreach (var kvp in tree.Right.GetKeyValuePairs<TKey, TValue, TNode>(false)) yield return kvp;

        }
    }

    private static int GetCount<TKey, TValue, TNode>(this TNode? tree)
        where TNode : class, IBinaryTree<TKey, TValue, TNode>
    {
        if (tree is null) return 0;
        if (tree.IsFrozen) return tree.Count;
        return 1 + tree.Left.GetCount<TKey, TValue, TNode>() + tree.Right.GetCount<TKey, TValue, TNode>();
    }

    private static byte GetDepth<TKey, TValue, TNode>(this TNode? tree)
        where TNode : class, IBinaryTree<TKey, TValue, TNode>
    {
        if (tree is null) return 0;
        if (tree.IsFrozen) return tree.Depth;
        return (byte)(1 + Math.Max(tree.Left.GetDepth<TKey, TValue, TNode>(), tree.Right.GetDepth<TKey, TValue, TNode>()));
    }

    private static int GetBalance<TKey, TValue, TNode>(this TNode? tree)
        where TNode : class, IBinaryTree<TKey, TValue, TNode>
    {
        if (tree is null) return 0;
        return tree.Left.GetDepth<TKey, TValue, TNode>() - tree.Right.GetDepth<TKey, TValue, TNode>();
    }

    private static void TrySetCountAndDepth<TKey, TValue, TNode>(this TNode? tree)
        where TNode : class, IBinaryTree<TKey, TValue, TNode>
    {
        if (tree is null) return;
        if (tree.IsFrozen) return;
        tree.Count = 1 + tree.Left.GetCount<TKey, TValue, TNode>() + tree.Right.GetCount<TKey, TValue, TNode>();
        tree.Depth = (byte)(1 + Math.Max(tree.Left.GetDepth<TKey, TValue, TNode>(), tree.Right.GetDepth<TKey, TValue, TNode>()));
    }

    private static TNode RotateLeft<TKey, TValue, TNode>(this TNode node)
        where TNode : class, IBinaryTree<TKey, TValue, TNode>
        where TKey : notnull, IComparable<TKey>
    {
        if (node.Right is null) return node; // cannot rotate left
        var newRoot = node.Right;
        node.Right = newRoot.Left;
        // unfreeze newRoot if needed
        newRoot = newRoot.Unfrozen();
        newRoot.Left = node;
        return newRoot;
    }

    private static TNode RotateRight<TKey, TValue, TNode>(this TNode node)
        where TNode : class, IBinaryTree<TKey, TValue, TNode>
        where TKey : notnull, IComparable<TKey>
    {
        if (node.Left is null) return node; // cannot rotate right

        var newRoot = node.Left;
        node.Left = newRoot.Right;
        // unfreeze newRoot if needed
        newRoot = newRoot.Unfrozen();
        newRoot.Right = node;
        return newRoot;
    }

    private static TNode InitLeaf<TKey, TValue, TNode>(this TNode node, TKey key, TValue value)
        where TNode : class, IBinaryTree<TKey, TValue, TNode>
        where TKey : notnull, IComparable<TKey>
    {
        node.Key = key;
        node.Value = value;
        node.Count = 1;
        node.Depth = 1;
        node.Left = null;
        node.Right = null;
        return node;
    }

    public static TNode? Remove<TKey, TValue, TNode>(this TNode? tree, TKey key)
        where TNode : class, IBinaryTree<TKey, TValue, TNode>
        where TKey : notnull, IComparable<TKey>
    {
        if (tree is null) return null;
        if (tree.Count == 0) return null;

        // shallow clone (unfreeze) if needed
        TNode result = tree.IsFrozen
            ? tree.PartCopy() as TNode ?? throw new InvalidOperationException("Failed to create unfrozen copy.")
            : tree;

        int comparison = result.Key.CompareTo(key);

        if (comparison == 0)
        {
            // remove key/value in this node
            if (result.Left is null)
            {
                if (result.Right is null)
                {
                    // leaf node
                    return null;
                }
                else
                {
                    result = result.Right;
                }
            }
            else
            {
                if (result.Right is null)
                {
                    result = result.Left;
                }
                else
                {
                    // two children, find inorder successor (smallest in the right subtree)
                    var successor = result.Right;
                    while (successor.Left is not null)
                    {
                        successor = successor.Left;
                    }
                    // copy successor's key/value to this node
                    result.Key = successor.Key;
                    result.Value = successor.Value;
                    // remove successor node from right subtree
                    result.Right = result.Right.Remove<TKey, TValue, TNode>(successor.Key);
                }
            }
        }
        else if (comparison > 0)
        {
            // go left
            if (result.Left is null)
            {
                // key not found
            }
            else
            {
                result.Left = result.Left.Remove<TKey, TValue, TNode>(key);
            }
        }
        else
        {
            // go right
            if (result.Right is null)
            {
                // key not found
            }
            else
            {
                result.Right = result.Right.Remove<TKey, TValue, TNode>(key);
            }
        }

        // rebalance if needed
        bool rotated = false;
        // doco: https://en.wikipedia.org/wiki/Tree_rotation
        int balance = result.GetBalance<TKey, TValue, TNode>();
        if (result.Left is not null && balance > 1)
        {
            // left-heavy, perform right rotation
            result = result.RotateRight<TKey, TValue, TNode>();
            rotated = true;
        }
        else if (result.Right is not null && balance < -1)
        {
            // right-heavy, perform left rotation
            result = result.RotateLeft<TKey, TValue, TNode>();
            rotated = true;
        }

        if (rotated)
        {
            // recalc count/depth for children
            result.Left?.TrySetCountAndDepth<TKey, TValue, TNode>();
            result.Right?.TrySetCountAndDepth<TKey, TValue, TNode>();
        }

        result.TrySetCountAndDepth<TKey, TValue, TNode>();
        return result;
    }

    public static TNode AddOrUpdate<TKey, TValue, TNode>(this TNode? tree, TKey key, TValue value)
        where TNode : class, IBinaryTree<TKey, TValue, TNode>, new()
        where TKey : notnull, IComparable<TKey>
    {
        if (tree is null) return new TNode().InitLeaf(key, value);

        TNode result = tree.IsFrozen
            ? tree.PartCopy() as TNode ?? throw new InvalidOperationException("Failed to create unfrozen copy.")
            : tree;

        if (tree.Count == 0) return tree.InitLeaf(key, value);

        int comparison = result.Key.CompareTo(key);
        if (comparison == 0)
        {
            // update value only
            result.Value = value;
            return result;
        }
        else if (comparison > 0)
        {
            // go left
            var left = result.Left;
            result.Left = left is null
                ? new TNode().InitLeaf(key, value)
                : left.AddOrUpdate(key, value);
        }
        else
        {
            // go right
            var right = result.Right;
            result.Right = right is null
                ? new TNode().InitLeaf(key, value)
                : right.AddOrUpdate(key, value);
        }

        // rebalance if needed
        bool rotated = false;
        // doco: https://en.wikipedia.org/wiki/Tree_rotation
        int balance = result.GetBalance<TKey, TValue, TNode>();
        if (result.Left is not null && balance > 1)
        {
            // left-heavy, perform right rotation
            result = result.RotateRight<TKey, TValue, TNode>();
            rotated = true;
        }
        else if (result.Right is not null && balance < -1)
        {
            // right-heavy, perform left rotation
            result = result.RotateLeft<TKey, TValue, TNode>();
            rotated = true;
        }

        if (rotated)
        {
            // recalc count/depth for children
            result.Left?.TrySetCountAndDepth<TKey, TValue, TNode>();
            result.Right?.TrySetCountAndDepth<TKey, TValue, TNode>();
        }

        result.TrySetCountAndDepth<TKey, TValue, TNode>();
        return result;
    }
}
