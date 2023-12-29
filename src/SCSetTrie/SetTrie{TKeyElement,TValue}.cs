﻿// Copyright © 2023 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace SCSetTrie;

/// <summary>
/// <para>
/// A set trie implementation, for the storage of sets (with associated values) in a
/// manner facilitating fast lookup of (values associated with) subsets and supersets
/// of a query set.
/// </para>
/// <para>
/// NB: set elements are ordered as they are added to the trie. The default ordering
/// is by hash code - so give serious consideration to using a type that implements 
/// value semantics for its hash code. (Allowing the specification of a comparer to
/// use is a TODO - as is coping with collisions).
/// </para>
/// </summary>
/// <typeparam name="TKeyElement">The type of each element of the stored sets.</typeparam>
/// <typeparam name="TValue">The type of the value associated with each stored set.</typeparam>
// TODO: need to handle ordering collisions.
// probably by fundamentally allowing for this (nodes can contain more than one element?).
public class SetTrie<TKeyElement,TValue>
    where TKeyElement : notnull
{
    private static readonly IEnumerable<KeyValuePair<ISet<TKeyElement>, TValue>> EmptyElements = Enumerable.Empty<KeyValuePair<ISet<TKeyElement>, TValue>>();
    private static readonly HashCodeComparer<TKeyElement> DefaultElementComparer = new();

    private readonly ISetTrieNode<TKeyElement, TValue> root;
    private readonly IComparer<TKeyElement> elementComparer;

    /// <summary>
    /// Initializes a new instance of the <see cref="SetTrie{TKeyElement,TValue}"/> class with a new 
    /// <see cref="SetTrieDictionaryNode{TKeyElement,TValue}"/> root node and no initial content, that
    /// uses hash code to determine the ordering of elements in the tree.
    /// </summary>
    public SetTrie()
        : this(new SetTrieDictionaryNode<TKeyElement, TValue>(), DefaultElementComparer, EmptyElements)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SetTrie{TKeyElement,TValue}"/> class with a specified
    /// root node and no (additional) initial content, that uses hash code to determine the ordering of elements
    /// in the tree.
    /// </summary>
    /// <param name="root">The root node of the tree.</param>
    public SetTrie(ISetTrieNode<TKeyElement, TValue> root)
        : this(root, DefaultElementComparer, EmptyElements)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SetTrie{TKeyElement,TValue}"/> class with a new 
    /// <see cref="SetTrieDictionaryNode{TKeyElement,TValue}"/> root node and no initial content, that
    /// uses a specified comparer to determine the ordering of elements in the tree.
    /// </summary>
    /// <param name="elementComparer">the comparer to use to determine the ordering of elements in the tree.</param>
    public SetTrie(IComparer<TKeyElement> elementComparer)
        : this(new SetTrieDictionaryNode<TKeyElement, TValue>(), elementComparer, EmptyElements)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SetTrie{TKeyElement,TValue}"/> class with a new <see cref="SetTrieDictionaryNode{TKeyElement,TValue}"/> root node and some initial content.
    /// </summary>
    /// <param name="content">The initial content to be added to the tree.</param>
    public SetTrie(IEnumerable<KeyValuePair<ISet<TKeyElement>, TValue>> content)
        : this(new SetTrieDictionaryNode<TKeyElement, TValue>(), DefaultElementComparer, content)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SetTrie{TKeyElement,TValue}"/> class with a 
    /// specified root node and no (additional) initial content, that uses a specified comparer to
    /// determine the ordering of elements in the tree.
    /// </summary>
    /// <param name="root">The root node of the tree.</param>
    /// <param name="elementComparer">the comparer to use to determine the ordering of elements in the tree.</param>
    public SetTrie(ISetTrieNode<TKeyElement, TValue> root, IComparer<TKeyElement> elementComparer)
        : this(root, elementComparer, EmptyElements)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SetTrie{TKeyElement,TValue}"/> class with a 
    /// specified root node and some (additional) initial content, that uses hash code to determine
    /// the ordering of elements in the tree.
    /// </summary>
    /// <param name="root">The root node of the tree.</param>
    /// <param name="content">The (additional) content to be added to the tree (beyond any already attached to the provided root node).</param>
    public SetTrie(ISetTrieNode<TKeyElement, TValue> root, IEnumerable<KeyValuePair<ISet<TKeyElement>, TValue>> content)
        : this(root, DefaultElementComparer, content)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SetTrie{TKeyElement,TValue}"/> class with a new 
    /// <see cref="SetTrieDictionaryNode{TKeyElement,TValue}"/> root node and some (additional) initial
    /// content, that uses hash code to determine the ordering of elements in the tree.
    /// </summary>
    /// <param name="elementComparer">the comparer to use to determine the ordering of elements in the tree.</param>
    /// <param name="content">The (additional) content to be added to the tree (beyond any already attached to the provided root node).</param>
    public SetTrie(IComparer<TKeyElement> elementComparer, IEnumerable<KeyValuePair<ISet<TKeyElement>, TValue>> content)
        : this(new SetTrieDictionaryNode<TKeyElement, TValue>(), elementComparer, content)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SetTrie{TKeyElement,TValue}"/> class with a 
    /// specified root node and some (additional) initial content, that a specified comparer to
    /// determine the ordering of elements in the tree.
    /// </summary>
    /// <param name="root">The root node of the tree.</param>
    /// <param name="elementComparer">the comparer to use to determine the ordering of elements in the tree.</param>
    /// <param name="content">The (additional) content to be added to the tree (beyond any already attached to the provided root node).</param>
    public SetTrie(ISetTrieNode<TKeyElement, TValue> root, IComparer<TKeyElement> elementComparer, IEnumerable<KeyValuePair<ISet<TKeyElement>, TValue>> content)
    {
        ArgumentNullException.ThrowIfNull(root);
        ArgumentNullException.ThrowIfNull(elementComparer);
        ArgumentNullException.ThrowIfNull(content);

        this.root = root;
        this.elementComparer = elementComparer;

        foreach (var kvp in content)
        {
            Add(kvp.Key, kvp.Value);
        }
    }

    public void Add(ISet<TKeyElement> key, TValue value)
    {
        var currentNode = root;
        foreach (var keyElement in key.OrderBy(k => k, elementComparer))
        {
            currentNode = currentNode.GetOrAddChildNode(keyElement);
        }

        currentNode.AddValue(value);
    }

    /// <summary>
    /// Attempts to retrieve the value associated with a set.
    /// </summary>
    /// <param name="key">The set to retrieve the associated value of.</param>
    /// <param name="value">Will be populated with the retrieved value.</param>
    /// <returns>True if and only if a value was successfully retrieved.</returns>
    public bool TryGet(ISet<TKeyElement> key, [MaybeNullWhen(false)] out TValue value)
    {
        var currentNode = root;
        foreach (var keyElement in key.OrderBy(k => k, elementComparer))
        {
            if (currentNode.Children.TryGetValue(keyElement, out var childNode))
            {
                currentNode = childNode;
            }
            else
            {
                value = default;
                return false;
            }
        }

        if (currentNode.HasValue)
        {
            value = currentNode.Value;
            return true;
        }

        value = default;
        return false;
    }

    /// <summary>
    /// Returns an enumerable of the values associated with each stored subset of a given set.
    /// </summary>
    /// <param name="key"></param>
    /// <returns>An enumerable of the values associated with each stored subset of the given set.</returns>
    public IEnumerable<TValue> GetSubsets(ISet<TKeyElement> key)
    {
        using var keyEnumerator = key.OrderBy(k => k, elementComparer).GetEnumerator();
        return ExpandNode(root);
        
        IEnumerable<TValue> ExpandNode(ISetTrieNode<TKeyElement, TValue> node)
        {
            if (node.HasValue)
            {
                yield return node.Value;
            }

            if (!keyEnumerator.MoveNext())
            {
                yield break;
            }

            if (node.Children.TryGetValue(keyEnumerator.Current, out var childNode))
            {
                node = childNode;
            }

            foreach (var value in ExpandNode(node))
            {
                yield return value;
            }
        }
    }

    /// <summary>
    /// Returns an enumerable of the values associated with each stored superset a given set.
    /// </summary>
    /// <param name="key"></param>
    /// <returns>An enumerable of the values associated with each stored superset a given set.</returns>
    public IEnumerable<TValue> GetSupersets(ISet<TKeyElement> key)
    {
        throw new NotImplementedException();

        ////using var keyEnumerator = key.OrderBy(k => k, elementComparer).GetEnumerator();
        ////return ExpandNode(root);

        ////IEnumerable<TValue> ExpandNode(ISetTrieNode<TKeyElement, TValue> node)
        ////{
        ////    if (!node.HasValue)
        ////    {
        ////        //TODO return values from self and all descendents
        ////    }

        ////    //..
        ////}
    }

    private class HashCodeComparer<T> : IComparer<T>
    {
        private static readonly IComparer<int> intComparer = Comparer<int>.Default;

        public int Compare(T? x, T? y) => intComparer.Compare(x.GetHashCode(), y.GetHashCode());
    }
}
