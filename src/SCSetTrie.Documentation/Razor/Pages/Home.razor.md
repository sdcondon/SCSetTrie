# Home

The SCSetTrie NuGet package contains a few [set trie](https://www.google.com/search?&q=set+trie) implementations for .NET.
That is, data structures for the storage of sets that facilitate fast retrieval of subsets and supersets of a query term.

* Allows for customisation of underlying storage via an abstraction for tree nodes.
  Included in the library are node implementations that just store things in memory.
* Allows for specification of the `IComparer<T>` to use to determine the ordering of elements in
  the tree (set tries require that set elements can be unambiguously ordered).
* Contains both synchronous and asynchronous implementations. The asynchronous implementations
  are intended to allow for efficient IO for node implementations that use it (NB individual
  queries are not parallelised).
* Contains core implementations that allow arbitrary values to be associated with each
  stored set, as well as thin utility wrappers for which the stored value is always just 
  the set itself (because this is probably a particularly common scenario).