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
    // TODO: For correct (or at least consistent) behaviour, the equality comparer used
    // by this dictionary should really be closely tied to the IComparer used by the trie (i.e.
    // equality in this dictionary should be exactly equivalent to the comparer returning 0).
    // How to achieve this, without overwhelming code smells? Can of course add ctor parameters
    // to this class, but what exactly should be added?
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
        if (HasValue)
        {
            throw new InvalidOperationException("A value is already stored against this node");
        }

        this.value = value;
        HasValue = true;
    }
}
