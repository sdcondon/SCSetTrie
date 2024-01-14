// Copyright © 2023-2024 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System.Collections.Generic;
using System.Linq;

namespace SCSetTrie;

/// <summary>
/// An implementation of a set trie - specifically, one for which the attached values are the sets themselves.
/// </summary>
/// <typeparam name="TKeyElement">The type of each element of the stored sets.</typeparam>
// TODO-BREAKING: Might be improved by making the set type a type parameter, so that return values could be specific.
// However, would need to be SetTrie<TKey, TKeyElement> where TKey : IEnumerable/ISet<TKeyElement> - and as such would
// need a rename to not clash. Hmm. Also, there's value in having a single type parameter - the SetTrie<K> and SetTrie<K,V>
// pairing is intuitive. Something like DeterminateValueSetTrie<KE,V> instead might be better if we need two type params
// anyway?
// TODO-BREAKING: changes to allow for IEnum<>-accepting methods, for ease-of-use. Perhaps should just use IEnumerable<>
// for the underlying values, not ISet<>. We don't actually use ISet<>, its just to push the "no duplicates" angle.
// But the no duplicates angle is perhaps important - this is after all a **set** trie. Hmm.. This type is annoying.
public class SetTrie<TKeyElement>
    where TKeyElement : notnull
{
    private readonly SetTrie<TKeyElement, ISet<TKeyElement>> actualTree;

    /// <summary>
    /// Initializes a new instance of the <see cref="SetTrie{TKeyElement}"/> class with a new 
    /// <see cref="SetTrieDictionaryNode{TKeyElement,TValue}"/> root node and no initial content, that
    /// uses the default comparer of the key element type to determine the ordering of elements in the
    /// tree.
    /// </summary>
    public SetTrie()
    {
        actualTree = new();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SetTrie{TKeyElement}"/> class with a new 
    /// <see cref="SetTrieDictionaryNode{TKeyElement,TValue}"/> root node and no initial content.
    /// </summary>
    /// <param name="elementComparer">
    /// The comparer to use to determine the ordering of elements when adding to tree and performing
    /// queries. NB: For correct behaviour, the trie must be able to unambiguously order the elements of a set.
    /// As such, this comparer must only return zero for equal elements (and of course duplicates shouldn't
    /// occur in any given set).
    /// </param>
    public SetTrie(IComparer<TKeyElement> elementComparer)
    {
        actualTree = new(elementComparer);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SetTrie{TKeyElement}"/> class with a specified
    /// root node and no (additional) initial content, that uses the default comparer of the key element
    /// type to determine the ordering of elements in the tree.
    /// </summary>
    /// <param name="root">The root node of the tree.</param>
    public SetTrie(ISetTrieNode<TKeyElement, ISet<TKeyElement>> root)
    {
        actualTree = new(root);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SetTrie{TKeyElement}"/> class with a new 
    /// <see cref="SetTrieDictionaryNode{TKeyElement,TValue}"/> root node and some initial content, that
    /// uses the default comparer of the key element type to determine the ordering of elements in the
    /// tree.
    /// </summary>
    /// <param name="content">The initial content to be added to the tree.</param>
    public SetTrie(IEnumerable<ISet<TKeyElement>> content)
    {
        actualTree = new(content.Select(t => KeyValuePair.Create(t, t)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SetTrie{TKeyElement}"/> class with a 
    /// specified root node and no (additional) initial content.
    /// </summary>
    /// <param name="elementComparer">
    /// The comparer to use to determine the ordering of elements when adding to tree and performing
    /// queries. NB: For correct behaviour, the trie must be able to unambiguously order the elements of a set.
    /// As such, this comparer must only return zero for equal elements (and of course duplicates shouldn't
    /// occur in any given set).
    /// </param>
    /// <param name="root">The root node of the tree.</param>
    public SetTrie(IComparer<TKeyElement> elementComparer, ISetTrieNode<TKeyElement, ISet<TKeyElement>> root)
    {
        actualTree = new(elementComparer, root);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SetTrie{TKeyElement}"/> class with a new 
    /// <see cref="SetTrieDictionaryNode{TKeyElement,TValue}"/> root node and some (additional) initial
    /// content.
    /// </summary>
    /// <param name="elementComparer">
    /// The comparer to use to determine the ordering of elements when adding to tree and performing
    /// queries. NB: For correct behaviour, the trie must be able to unambiguously order the elements of a set.
    /// As such, this comparer must only return zero for equal elements (and of course duplicates shouldn't
    /// occur in any given set).
    /// </param>
    /// <param name="content">The (additional) content to be added to the tree (beyond any already attached to the provided root node).</param>
    public SetTrie(IComparer<TKeyElement> elementComparer, IEnumerable<ISet<TKeyElement>> content)
    {
        actualTree = new(elementComparer, content.Select(t => KeyValuePair.Create(t, t)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SetTrie{TKeyElement}"/> class with a 
    /// specified root node and some (additional) initial content, that uses the default 
    /// comparer of the key element type to determine the ordering of elements in the tree.
    /// in the tree.
    /// </summary>
    /// <param name="root">The root node of the tree.</param>
    /// <param name="content">The (additional) content to be added to the tree (beyond any already attached to the provided root node).</param>
    public SetTrie(ISetTrieNode<TKeyElement, ISet<TKeyElement>> root, IEnumerable<ISet<TKeyElement>> content)
    {
        actualTree = new(root, content.Select(t => KeyValuePair.Create(t, t)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SetTrie{TKeyElement}"/> class with a 
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
    public SetTrie(IComparer<TKeyElement> elementComparer, ISetTrieNode<TKeyElement, ISet<TKeyElement>> root, IEnumerable<ISet<TKeyElement>> content)
    {
        actualTree = new(elementComparer, root, content.Select(t => KeyValuePair.Create(t, t)));
    }

    /// <summary>
    /// Adds a set to the trie.
    /// </summary>
    /// <param name="key">The set to add.</param>
    public void Add(ISet<TKeyElement> key)
    {
        actualTree.Add(key, key);
    }

    /// <summary>
    /// Removes a set from the trie.
    /// </summary>
    /// <param name="key">The set to remove.</param>
    /// <returns>A value indicating whether the set was present prior to this operation.</returns>
    public bool Remove(ISet<TKeyElement> key)
    {
        return actualTree.Remove(key);
    }

    /// <summary>
    /// Determines whether a given set (matched exactly) is present in the trie.
    /// </summary>
    /// <param name="key">The set to retrieve the associated value of.</param>
    /// <returns>True if and only if the set is present in the trie.</returns>
    public bool Contains(ISet<TKeyElement> key) => actualTree.TryGet(key, out _);

    /// <summary>
    /// Returns an enumerable of each stored subset of a given set.
    /// </summary>
    /// <param name="key">The stored subsets of this set will be retrieved.</param>
    /// <returns>An enumerable each stored subset of the given set.</returns>
    public IEnumerable<ISet<TKeyElement>> GetSubsets(ISet<TKeyElement> key)
    {
        return actualTree.GetSubsets(key);
    }

    /// <summary>
    /// Returns an enumerable of teach stored superset a given set.
    /// </summary>
    /// <param name="key">The stored supersets of this set will be retrieved.</param>
    /// <returns>An enumerable of each stored superset the given set.</returns>
    public IEnumerable<ISet<TKeyElement>> GetSupersets(ISet<TKeyElement> key)
    {
        return actualTree.GetSupersets(key);
    }
}
