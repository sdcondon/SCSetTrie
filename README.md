# SCSetTrie

A set trie implementation for .NET, allowing for fast subset and superset lookup.

* Contains synchronous and asynchronous implementations.
* Allows for storage customisation via node type abstraction.
  Included in the library are implmentations that store things in memory.
* Allows specification of the IComparer to use to determine the ordering of elements in
  the tree.
* Is add-only, for the moment at least - no removals.
* Will be resolved for 1.0 - currently no protection against collisions when comparing
  elements to determine their ordering in the tree. Is definitively a bug given that the 
  default ordering is by hash code.
