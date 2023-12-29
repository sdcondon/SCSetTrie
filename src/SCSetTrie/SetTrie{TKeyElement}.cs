// Copyright © 2023 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System.Collections.Generic;
using System.Linq;

namespace SCSetTrie;

/// <summary>
/// An implementation of a set trie - specifically, one for which the attached values are the sets themselves.
/// </summary>
public class SetTrie<TKeyElement>
{
    private readonly SetTrie<TKeyElement, ISet<TKeyElement>> actualTree;

    /// <summary>
    /// Initializes a new instance of the <see cref="SetTrie{TKeyElement}"/> class.
    /// </summary>
    ////public SetTrie()
    ////{
    ////    actualTree = new();
    ////}

    /// <summary>
    /// Initializes a new instance of the <see cref="SetTrie{TKeyElement}"/> class with a specified root node.
    /// </summary>
    public SetTrie(ISetTrieNode<TKeyElement, ISet<TKeyElement>> root)
    {
        actualTree = new(root);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SetTrie{TKeyElement}"/> class with some initial content.
    /// </summary>
    ////public SetTrie(IEnumerable<ISet<TKeyElement>> content)
    ////{
    ////    actualTree = new(content.Select(t => KeyValuePair.Create(t, t)));
    ////}

    /// <summary>
    /// Initializes a new instance of the <see cref="SetTrie{TKeyElement}"/> class with a specified root node and some initial content.
    /// </summary>
    public SetTrie(ISetTrieNode<TKeyElement, ISet<TKeyElement>> root, IEnumerable<ISet<TKeyElement>> content)
    {
        actualTree = new(root, content.Select(t => KeyValuePair.Create(t, t)));
    }

    public void Add(ISet<TKeyElement> key)
    {
        actualTree.Add(key, key);
    }

    public IEnumerable<ISet<TKeyElement>> GetSubsets(ISet<TKeyElement> key)
    {
        return actualTree.GetSubsets(key);
    }

    public IEnumerable<ISet<TKeyElement>> GetSupersets(ISet<TKeyElement> key)
    {
        return actualTree.GetSupersets(key);
    }
}
