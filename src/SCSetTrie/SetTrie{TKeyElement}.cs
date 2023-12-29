// Copyright © 2023 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System.Collections.Generic;
using System.Linq;

namespace SCSetTrie;

/// <summary>
/// An implementation of a set trie - specifically, one for which the attached values are the sets themselves.
/// </summary>
/// <typeparam name="TKeyElement">The type of each element of the stored sets.</typeparam>
public class SetTrie<TKeyElement>
    where TKeyElement : notnull
{
    private readonly SetTrie<TKeyElement, ISet<TKeyElement>> actualTree;

    /// <summary>
    /// Initializes a new instance of the <see cref="SetTrie{TKeyElement}"/> class with a new 
    /// <see cref="SetTrieDictionaryNode{TKeyElement,TValue}"/> root node and no initial content, that
    /// uses hash code to determine the ordering of elements in the tree.
    /// </summary>
    public SetTrie()
    {
        actualTree = new();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SetTrie{TKeyElement}"/> class with a specified
    /// root node and no (additional) initial content, that uses hash code to determine the ordering of elements
    /// in the tree.
    /// </summary>
    /// <param name="root">The root node of the tree.</param>
    public SetTrie(ISetTrieNode<TKeyElement, ISet<TKeyElement>> root)
    {
        actualTree = new(root);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SetTrie{TKeyElement}"/> class with a new 
    /// <see cref="SetTrieDictionaryNode{TKeyElement,TValue}"/> root node and no initial content, that
    /// uses a specified comparer to determine the ordering of elements in the tree.
    /// </summary>
    /// <param name="elementComparer">the comparer to use to determine the ordering of elements in the tree.</param>
    public SetTrie(IComparer<TKeyElement> elementComparer)
    {
        actualTree = new(elementComparer);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SetTrie{TKeyElement}"/> class with a new <see cref="SetTrieDictionaryNode{TKeyElement,TValue}"/> root node and some initial content.
    /// </summary>
    /// <param name="content">The initial content to be added to the tree.</param>
    public SetTrie(IEnumerable<ISet<TKeyElement>> content)
    {
        actualTree = new(content.Select(t => KeyValuePair.Create(t, t)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SetTrie{TKeyElement}"/> class with a 
    /// specified root node and no (additional) initial content, that uses a specified comparer to
    /// determine the ordering of elements in the tree.
    /// </summary>
    /// <param name="root">The root node of the tree.</param>
    /// <param name="elementComparer">the comparer to use to determine the ordering of elements in the tree.</param>
    public SetTrie(ISetTrieNode<TKeyElement, ISet<TKeyElement>> root, IComparer<TKeyElement> elementComparer)
    {
        actualTree = new(root, elementComparer);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SetTrie{TKeyElement}"/> class with a 
    /// specified root node and some (additional) initial content, that uses hash code to determine
    /// the ordering of elements in the tree.
    /// </summary>
    /// <param name="root">The root node of the tree.</param>
    /// <param name="content">The (additional) content to be added to the tree (beyond any already attached to the provided root node).</param>
    public SetTrie(ISetTrieNode<TKeyElement, ISet<TKeyElement>> root, IEnumerable<ISet<TKeyElement>> content)
    {
        actualTree = new(root, content.Select(t => KeyValuePair.Create(t, t)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SetTrie{TKeyElement}"/> class with a new 
    /// <see cref="SetTrieDictionaryNode{TKeyElement,TValue}"/> root node and some (additional) initial
    /// content, that uses hash code to determine the ordering of elements in the tree.
    /// </summary>
    /// <param name="elementComparer">the comparer to use to determine the ordering of elements in the tree.</param>
    /// <param name="content">The (additional) content to be added to the tree (beyond any already attached to the provided root node).</param>
    public SetTrie(IComparer<TKeyElement> elementComparer, IEnumerable<ISet<TKeyElement>> content)
    {
        actualTree = new(elementComparer, content.Select(t => KeyValuePair.Create(t, t)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SetTrie{TKeyElement}"/> class with a 
    /// specified root node and some (additional) initial content, that a specified comparer to
    /// determine the ordering of elements in the tree.
    /// </summary>
    /// <param name="root">The root node of the tree.</param>
    /// <param name="elementComparer">the comparer to use to determine the ordering of elements in the tree.</param>
    /// <param name="content">The (additional) content to be added to the tree (beyond any already attached to the provided root node).</param>
    public SetTrie(ISetTrieNode<TKeyElement, ISet<TKeyElement>> root, IComparer<TKeyElement> elementComparer, IEnumerable<ISet<TKeyElement>> content)
    {
        actualTree = new(root, elementComparer, content.Select(t => KeyValuePair.Create(t, t)));
    }

    public void Add(ISet<TKeyElement> key)
    {
        actualTree.Add(key, key);
    }

    /// <summary>
    /// Returns an enumerable of each stored subset of a given set.
    /// </summary>
    /// <param name="key"></param>
    /// <returns>An enumerable each stored subset of the given set.</returns>
    public IEnumerable<ISet<TKeyElement>> GetSubsets(ISet<TKeyElement> key)
    {
        return actualTree.GetSubsets(key);
    }

    public IEnumerable<ISet<TKeyElement>> GetSupersets(ISet<TKeyElement> key)
    {
        return actualTree.GetSupersets(key);
    }
}
