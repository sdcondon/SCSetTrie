// Copyright © 2023 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System;
using System.Collections.Generic;

namespace SCSetTrie;

/// <summary>
/// An implementation of <see cref="ISetTrieNode{TKeyElement, TValue}"/> that just stores things
/// in memory, using a <see cref="Dictionary{TKey, TValue}"/> for the children of a node.
/// </summary>
/// <typeparam name="TKeyElement">The type of each element of the stored sets.</typeparam>
/// <typeparam name="TValue">The type of the value associated with each stored set.</typeparam>
public class SetTrieDictionaryNode<TKeyElement, TValue> : ISetTrieNode<TKeyElement, TValue>
    where TKeyElement : notnull
{
    private readonly Dictionary<TKeyElement, ISetTrieNode<TKeyElement, TValue>> children = new();
    private TValue? value;

    /// <inheritdoc/>
    // NB: we don't bother wrapping children in a ReadOnlyDict to stop unscrupulous
    // users from casting. Would be more memory for a real edge case.
    public IReadOnlyDictionary<TKeyElement, ISetTrieNode<TKeyElement, TValue>> Children => children;

    /// <inheritdoc/>
    public bool HasValue { get; private set; }

    /// <inheritdoc/>
    public TValue Value => HasValue ? value! : throw new InvalidOperationException("Node has no attached value");

    /// <inheritdoc/>
    public ISetTrieNode<TKeyElement, TValue> GetOrAddChildNode(TKeyElement keyElement)
    {
        if (!children.TryGetValue(keyElement, out var node))
        {
            node = new SetTrieDictionaryNode<TKeyElement, TValue>();
            children.Add(keyElement, node);
        }

        return node;
    }

    /// <inheritdoc/>
    public void AddValue(TValue value)
    {
        // TODO: throw if already present
        this.value = value;
        HasValue = true;
    }
}
