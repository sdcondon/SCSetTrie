// Copyright © 2023 Simon Condon.
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
/// value semantics for its hash code. (NB coping with collisions is a TODO for v1).
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

    /// <summary>
    /// Adds a set and associated value to the trie.
    /// </summary>
    /// <param name="key">The set to add.</param>
    /// <param name="value">The value to associate with the set.</param>
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
        var keyElements = key.ToArray();
        Array.Sort(keyElements, elementComparer);

        return ExpandNode(root, 0);
        
        IEnumerable<TValue> ExpandNode(ISetTrieNode<TKeyElement, TValue> node, int keyElementIndex)
        {
            if (keyElementIndex >= keyElements.Length)
            {
                if (node.HasValue)
                {
                    yield return node.Value;
                }

                yield break;
            }

            if (node.Children.TryGetValue(keyElements[keyElementIndex], out var childNode))
            {
                foreach (var value in ExpandNode(childNode, keyElementIndex + 1))
                {
                    yield return value;
                }
            }

            foreach (var value in ExpandNode(node, keyElementIndex + 1))
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
        var keyElements = key.ToArray();
        Array.Sort(keyElements, elementComparer);

        return ExpandNode(root, 0);

        IEnumerable<TValue> ExpandNode(ISetTrieNode<TKeyElement, TValue> node, int keyElementIndex)
        {
            if (keyElementIndex >= keyElements.Length)
            {
                foreach (var value in GetAllDescendentValues(node))
                {
                    yield return value;
                }

                yield break;
            }

            var lastKeyElement = keyElementIndex == 0 ? default : keyElements[keyElementIndex - 1];
            var currentKeyElement = keyElements[keyElementIndex];
            foreach (var (childKeyElement, childNode) in node.Children)
            {
                if (keyElementIndex == 0 || elementComparer.Compare(childKeyElement, lastKeyElement) > 0)
                {
                    var childComparedToCurrent = elementComparer.Compare(childKeyElement, currentKeyElement);
                    if (childComparedToCurrent <= 0)
                    {
                        var keyElementIndexOffset = childComparedToCurrent == 0 ? 1 : 0; 
                        foreach (var value in ExpandNode(childNode, keyElementIndex + keyElementIndexOffset))
                        {
                            yield return value;
                        }
                    }
                }
            }
        }

        IEnumerable<TValue> GetAllDescendentValues(ISetTrieNode<TKeyElement, TValue> node)
        {
            if (node.HasValue)
            {
                yield return node.Value;
            }

            foreach (var childNode in node.Children.Values)
            {
                foreach (var value in GetAllDescendentValues(childNode))
                {
                    yield return value;
                }
            }
        }
    }

    private class HashCodeComparer<T> : IComparer<T>
    {
        private static readonly IComparer<int> intComparer = Comparer<int>.Default;

        public int Compare(T? x, T? y) => intComparer.Compare(x.GetHashCode(), y.GetHashCode());
    }
}
