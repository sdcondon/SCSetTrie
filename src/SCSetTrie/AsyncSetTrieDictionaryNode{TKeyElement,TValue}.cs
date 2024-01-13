// Copyright © 2023 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SCSetTrie;

#pragma warning disable CS1998 // async lacks await. See 'NB' in class summary.
/// <summary>
/// <para>
/// An implementation of <see cref="IAsyncSetTrieNode{TKeyElement, TValue}"/> that just stores its content in memory.
/// Uses a <see cref="ConcurrentDictionary{TKey, TValue}"/> for child nodes.
/// </para>
/// <para>
/// NB: If you are using this type, you should consider just using <see cref="SetTrie{TKeyElement, TValue}"/> to avoid the overhead of asynchronicity.
/// <see cref="AsyncSetTrie{TKeyElement, TValue}"/> is intended to facilitate tries that use secondary storage - this type is primarily
/// intended as an example implementation to base real (secondary storage utilising) implementations on.
/// </para>
/// </summary>
/// <typeparam name="TKeyElement">The type of each element of the stored sets.</typeparam>
/// <typeparam name="TValue">The type of the value associated with each stored set.</typeparam>
public class AsyncSetTrieDictionaryNode<TKeyElement, TValue> : IAsyncSetTrieNode<TKeyElement, TValue>
    where TKeyElement : notnull
{
    private readonly ConcurrentDictionary<TKeyElement, IAsyncSetTrieNode<TKeyElement, TValue>> children;
    private TValue? value;

    /// <summary>
    /// Initialises a new instance of the <see cref="AsyncSetTrieDictionaryNode{TKeyElement, TValue}"/> class.
    /// </summary>
    public AsyncSetTrieDictionaryNode()
        : this(EqualityComparer<TKeyElement>.Default)
    {
    }

    /// <summary>
    /// Initialises a new instance of the <see cref="AsyncSetTrieDictionaryNode{TKeyElement, TValue}"/> class.
    /// </summary>
    /// <param name="equalityComparer">
    /// The equality comparer that should be used by the child dictionary.
    /// For correct behaviour, trie instances accessing this node should be using an <see cref="IComparer{T}"/> that is consistent with it. 
    /// That is, one that only returns zero for elements considered equal by equality comparer used by this instance.
    /// </param>
    public AsyncSetTrieDictionaryNode(IEqualityComparer<TKeyElement> equalityComparer)
    {
        children = new(equalityComparer);
    }

    /// <inheritdoc/>
    public bool HasValue { get; private set; }

    /// <inheritdoc/>
    public TValue Value => HasValue ? value! : throw new InvalidOperationException("Node has no attached value");

    /// <inheritdoc/>
    public async IAsyncEnumerable<KeyValuePair<TKeyElement, IAsyncSetTrieNode<TKeyElement, TValue>>> GetChildren()
    {
        foreach (var kvp in children)
        {
            yield return kvp;
        }
    }

    /// <inheritdoc/>
    public ValueTask<IAsyncSetTrieNode<TKeyElement, TValue>?> TryGetChildAsync(TKeyElement keyElement)
    {
        children.TryGetValue(keyElement, out var child);
        return ValueTask.FromResult(child);
    }

    /// <inheritdoc/>
    public ValueTask<IAsyncSetTrieNode<TKeyElement, TValue>> GetOrAddChildAsync(TKeyElement keyElement)
    {
        IAsyncSetTrieNode<TKeyElement, TValue> node = new AsyncSetTrieDictionaryNode<TKeyElement, TValue>();
        if (!children.TryAdd(keyElement, node))
        {
            node = children[keyElement];
        }

        return ValueTask.FromResult(node);
    }

    /// <inheritdoc/>
    public ValueTask DeleteChildAsync(TKeyElement keyElement)
    {
        children.Remove(keyElement, out _);
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc/>
    public ValueTask AddValueAsync(TValue value)
    {
        if (HasValue)
        {
            throw new InvalidOperationException("A value is already stored against this node");
        }

        this.value = value;
        HasValue = true;

        return ValueTask.CompletedTask;
    }

    /// <inheritdoc/>
    public ValueTask RemoveValueAsync()
    {
        value = default;
        HasValue = false;

        return ValueTask.CompletedTask;
    }
}
