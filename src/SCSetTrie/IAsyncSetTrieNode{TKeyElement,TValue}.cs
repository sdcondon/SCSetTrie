// Copyright © 2023-2024 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SCSetTrie;

/// <summary>
/// Interface for types capable of serving as nodes of an <see cref="AsyncSetTrie{TKeyElement, TValue}"/>.
/// </summary>
/// <typeparam name="TKeyElement">The type of each element of the stored sets.</typeparam>
/// <typeparam name="TValue">The type of the value associated with each stored set.</typeparam>
public interface IAsyncSetTrieNode<TKeyElement, TValue>
{
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
    /// Get the child nodes of this node, keyed by the element represented by the child.
    /// </summary>
    IAsyncEnumerable<KeyValuePair<TKeyElement, IAsyncSetTrieNode<TKeyElement, TValue>>> GetChildren();

    /// <summary>
    /// Attempts to retrieve a child node by the element it represents.
    /// </summary>
    /// <param name="keyElement">The element represented by the child node to retrieve.</param>
    /// <returns>The child node, or <see langword="null"/> if no matching node was found.</returns>
    ValueTask<IAsyncSetTrieNode<TKeyElement, TValue>?> TryGetChildAsync(TKeyElement keyElement);

    /// <summary>
    /// Gets or adds a child of this node.
    /// </summary>
    /// <param name="keyElement">The element represented by the retrieved or added node.</param>
    /// <returns>The retrieved or added node.</returns>
    ValueTask<IAsyncSetTrieNode<TKeyElement, TValue>> GetOrAddChildAsync(TKeyElement keyElement);

    /// <summary>
    /// Deletes a child of this node.
    /// </summary>
    /// <param name="keyElement">The element represented by the node to be removed.</param>
    ValueTask DeleteChildAsync(TKeyElement keyElement);

    /// <summary>
    /// Adds a value to this node, in so doing specifying that this node represents the "last" element of a stored set.
    /// </summary>
    /// <param name="value">The value to store.</param>
    ValueTask AddValueAsync(TValue value);

    /// <summary>
    /// Removes the value from this node, in so doing specifying that this node no longer represents the "last" element of a stored set.
    /// </summary>
    ValueTask RemoveValueAsync();
}
