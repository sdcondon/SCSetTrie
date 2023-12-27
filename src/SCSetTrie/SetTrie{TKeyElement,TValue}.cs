using System;
using System.Collections.Generic;
using System.Linq;

namespace SCSetTrie
{
    public class SetTrie<TKeyElement,TValue>
    {
        private readonly ISetTrieNode<TKeyElement, TValue> root;

        /// <summary>
        /// Initializes a new instance of the <see cref="SetTrie{TKeyElement,TValue}"/> class with a new <see cref="SetTrieDictionaryNode{TKeyElement,TValue}"/> root node and no initial content.
        /// </summary>
        public SetTrie()
            : this(new SetTrieDictionaryNode<TKeyElement, TValue>(), Enumerable.Empty<KeyValuePair<ISet<TKeyElement>, TValue>>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SetTrie{TKeyElement,TValue}"/> class with a specified root node and no (additional) initial content.
        /// </summary>
        /// <param name="root">The root node of the tree.</param>
        public SetTrie(ISetTrieNode<TKeyElement, TValue> root)
            : this(root, Enumerable.Empty<KeyValuePair<ISet<TKeyElement>, TValue>>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SetTrie{TKeyElement,TValue}"/> class with a new <see cref="SetTrieDictionaryNode{TKeyElement,TValue}"/> root node and some initial content.
        /// </summary>
        /// <param name="content">The initial content to be added to the tree.</param>
        public SetTrie(IEnumerable<KeyValuePair<ISet<TKeyElement>, TValue>> content)
            : this(new SetTrieDictionaryNode<TKeyElement, TValue>(), content)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SetTrie{TKeyElement,TValue}"/> class with a specified root node and some (additional) initial content.
        /// </summary>
        /// <param name="root">The root node of the tree.</param>
        /// <param name="content">The (additional) content to be added to the tree (beyond any already attached to the provided root node).</param>
        public SetTrie(ISetTrieNode<TKeyElement, TValue> root, IEnumerable<KeyValuePair<ISet<TKeyElement>, TValue>> content)
        {
            this.root = root ?? throw new ArgumentNullException(nameof(root));
            ArgumentNullException.ThrowIfNull(content);

            foreach (var kvp in content)
            {
                Add(kvp.Key, kvp.Value);
            }
        }

        public void Add(ISet<TKeyElement> key, TValue value)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TValue> GetSubsets(ISet<TKeyElement> key)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TValue> GetSupersets(ISet<TKeyElement> key)
        {
            throw new NotImplementedException();
        }
    }
}
