# SCSetTrie

A set trie implementation for .NET.
That is, a tree data structure for the storage of sets that is intended for fast subset and superset lookup.

* Allows for storage customisation via an abstraction for tree nodes.
  Included in the library are node implementations that just store things in memory.
* Contains both synchronous and asynchronous implmentations (to allow for efficent 
  IO for node implmentations that use it). For each of those, the core implementation 
  allows the stored value to differ from the set itself, but a thin wrapper type for
  which the stored value is just the set itself is also provided.
* Allows specification of the `IComparer<T>` to use to determine the ordering of elements in
  the tree.
* ⚠ Is add-only, for the moment at least - no removals.
* ⚠ Will be resolved for 1.0 - currently no protection against collisions when comparing
  elements to determine their ordering in the tree. Is definitively a bug given that the 
  default ordering is by hash code, and hash collisions are a fact of life.
