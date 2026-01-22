using System;
using System.Collections.Generic;

namespace DTOMaker.Models.BinaryTree;

/// <summary>
/// Provides extension methods for working with data structures that implement IBinaryTree<TKey, TValue>.
/// </summary>
/// <remarks>
/// These methods enable common operations such as searching, enumeration, addition, and removal
/// of nodes in a binary tree. When addition or removal operations are performed tree balance is maintained via
/// node rotatations so that the depth difference between left and right subtrees is never more than 1.
/// </remarks>
public static class IBinaryTreeExtensions
{
    public static IBinaryTree<TKey, TValue>? Get<TKey, TValue>(this IBinaryTree<TKey, TValue> tree, TKey key)
        where TKey : notnull, IComparable<TKey>
    {
        if (tree is null) return null;
        int comparison = tree.Key.CompareTo(key);
        if (comparison == 0)
        {
            return tree; // found
        }
        else if (comparison > 0)
        {
            // go left
            return tree.Left?.Get(key);
        }
        else
        {
            // go right
            return tree.Right?.Get(key);
        }
    }

    public static IEnumerable<KeyValuePair<TKey, TValue>> GetKeyValuePairs<TKey, TValue>(this IBinaryTree<TKey, TValue>? tree, bool reverse)
        where TKey : notnull, IComparable<TKey>
    {
        if (tree is null) yield break;
        if (reverse)
        {
            foreach (var kvp in tree.Right.GetKeyValuePairs(true)) yield return kvp;
            yield return new KeyValuePair<TKey, TValue>(tree.Key, tree.Value);
            foreach (var kvp in tree.Left.GetKeyValuePairs(true)) yield return kvp;
        }
        else
        {
            foreach (var kvp in tree.Left.GetKeyValuePairs(false)) yield return kvp;
            yield return new KeyValuePair<TKey, TValue>(tree.Key, tree.Value);
            foreach (var kvp in tree.Right.GetKeyValuePairs(false)) yield return kvp;

        }
    }

    private static int GetCount<TKey, TValue>(this IBinaryTree<TKey, TValue>? tree)
    {
        if (tree is null) return 0;
        if (tree.IsFrozen) return tree.Count;
        return 1 + tree.Left.GetCount() + tree.Right.GetCount();
    }

    private static byte GetDepth<TKey, TValue>(this IBinaryTree<TKey, TValue>? tree)
    {
        if (tree is null) return 0;
        if (tree.IsFrozen) return tree.Depth;
        return (byte)(1 + Math.Max(tree.Left.GetDepth(), tree.Right.GetDepth()));
    }

    private static int GetBalance<TKey, TValue>(this IBinaryTree<TKey, TValue>? tree)
    {
        if (tree is null) return 0;
        return tree.Left.GetDepth() - tree.Right.GetDepth();
    }

    private static void TrySetCountAndDepth<TKey, TValue>(this IBinaryTree<TKey, TValue>? tree)
    {
        if (tree is null) return;
        if (tree.IsFrozen) return;
        tree.Count = 1 + tree.Left.GetCount() + tree.Right.GetCount();
        tree.Depth = (byte)(1 + Math.Max(tree.Left.GetDepth(), tree.Right.GetDepth()));
    }

    private static IBinaryTree<TKey, TValue> RotateLeft<TKey, TValue>(this IBinaryTree<TKey, TValue> node)
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

    private static IBinaryTree<TKey, TValue> RotateRight<TKey, TValue>(this IBinaryTree<TKey, TValue> node)
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

    private static IBinaryTree<TKey, TValue> InitLeaf<TKey, TValue>(this IBinaryTree<TKey, TValue> node, TKey key, TValue value)
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

    public static IBinaryTree<TKey, TValue>? Remove<TKey, TValue>(this IBinaryTree<TKey, TValue>? tree, TKey key)
        where TKey : notnull, IComparable<TKey>
    {
        if (tree is null) return null;

        // shallow clone (unfreeze) if needed
        IBinaryTree<TKey, TValue> result = tree.IsFrozen
            ? tree.PartCopy() as IBinaryTree<TKey, TValue> ?? throw new InvalidOperationException("Failed to create unfrozen copy.")
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
                    result.Right = result.Right.Remove(successor.Key);
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
                result.Left = result.Left.Remove(key);
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
                result.Right = result.Right.Remove(key);
            }
        }

        // rebalance if needed
        bool rotated = false;
        // doco: https://en.wikipedia.org/wiki/Tree_rotation
        int balance = result.GetBalance();
        if (result.Left is not null && balance > 1)
        {
            // left-heavy, perform right rotation
            result = result.RotateRight();
            rotated = true;
        }
        else if (result.Right is not null && balance < -1)
        {
            // right-heavy, perform left rotation
            result = result.RotateLeft();
            rotated = true;
        }

        if (rotated)
        {
            // recalc count/depth for children
            result.Left?.TrySetCountAndDepth();
            result.Right?.TrySetCountAndDepth();
        }

        result.TrySetCountAndDepth();
        return result;
    }

    public static IBinaryTree<TKey, TValue> AddOrUpdate<TKey, TValue>(this IBinaryTree<TKey, TValue>? tree, TKey key, TValue value,
        Func<IBinaryTree<TKey, TValue>> newNodeFn)
        where TKey : notnull, IComparable<TKey>
    {
        if (tree is null) return newNodeFn().InitLeaf(key, value);

        IBinaryTree<TKey, TValue> result = tree.IsFrozen
            ? tree.PartCopy() as IBinaryTree<TKey, TValue> ?? throw new InvalidOperationException("Failed to create unfrozen copy.")
            : tree;

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
                ? newNodeFn().InitLeaf(key, value)
                : left.AddOrUpdate(key, value, newNodeFn);
        }
        else
        {
            // go right
            var right = result.Right;
            result.Right = right is null
                ? newNodeFn().InitLeaf(key, value)
                : right.AddOrUpdate(key, value, newNodeFn);
        }

        // rebalance if needed
        bool rotated = false;
        // doco: https://en.wikipedia.org/wiki/Tree_rotation
        int balance = result.GetBalance();
        if (result.Left is not null && balance > 1)
        {
            // left-heavy, perform right rotation
            result = result.RotateRight();
            rotated = true;
        }
        else if (result.Right is not null && balance < -1)
        {
            // right-heavy, perform left rotation
            result = result.RotateLeft();
            rotated = true;
        }

        if (rotated)
        {
            // recalc count/depth for children
            result.Left?.TrySetCountAndDepth();
            result.Right?.TrySetCountAndDepth();
        }

        result.TrySetCountAndDepth();
        return result;
    }
}
