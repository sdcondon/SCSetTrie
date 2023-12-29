// Copyright © 2023 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System.Collections.Generic;

namespace SCSetTrie;

public interface ISetTrieNode<TKeyElement, TValue>
{
    IReadOnlyDictionary<TKeyElement, ISetTrieNode<TKeyElement, TValue>> Children { get; }

    /// <summary>
    /// Gets a value indicating whether a value is stored against this node.
    /// That is, whether this node represents the "last" element of a stored set.
    /// </summary>
    bool HasValue { get; }

    /// <summary>
    /// Gets the value stored against the node. Should throw an <see cref="InvalidOperationException"/> 
    /// if no value has been stored against the node.
    /// </summary>
    TValue Value { get; } 

    ISetTrieNode<TKeyElement, TValue> GetOrAddChildNode(TKeyElement keyElement);

    void AddValue(TValue value);
}
