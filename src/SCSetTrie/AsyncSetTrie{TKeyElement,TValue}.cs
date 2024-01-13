// Copyright © 2023-2024 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
public class AsyncSetTrie<TKeyElement,TValue>
    where TKeyElement : notnull
{
    private static readonly IEnumerable<KeyValuePair<ISet<TKeyElement>, TValue>> EmptyElements = Enumerable.Empty<KeyValuePair<ISet<TKeyElement>, TValue>>();

    private readonly IAsyncSetTrieNode<TKeyElement, TValue> root;
    private readonly IComparer<TKeyElement> elementComparer;

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncSetTrie{TKeyElement,TValue}"/> class with a new 
    /// <see cref="AsyncSetTrieDictionaryNode{TKeyElement,TValue}"/> root node and no initial content, that
    /// uses the default comparer of the key element type to determine the ordering of elements in the
    /// tree.
    /// </summary>
    public AsyncSetTrie()
        : this(Comparer<TKeyElement>.Default, new AsyncSetTrieDictionaryNode<TKeyElement, TValue>(), EmptyElements)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncSetTrie{TKeyElement,TValue}"/> class with a new 
    /// <see cref="AsyncSetTrieDictionaryNode{TKeyElement,TValue}"/> root node and no initial content.
    /// </summary>
    /// <param name="elementComparer">
    /// The comparer to use to determine the ordering of elements when adding to tree and performing
    /// queries. NB: For correct behaviour, the trie must be able to unambiguously order the elements of a set.
    /// As such, this comparer must only return zero for equal elements (and of course duplicates shouldn't
    /// occur in any given set).
    /// </param>
    public AsyncSetTrie(IComparer<TKeyElement> elementComparer)
        : this(elementComparer, new AsyncSetTrieDictionaryNode<TKeyElement, TValue>(), EmptyElements)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncSetTrie{TKeyElement,TValue}"/> class with a specified
    /// root node and no (additional) initial content, that uses the default comparer of the key element
    /// type to determine the ordering of elements in the tree.
    /// </summary>
    /// <param name="root">The root node of the tree.</param>
    public AsyncSetTrie(IAsyncSetTrieNode<TKeyElement, TValue> root)
        : this(Comparer<TKeyElement>.Default, root, EmptyElements)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncSetTrie{TKeyElement,TValue}"/> class with a new 
    /// <see cref="AsyncSetTrieDictionaryNode{TKeyElement,TValue}"/> root node and some initial content,
    /// that uses the default comparer of the key element type to determine the ordering of elements
    /// in the tree.
    /// </summary>
    /// <param name="content">The initial content to be added to the tree.</param>
    public AsyncSetTrie(IEnumerable<KeyValuePair<ISet<TKeyElement>, TValue>> content)
        : this(Comparer<TKeyElement>.Default, new AsyncSetTrieDictionaryNode<TKeyElement, TValue>(), content)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncSetTrie{TKeyElement,TValue}"/> class with a 
    /// specified root node and no (additional) initial content.
    /// </summary>
    /// <param name="elementComparer">
    /// The comparer to use to determine the ordering of elements when adding to tree and performing
    /// queries. NB: For correct behaviour, the trie must be able to unambiguously order the elements of a set.
    /// As such, this comparer must only return zero for equal elements (and of course duplicates shouldn't
    /// occur in any given set).
    /// </param>
    /// <param name="root">The root node of the tree.</param>
    public AsyncSetTrie(IComparer<TKeyElement> elementComparer, IAsyncSetTrieNode<TKeyElement, TValue> root)
        : this(elementComparer, root, EmptyElements)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncSetTrie{TKeyElement,TValue}"/> class with a new 
    /// <see cref="AsyncSetTrieDictionaryNode{TKeyElement,TValue}"/> root node and some (additional) initial
    /// content.
    /// </summary>
    /// <param name="elementComparer">
    /// The comparer to use to determine the ordering of elements when adding to tree and performing
    /// queries. NB: For correct behaviour, the trie must be able to unambiguously order the elements of a set.
    /// As such, this comparer must only return zero for equal elements (and of course duplicates shouldn't
    /// occur in any given set).
    /// </param>
    /// <param name="content">The (additional) content to be added to the tree (beyond any already attached to the provided root node).</param>
    public AsyncSetTrie(IComparer<TKeyElement> elementComparer, IEnumerable<KeyValuePair<ISet<TKeyElement>, TValue>> content)
        : this(elementComparer, new AsyncSetTrieDictionaryNode<TKeyElement, TValue>(), content)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncSetTrie{TKeyElement,TValue}"/> class with a 
    /// specified root node and some (additional) initial content, that uses the default comparer
    /// of the key element type to determine the ordering of elements in the tree.
    /// </summary>
    /// <param name="root">The root node of the tree.</param>
    /// <param name="content">The (additional) content to be added to the tree (beyond any already attached to the provided root node).</param>
    public AsyncSetTrie(IAsyncSetTrieNode<TKeyElement, TValue> root, IEnumerable<KeyValuePair<ISet<TKeyElement>, TValue>> content)
        : this(Comparer<TKeyElement>.Default, root, content)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncSetTrie{TKeyElement,TValue}"/> class with a 
    /// specified root node and some (additional) initial content.
    /// </summary>
    /// <param name="elementComparer">
    /// The comparer to use to determine the ordering of elements when adding to tree and performing
    /// queries. NB: For correct behaviour, the trie must be able to unambiguously order the elements of a set.
    /// As such, this comparer must only return zero for equal elements (and of course duplicates shouldn't
    /// occur in any given set).
    /// </param>
    /// <param name="root">The root node of the tree.</param>
    /// <param name="content">The (additional) content to be added to the tree (beyond any already attached to the provided root node).</param>
    public AsyncSetTrie(IComparer<TKeyElement> elementComparer, IAsyncSetTrieNode<TKeyElement, TValue> root, IEnumerable<KeyValuePair<ISet<TKeyElement>, TValue>> content)
    {
        ArgumentNullException.ThrowIfNull(root);
        ArgumentNullException.ThrowIfNull(elementComparer);
        ArgumentNullException.ThrowIfNull(content);

        this.root = root;
        this.elementComparer = elementComparer;

        foreach (var kvp in content)
        {
            AddAsync(kvp.Key, kvp.Value).GetAwaiter().GetResult();
        }
    }

    /// <summary>
    /// Adds a set and associated value to the trie.
    /// </summary>
    /// <param name="key">The set to add.</param>
    /// <param name="value">The value to associate with the set.</param>
    public async Task AddAsync(ISet<TKeyElement> key, TValue value)
    {
        ArgumentNullException.ThrowIfNull(key);

        var currentNode = root;
        foreach (var keyElement in elementComparer.Sort(key))
        {
            currentNode = await currentNode.GetOrAddChildAsync(keyElement);
        }

        await currentNode.AddValueAsync(value);
    }

    /// <summary>
    /// Removes a set from the trie.
    /// </summary>
    /// <param name="key">The set to remove.</param>
    /// <returns>A value indicating whether the set was present prior to this operation.</returns>
    public async Task<bool> RemoveAsync(ISet<TKeyElement> key)
    {
        ArgumentNullException.ThrowIfNull(key);

        var keyElements = elementComparer.Sort(key);
        return await ExpandNodeAsync(root, 0);

        async ValueTask<bool> ExpandNodeAsync(IAsyncSetTrieNode<TKeyElement, TValue> node, int keyElementIndex)
        {
            if (keyElementIndex < keyElements.Length)
            {
                var keyElement = keyElements[keyElementIndex];
                var childNode = await node.TryGetChildAsync(keyElement);

                if (childNode == null || !await ExpandNodeAsync(childNode, keyElementIndex + 1))
                {
                    return false;
                }

                if (!await childNode.GetChildren().GetAsyncEnumerator().MoveNextAsync() && !childNode.HasValue)
                {
                    await node.DeleteChildAsync(keyElement);
                }

                return true;
            }
            else
            {
                if (!node.HasValue)
                {
                    return false;
                }

                await node.RemoveValueAsync();
                return true;
            }
        }
    }

    /// <summary>
    /// Attempts to retrieve the value associated with a set.
    /// </summary>
    /// <param name="key">The set to retrieve the associated value of.</param>
    /// <returns>A task that returns a value indicating whether it was successful, and if so what the retrieved value is.</returns>
    public async Task<(bool isSucceeded, TValue? value)> TryGetAsync(ISet<TKeyElement> key)
    {
        ArgumentNullException.ThrowIfNull(key);

        var currentNode = root;
        foreach (var keyElement in elementComparer.Sort(key))
        {
            var childNode = await currentNode.TryGetChildAsync(keyElement);
            if (childNode != null)
            {
                currentNode = childNode;
            }
            else
            {
                return (false, default);
            }
        }

        if (currentNode.HasValue)
        {
            return (true, currentNode.Value);
        }

        return (false, default);
    }

    /// <summary>
    /// Returns an enumerable of the values associated with each stored subset of a given set.
    /// </summary>
    /// <param name="key">The values associated with the stored subsets of this set will be retrieved.</param>
    /// <returns>An async enumerable of the values associated with each stored subset of the given set.</returns>
    public IAsyncEnumerable<TValue> GetSubsets(ISet<TKeyElement> key)
    {
        ArgumentNullException.ThrowIfNull(key);

        var keyElements = elementComparer.Sort(key);
        return ExpandNode(root, 0);
        
        async IAsyncEnumerable<TValue> ExpandNode(IAsyncSetTrieNode<TKeyElement, TValue> node, int keyElementIndex)
        {
            if (keyElementIndex < keyElements.Length)
            {
                var childNode = await node.TryGetChildAsync(keyElements[keyElementIndex]);

                if (childNode != null)
                {
                    await foreach (var value in ExpandNode(childNode, keyElementIndex + 1))
                    {
                        yield return value;
                    }
                }

                await foreach (var value in ExpandNode(node, keyElementIndex + 1))
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
    /// Returns an enumerable of the values associated with each stored superset a given set.
    /// </summary>
    /// <param name="key">The values associated with the stored supersets of this set will be retrieved.</param>
    /// <returns>An async enumerable of the values associated with each stored superset a given set.</returns>
    public IAsyncEnumerable<TValue> GetSupersets(ISet<TKeyElement> key)
    {
        ArgumentNullException.ThrowIfNull(key);

        var keyElements = elementComparer.Sort(key);
        return ExpandNode(root, 0);

        async IAsyncEnumerable<TValue> ExpandNode(IAsyncSetTrieNode<TKeyElement, TValue> node, int keyElementIndex)
        {
            if (keyElementIndex < keyElements.Length)
            {
                await foreach (var (childKeyElement, childNode) in node.GetChildren())
                {
                    if (keyElementIndex == 0 || elementComparer.Compare(childKeyElement, keyElements[keyElementIndex - 1]) > 0)
                    {
                        var childComparedToCurrent = elementComparer.Compare(childKeyElement, keyElements[keyElementIndex]);

                        if (childComparedToCurrent <= 0)
                        {
                            var keyElementIndexOffset = childComparedToCurrent == 0 ? 1 : 0;

                            await foreach (var value in ExpandNode(childNode, keyElementIndex + keyElementIndexOffset))
                            {
                                yield return value;
                            }
                        }
                    }
                }
            }
            else
            {
                await foreach (var value in GetAllDescendentValues(node))
                {
                    yield return value;
                }
            }
        }

        async IAsyncEnumerable<TValue> GetAllDescendentValues(IAsyncSetTrieNode<TKeyElement, TValue> node)
        {
            if (node.HasValue)
            {
                yield return node.Value;
            }

            await foreach (var (_, childNode) in node.GetChildren())
            {
                await foreach (var value in GetAllDescendentValues(childNode))
                {
                    yield return value;
                }
            }
        }
    }
}
