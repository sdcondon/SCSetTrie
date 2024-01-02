// Copyright © 2023 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System;
using System.Collections.Generic;

namespace SCSetTrie;

/// <summary>
/// An implementation of <see cref="ISetTrieNode{TKeyElement, TValue}"/> that just stores things in memory. 
/// Uses a <see cref="Dictionary{TKey, TValue}"/> for the children of a node.
/// </summary>
/// <typeparam name="TKeyElement">The type of each element of the stored sets.</typeparam>
/// <typeparam name="TValue">The type of the value associated with each stored set.</typeparam>
public class SetTrieDictionaryNode<TKeyElement, TValue> : ISetTrieNode<TKeyElement, TValue>
    where TKeyElement : notnull
{
    private readonly Dictionary<TKeyElement, ISetTrieNode<TKeyElement, TValue>> children;
    private TValue? value;

    /// <summary>
    /// Initialises a new instance of the <see cref="SetTrieDictionaryNode{TKeyElement, TValue}"/> class.
    /// </summary>
    public SetTrieDictionaryNode()
        : this(EqualityComparer<TKeyElement>.Default)
    {
    }

    /// <summary>
    /// Initialises a new instance of the <see cref="SetTrieDictionaryNode{TKeyElement, TValue}"/> class.
    /// </summary>
    /// <param name="equalityComparer">
    /// The equality comparer that should be used by the child dictionary.
    /// For correct behaviour, trie instances accessing this node should be using an <see cref="IComparer{T}"/> that is consistent with it. 
    /// That is, one that only returns zero for elements considered equal by equality comparer used by this instance.
    /// </param>
    public SetTrieDictionaryNode(IEqualityComparer<TKeyElement> equalityComparer)
    {
        children = new(equalityComparer);
    }

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
