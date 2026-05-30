using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DTOMaker.Models.BinaryTree;

public readonly struct DictionaryFacade<TKey, TValue, TNode> : IDictionary<TKey, TValue>
    where TKey : notnull, IComparable<TKey>
    where TNode : class, IBinaryTree<TKey, TValue, TNode>, new()
{
    private readonly Func<TNode?> _getter;
    private readonly Action<TNode?> _setter;

    public DictionaryFacade(Func<TNode?> getter, Action<TNode?> setter)
    {
        _getter = getter;
        _setter = setter;
    }

    // ------------------------------ readonly methods --------------------------------------------------

    public bool IsReadOnly => false;
    public IEnumerable<TKey> Keys => _getter().GetKeyValuePairs<TKey, TValue, TNode>().Select(kv => kv.Key);
    public IEnumerable<TValue> Values => _getter().GetKeyValuePairs<TKey, TValue, TNode>().Select(kv => kv.Value);
    public int Count => _getter()?.Count ?? 0;

    /// <summary>
    /// Returns all keys as a new list. Avoid using this as it will allocate a new list every time.
    /// </summary>
    ICollection<TKey> IDictionary<TKey, TValue>.Keys => _getter().GetKeyValuePairs<TKey, TValue, TNode>().Select(kv => kv.Key).ToList();

    /// <summary>
    /// Returns all values as a new list. Avoid using this as it will allocate a new list every time.
    /// </summary>
    ICollection<TValue> IDictionary<TKey, TValue>.Values => _getter().GetKeyValuePairs<TKey, TValue, TNode>().Select(kv => kv.Value).ToList();
    public bool ContainsKey(TKey key) => _getter().Get<TKey, TValue, TNode>(key) is not null;
    public bool Contains(KeyValuePair<TKey, TValue> item) => TryGetValue(item.Key, out var value) && Equals(value, item.Value);
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _getter().GetKeyValuePairs<TKey, TValue, TNode>().GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public bool TryGetValue(TKey key, out TValue value)
    {
        var node = _getter().Get<TKey, TValue, TNode>(key);
        value = node is not null ? node.Value : default!;
        return node is not null;
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        int count = 0;
        var pairs = _getter().GetKeyValuePairs<TKey, TValue, TNode>();
        foreach (var kvp in pairs)
        {
            array[arrayIndex + count] = kvp;
            count++;
        }
    }

    public TValue this[TKey key]
    {
        get
        {
            TNode? node = _getter().Get<TKey, TValue, TNode>(key);
            return (node is not null ? node.Value : default) ?? throw new KeyNotFoundException($"Key not found: {key}");
        }

        // ------------------------------ mutating methods --------------------------------------------------

        set => _setter(_getter().Get<TKey, TValue, TNode>(key).AddOrUpdate(key, value));
    }

    public void Clear() => _setter(null);

    public void Add(TKey key, TValue value)
    {
        var root = _getter();
        // todo? optimise this with an AddOrThrow method in IBinaryTree
        if (root.Get<TKey, TValue, TNode>(key) is not null) throw new ArgumentException("An item with the same key has already been added.");
        root = root.AddOrUpdate(key, value);
        _setter(root);
    }

    public void Add(KeyValuePair<TKey, TValue> item) => Add(item.Key, item.Value);

    public bool Remove(TKey key)
    {
        var root = _getter();
        if (root.Get<TKey, TValue, TNode>(key) is null) return false;
        _setter(root.Remove<TKey, TValue, TNode>(key));
        return true;
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        if (!TryGetValue(item.Key, out var value) || !Equals(value, item.Value)) return false;

        var root = _getter();
        _setter(root.Remove<TKey, TValue, TNode>(item.Key));
        return true;
    }

}