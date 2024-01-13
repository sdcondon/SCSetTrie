// Copyright © 2023-2024 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SCSetTrie;

/// <summary>
/// An implementation of a set trie - specifically, one for which the attached values are the sets themselves.
/// </summary>
/// <typeparam name="TKeyElement">The type of each element of the stored sets.</typeparam>
// TODO-BREAKING: Probably should have made the set type the type parameter, so that return values could be specific.
// However, would need to be AsyncSetTrie<TKey, TKeyElement> where TKey : IEnumerable/ISet<TKeyElement> - and as such would
// need a rename. Hmm..
public class AsyncSetTrie<TKeyElement>
    where TKeyElement : notnull
{
    private readonly AsyncSetTrie<TKeyElement, ISet<TKeyElement>> actualTree;

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncSetTrie{TKeyElement}"/> class with a new 
    /// <see cref="AsyncSetTrieDictionaryNode{TKeyElement,TValue}"/> root node and no initial content, that
    /// uses the default comparer of the key element type to determine the ordering of elements in the
    /// tree.
    /// </summary>
    public AsyncSetTrie()
    {
        actualTree = new();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncSetTrie{TKeyElement}"/> class with a new 
    /// <see cref="AsyncSetTrieDictionaryNode{TKeyElement,TValue}"/> root node and no initial content.
    /// </summary>
    /// <param name="elementComparer">
    /// The comparer to use to determine the ordering of elements when adding to tree and performing
    /// queries. NB: For correct behaviour, the trie must be able to unambiguously order the elements of a set.
    /// As such, this comparer must only return zero for equal elements (and of course duplicates shouldn't
    /// occur in any given set).
    /// </param>
    public AsyncSetTrie(IComparer<TKeyElement> elementComparer)
    {
        actualTree = new(elementComparer);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncSetTrie{TKeyElement}"/> class with a specified
    /// root node and no (additional) initial content, that uses the default comparer of the key element
    /// type to determine the ordering of elements in the tree.
    /// </summary>
    /// <param name="root">The root node of the tree.</param>
    public AsyncSetTrie(IAsyncSetTrieNode<TKeyElement, ISet<TKeyElement>> root)
    {
        actualTree = new(root);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncSetTrie{TKeyElement}"/> class with a new 
    /// <see cref="AsyncSetTrieDictionaryNode{TKeyElement,TValue}"/> root node and some initial content,
    /// that uses the default comparer of the key element type to determine the ordering of elements
    /// in the tree.
    /// </summary>
    /// <param name="content">The initial content to be added to the tree.</param>
    public AsyncSetTrie(IEnumerable<ISet<TKeyElement>> content)
    {
        actualTree = new(content.Select(a => KeyValuePair.Create(a, a)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncSetTrie{TKeyElement}"/> class with a 
    /// specified root node and no (additional) initial content.
    /// </summary>
    /// <param name="elementComparer">
    /// The comparer to use to determine the ordering of elements when adding to tree and performing
    /// queries. NB: For correct behaviour, the trie must be able to unambiguously order the elements of a set.
    /// As such, this comparer must only return zero for equal elements (and of course duplicates shouldn't
    /// occur in any given set).
    /// </param>
    /// <param name="root">The root node of the tree.</param>
    public AsyncSetTrie(IComparer<TKeyElement> elementComparer, IAsyncSetTrieNode<TKeyElement, ISet<TKeyElement>> root)
    {
        actualTree = new(elementComparer, root);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncSetTrie{TKeyElement}"/> class with a new 
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
    public AsyncSetTrie(IComparer<TKeyElement> elementComparer, IEnumerable<ISet<TKeyElement>> content)
    {
        actualTree = new(elementComparer, content.Select(a => KeyValuePair.Create(a, a)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncSetTrie{TKeyElement}"/> class with a 
    /// specified root node and some (additional) initial content, that uses the default comparer
    /// of the key element type to determine the ordering of elements in the tree.
    /// </summary>
    /// <param name="root">The root node of the tree.</param>
    /// <param name="content">The (additional) content to be added to the tree (beyond any already attached to the provided root node).</param>
    public AsyncSetTrie(IAsyncSetTrieNode<TKeyElement, ISet<TKeyElement>> root, IEnumerable<ISet<TKeyElement>> content)
    {
        actualTree = new(root, content.Select(a => KeyValuePair.Create(a, a)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncSetTrie{TKeyElement}"/> class with a 
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
    public AsyncSetTrie(IComparer<TKeyElement> elementComparer, IAsyncSetTrieNode<TKeyElement, ISet<TKeyElement>> root, IEnumerable<ISet<TKeyElement>> content)
    {
        actualTree = new AsyncSetTrie<TKeyElement, ISet<TKeyElement>>(elementComparer, root, content.Select(a => KeyValuePair.Create(a, a)));
    }

    /// <summary>
    /// Adds a set to the trie.
    /// </summary>
    /// <param name="key">The set to add.</param>
    /// <returns>A task representing completion of the operation.</returns>
    public Task AddAsync(ISet<TKeyElement> key) => actualTree.AddAsync(key, key);

    /// <summary>
    /// Removes a set from the trie.
    /// </summary>
    /// <param name="key">The set to remove.</param>
    /// <returns>A value indicating whether the set was present prior to this operation.</returns>
    public Task<bool> RemoveAsync(ISet<TKeyElement> key)
    {
        return actualTree.RemoveAsync(key);
    }

    /// <summary>
    /// Returns an enumerable of each stored subset of a given set.
    /// </summary>
    /// <param name="key">The stored subsets of this set will be retrieved.</param>
    /// <returns>An async enumerable each stored subset of the given set.</returns>
    public IAsyncEnumerable<ISet<TKeyElement>> GetSubsets(ISet<TKeyElement> key) => actualTree.GetSubsets(key);

    /// <summary>
    /// Returns an enumerable of teach stored superset a given set.
    /// </summary>
    /// <param name="key">The stored supersets of this set will be retrieved.</param>
    /// <returns>An async enumerable of each stored superset the given set.</returns>
    public IAsyncEnumerable<ISet<TKeyElement>> GetSupersets(ISet<TKeyElement> key) => actualTree.GetSupersets(key);
}
