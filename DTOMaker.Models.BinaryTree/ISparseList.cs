using System;
using System.Collections;
using System.Collections.Generic;

namespace DTOMaker.Models.BinaryTree;

public interface ISparseList<T> : IEntityBase
{
    int Count { get; }
}

/// <summary>
/// A leaf node of the sparse list.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface ISparseListItems<T> : ISparseList<T>
{
    T GetItem(int index);
}

/// <summary>
/// The root node or an internal node in the sparse list.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface ISparseListNode<T> : ISparseList<T>
{
    int Trees { get; set; }
    ISparseList<T> GetTree(int index);
}

public static class SparseListExtensions
{
    public static IEnumerable<TValue> GetValues<TValue>(this ISparseList<TValue>? tree, bool reverse = false)
    {
        if (tree is null) yield break;
        if (tree is ISparseListItems<TValue> leaf)
        {
            if (reverse)
            {
                for (int i = leaf.Count - 1; i >= 0; i--)
                {
                    yield return leaf.GetItem(i);
                }
            }
            else
            {
                for (int i = 0; i < leaf.Count; i++)
                {
                    yield return leaf.GetItem(i);
                }
            }
        }
        else if (tree is ISparseListNode<TValue> node)
        {
            if (reverse)
            {
                for (int i = node.Trees - 1; i >= 0; i--)
                {
                    foreach (var value in node.GetTree(i).GetValues<TValue>(true))
                    {
                        yield return value;
                    }
                }
            }
            else
            {
                for (int i = 0; i < node.Trees; i++)
                {
                    foreach (var value in node.GetTree(i).GetValues<TValue>(false))
                    {
                        yield return value;
                    }
                }
            }
        }
        else
        {
            throw new ArgumentException($"Unexpected tree type: {tree.GetType().FullName}", nameof(tree));
        }
    }
}

public readonly struct ListFacade<TValue, TNode> : IList<TValue>
    where TNode : class, ISparseList<TValue>
    where TValue : IEquatable<TValue>
{
    private readonly Func<TNode?> _getter;
    private readonly Func<TNode> _newLeaf;
    private readonly Func<TNode> _newNode;

    public ListFacade(Func<TNode?> getter, Func<TNode> newLeaf, Func<TNode> newNode) //, Action<TNode?> setter)
    {
        _getter = getter;
        _newLeaf = newLeaf;
        _newNode = newNode;
    }

    // ------------------------------ readonly methods --------------------------------------------------
    public TValue this[int index]
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    public int Count => _getter()?.Count ?? 0;

    public bool IsReadOnly => false;

    public IEnumerator<TValue> GetEnumerator() => _getter().GetValues<TValue>().GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public bool Contains(TValue item)
    {
        var values = _getter().GetValues<TValue>();
        if (item is null)
        {
            foreach (var value in values)
            {
                if (value is null) return true;
            }
        }
        else
        {
            foreach (var value in values)
            {
                if (ReferenceEquals(value, item)) return true;
                if (value is not null && value.Equals(item)) return true;
            }
        }
        return false;
    }

    public void CopyTo(TValue[] array, int arrayIndex)
    {
        int count = 0;
        var values = _getter().GetValues<TValue>();
        foreach (var value in values)
        {
            array[arrayIndex + count] = value;
            count++;
        }
    }

    public int IndexOf(TValue item)
    {
        int index = 0;
        var values = _getter().GetValues<TValue>(false);
        if (item is null)
        {
            foreach (var value in values)
            {
                if (value is null) return index;
                index++;
            }
        }
        else
        {
            foreach (var value in values)
            {
                if (ReferenceEquals(value, item)) return index;
                if (value is not null && value.Equals(item)) return index;
                index++;
            }
        }
        return -1;
    }

    // ------------------------------ mutating methods --------------------------------------------------
    public void Add(TValue item)
    {
        throw new NotImplementedException();
    }

    public void Clear()
    {
        throw new NotImplementedException();
    }

    public void Insert(int index, TValue item)
    {
        throw new NotImplementedException();
    }

    public bool Remove(TValue item)
    {
        throw new NotImplementedException();
    }

    public void RemoveAt(int index)
    {
        throw new NotImplementedException();
    }
}

