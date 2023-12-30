# Home

The SCSetTrie NuGet package contains a few set trie implementations for .NET. That is, data structures for the storage of
sets that facilitate fast retrieval of subsets and supersets of a query term.

* Allows for customisation of underlying storage via an abstraction for tree nodes.
  Included in the library are node implementations that just store things in memory.
* Contains both synchronous and asynchronous implementations, to allow for efficient 
  IO for node implementations that use it. For each of those, there is a core implementation 
  that allows the stored value to differ from the set itself, and also a thin wrapper for
  which the stored value is always just the set itself.
* Allows specification of the `IComparer<T>` to use to determine the ordering of elements in
  the tree. NB: for correct behaviour, the provided implementation MUST define a "less than or 
  equal" relation on the set of elements that is "antisymmetric" - that is, the comparison can
  only return zero for equal elements. The library provides an implementation that uses hash code
  for this, making an arbitrary but consistent decision when collisions occur. Note however
  that this type is not suitable for use when any kind of persistence is involved, because these
  arbitrary decisions are not guarenteed to be the same across runs.
* ⚠ Is add-only, for the moment at least - no removals (purely because I don't need removals yet).
