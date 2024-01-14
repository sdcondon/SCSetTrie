// Copyright © 2023-2024 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System;
using System.Collections.Generic;

namespace SCSetTrie;

/// <summary>
/// Interface for types capable of serving as nodes of a <see cref="SetTrie{TKeyElement, TValue}"/>.
/// </summary>
/// <typeparam name="TKeyElement">The type of each element of the stored sets.</typeparam>
/// <typeparam name="TValue">The type of the value associated with each stored set.</typeparam>
public interface ISetTrieNode<TKeyElement, TValue>
{
    /// <summary>
    /// Gets the child nodes of this node, keyed by the element represented by the child.
    /// </summary>
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

    /// <summary>
    /// Gets or adds a child of this node.
    /// </summary>
    /// <param name="keyElement">The element represented by the node to be retrieved or added.</param>
    /// <returns>The retrieved or added node.</returns>
    ISetTrieNode<TKeyElement, TValue> GetOrAddChildNode(TKeyElement keyElement);

    /// <summary>
    /// Deletes a child of this node.
    /// </summary>
    /// <param name="keyElement">The element represented by the node to be removed.</param>
    // TODO-BREAKING: Arrrgh, I used "ChildNode" above, but just "Child" here. Really shouldn't bug me but does.
    // Not worth a major version bump, but the next time I'm making breaking changes anyway..
    void DeleteChild(TKeyElement keyElement);

    /// <summary>
    /// Adds a value to this node, in so doing specifying that this node represents the "last" element of a stored set.
    /// </summary>
    /// <param name="value">The value to store.</param>
    void AddValue(TValue value);

    /// <summary>
    /// Removes the value from this node, in so doing specifying that this node no longer represents the "last" element of a stored set.
    /// </summary>
    void RemoveValue();
}
