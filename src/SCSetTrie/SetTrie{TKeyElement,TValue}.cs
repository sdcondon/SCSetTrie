// Copyright © 2023 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace SCSetTrie;

/// <summary>
/// A set trie implementation, for the storage of sets (with associated values) in a
/// manner facilitating fast lookup of (values associated with) subsets and supersets
/// of a query set.
/// </summary>
/// <typeparam name="TKeyElement">The type of each element of the stored sets.</typeparam>
/// <typeparam name="TValue">The type of the value associated with each stored set.</typeparam>
// TODO: we don't actually use ISet<>, we only need IEnum<>, and probably should explicitly check for
// 0-comparisons anyway to validate unambiguous ordering from comparer.. Change interface to use IEnum<>?
public class SetTrie<TKeyElement,TValue>
    where TKeyElement : notnull
{
    private static readonly IEnumerable<KeyValuePair<ISet<TKeyElement>, TValue>> EmptyElements = Enumerable.Empty<KeyValuePair<ISet<TKeyElement>, TValue>>();

    private readonly ISetTrieNode<TKeyElement, TValue> root;
    private readonly IComparer<TKeyElement> elementComparer;

    /// <summary>
    /// Initializes a new instance of the <see cref="SetTrie{TKeyElement,TValue}"/> class with a new 
    /// <see cref="SetTrieDictionaryNode{TKeyElement,TValue}"/> root node and no initial content, that
    /// uses the default comparer of the key element type to determine the ordering of elements in the
    /// tree.
    /// </summary>
    public SetTrie()
        : this(Comparer<TKeyElement>.Default, new SetTrieDictionaryNode<TKeyElement, TValue>(), EmptyElements)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SetTrie{TKeyElement,TValue}"/> class with a new 
    /// <see cref="SetTrieDictionaryNode{TKeyElement,TValue}"/> root node and no initial content.
    /// </summary>
    /// <param name="elementComparer">
    /// The comparer to use to determine the ordering of elements when adding to tree and performing
    /// queries. NB: For correct behaviour, it MUST define a "less than or equal" relation on the set of
    /// elements that is "antisymmetric" - that is, the comparison can only return zero for equal elements.
    /// </param>
    public SetTrie(IComparer<TKeyElement> elementComparer)
        : this(elementComparer, new SetTrieDictionaryNode<TKeyElement, TValue>(), EmptyElements)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SetTrie{TKeyElement,TValue}"/> class with a specified
    /// root node and no (additional) initial content, that uses the default comparer of the key element
    /// type to determine the ordering of elements in the tree.
    /// </summary>
    /// <param name="root">The root node of the tree.</param>
    public SetTrie(ISetTrieNode<TKeyElement, TValue> root)
        : this(Comparer<TKeyElement>.Default, root, EmptyElements)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SetTrie{TKeyElement,TValue}"/> class with a new 
    /// <see cref="SetTrieDictionaryNode{TKeyElement,TValue}"/> root node and some initial content,
    /// that uses the default comparer of the key element type to determine the ordering of elements
    /// in the tree.
    /// </summary>
    /// <param name="content">The initial content to be added to the tree.</param>
    public SetTrie(IEnumerable<KeyValuePair<ISet<TKeyElement>, TValue>> content)
        : this(Comparer<TKeyElement>.Default, new SetTrieDictionaryNode<TKeyElement, TValue>(), content)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SetTrie{TKeyElement,TValue}"/> class with a 
    /// specified root node and no (additional) initial content.
    /// </summary>
    /// <param name="elementComparer">
    /// The comparer to use to determine the ordering of elements when adding to tree and performing
    /// queries. NB: For correct behaviour, MUST define a "less than or equal" relation on the set of
    /// elements that is "antisymmetric" - that is, the comparison can only return zero for equal elements.
    /// </param>
    /// <param name="root">The root node of the tree.</param>
    public SetTrie(IComparer<TKeyElement> elementComparer, ISetTrieNode<TKeyElement, TValue> root)
        : this(elementComparer, root, EmptyElements)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SetTrie{TKeyElement,TValue}"/> class with a new 
    /// <see cref="SetTrieDictionaryNode{TKeyElement,TValue}"/> root node and some (additional) initial
    /// content.
    /// </summary>
    /// <param name="elementComparer">
    /// The comparer to use to determine the ordering of elements when adding to tree and performing
    /// queries. NB: For correct behaviour, MUST define a "less than or equal" relation on the set of
    /// elements that is "antisymmetric" - that is, the comparison can only return zero for equal elements.
    /// </param>
    /// <param name="content">The (additional) content to be added to the tree (beyond any already attached to the provided root node).</param>
    public SetTrie(IComparer<TKeyElement> elementComparer, IEnumerable<KeyValuePair<ISet<TKeyElement>, TValue>> content)
        : this(elementComparer, new SetTrieDictionaryNode<TKeyElement, TValue>(), content)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SetTrie{TKeyElement,TValue}"/> class with a 
    /// specified root node and some (additional) initial content, that uses the default comparer
    /// of the key element type to determine the ordering of elements in the tree.
    /// </summary>
    /// <param name="root">The root node of the tree.</param>
    /// <param name="content">The (additional) content to be added to the tree (beyond any already attached to the provided root node).</param>
    public SetTrie(ISetTrieNode<TKeyElement, TValue> root, IEnumerable<KeyValuePair<ISet<TKeyElement>, TValue>> content)
        : this(Comparer<TKeyElement>.Default, root, content)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SetTrie{TKeyElement,TValue}"/> class with a 
    /// specified root node and some (additional) initial content.
    /// </summary>
    /// <param name="elementComparer">
    /// The comparer to use to determine the ordering of elements when adding to tree and performing
    /// queries. NB: For correct behaviour, MUST define a "less than or equal" relation on the set of
    /// elements that is "antisymmetric" - that is, the comparison can only return zero for equal elements.
    /// </param>
    /// <param name="root">The root node of the tree.</param>
    /// <param name="content">The (additional) content to be added to the tree (beyond any already attached to the provided root node).</param>
    public SetTrie(IComparer<TKeyElement> elementComparer, ISetTrieNode<TKeyElement, TValue> root, IEnumerable<KeyValuePair<ISet<TKeyElement>, TValue>> content)
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
        ArgumentNullException.ThrowIfNull(key);

        var currentNode = root;
        foreach (var keyElement in elementComparer.Sort(key))
        {
            currentNode = currentNode.GetOrAddChildNode(keyElement);
        }

        currentNode.AddValue(value);
    }

    /// <summary>
    /// Removes a set from the trie.
    /// </summary>
    /// <param name="key">The set to remove.</param>
    /// <returns>A value indicating whether the set was present prior to this operation.</returns>
    public bool Remove(ISet<TKeyElement> key)
    {
        ArgumentNullException.ThrowIfNull(key);

        var keyElements = elementComparer.Sort(key);
        return ExpandNode(root, 0);

        bool ExpandNode(ISetTrieNode<TKeyElement, TValue> node, int keyElementIndex)
        {
            if (keyElementIndex < keyElements.Length)
            {
                var keyElement = keyElements[keyElementIndex];

                if (!node.Children.TryGetValue(keyElement, out var childNode) || !ExpandNode(childNode, keyElementIndex + 1))
                {
                    return false;
                }

                if (childNode.Children.Count == 0 && !childNode.HasValue)
                {
                    node.DeleteChild(keyElement);
                }

                return true;
            }
            else
            {
                if (!node.HasValue)
                {
                    return false;
                }

                node.RemoveValue();
                return true;
            }
        }
    }

    /// <summary>
    /// Attempts to retrieve the value associated with a set, matched exactly.
    /// </summary>
    /// <param name="key">The set to retrieve the associated value of.</param>
    /// <param name="value">Will be populated with the retrieved value.</param>
    /// <returns>True if and only if a value was successfully retrieved.</returns>
    public bool TryGet(ISet<TKeyElement> key, [MaybeNullWhen(false)] out TValue value)
    {
        ArgumentNullException.ThrowIfNull(key);

        var currentNode = root;
        foreach (var keyElement in elementComparer.Sort(key))
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
    /// Retrieves the values associated with each stored subset of a given set.
    /// </summary>
    /// <param name="key">The values associated with the stored subsets of this set will be retrieved.</param>
    /// <returns>An enumerable of the values associated with each stored subset of the given set.</returns>
    public IEnumerable<TValue> GetSubsets(ISet<TKeyElement> key)
    {
        ArgumentNullException.ThrowIfNull(key);

        var keyElements = elementComparer.Sort(key);
        return ExpandNode(root, 0);
        
        IEnumerable<TValue> ExpandNode(ISetTrieNode<TKeyElement, TValue> node, int keyElementIndex)
        {
            if (keyElementIndex < keyElements.Length)
            {
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
            else
            {
                if (node.HasValue)
                {
                    yield return node.Value;
                }
            }
        }
    }

    /// <summary>
    /// Retrieves the values associated with each stored superset a given set.
    /// </summary>
    /// <param name="key">The values associated with the stored supersets of this set will be retrieved.</param>
    /// <returns>An enumerable of the values associated with each stored superset the given set.</returns>
    public IEnumerable<TValue> GetSupersets(ISet<TKeyElement> key)
    {
        ArgumentNullException.ThrowIfNull(key);

        var keyElements = elementComparer.Sort(key);
        return ExpandNode(root, 0);

        IEnumerable<TValue> ExpandNode(ISetTrieNode<TKeyElement, TValue> node, int keyElementIndex)
        {
            if (keyElementIndex < keyElements.Length)
            {
                foreach (var (childKeyElement, childNode) in node.Children)
                {
                    if (keyElementIndex == 0 || elementComparer.Compare(childKeyElement, keyElements[keyElementIndex - 1]) > 0)
                    {
                        var childComparedToCurrent = elementComparer.Compare(childKeyElement, keyElements[keyElementIndex]);

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
            else
            {
                foreach (var value in GetAllDescendentValues(node))
                {
                    yield return value;
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
}
