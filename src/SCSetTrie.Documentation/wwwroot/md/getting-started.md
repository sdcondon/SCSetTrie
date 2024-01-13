# Getting Started

## Using the Synchronous Implementations

The simplest type to use is the `SetTrie<TKeyElement>` type - where
TKeyElement is the type of the elements of the stored sets.
When looking at the code below, please note the following important
facts about SetTrie construction:

* SetTries require a comparer (to determine the ordering of elements in the trie)
  and a root node (that implements the backing store for the trie). However, there
  are defaults for both of these, so a parameterless constructor does exist.
* The default comparer is a new instance of a comparer declared in the library
  called `CollisionResolvingHashCodeComparer<T>`. It is important that set tries can
  unambiguously order the elements of a set. That is, it is important that the comparer 
  they use defines an "antisymmetric" less-than-or-equal-to relationship between elements.
  That is, that it doesn't return 0 for any pairings other than those of equal 
  elements (and of course sets shouldn't contain duplicates).
  CollisionResolvingHashCodeComparer does this by using the hash code of the stored
  elements for ordering, and making an arbitrary but consistent decision when
  collisions occur. Of course, for IComparable types (such as the ints in the 
  example below), it is a fairly safe bet that `Comparer<T>.Default` defines an
  antisymmetric less-than-or-equal relationship, so the recommendation is to use that
  where possible. This is only not a default behaviour out of a degree of paranoia -
  this may change in a future update.
* The default root node is a new instance of the `SetTrieDictionaryNode<,>` type,
  which just stores things in memory (using a Dictionary for child nodes).

```
using SCSetTrie;

var setTrie = new SetTrie<int>();

setTrie.Add(new HashSet<int>([]));
setTrie.Add(new HashSet<int>([1]));
setTrie.Add(new HashSet<int>([3]));
setTrie.Add(new HashSet<int>([1, 2, 3]));

// subsets will yield [] and [1]:
IEnumerable<ISet<int>> subsets = setTrie.GetSubsets(new HashSet<int>([1, 2]));

// supersets will yield [3] and [1, 2, 3]:
IEnumerable<ISet<int>> supersets = setTrie.GetSupersets(new HashSet<int>([3]));
```

Attaching values other than the sets themselves can be accomplished with the
`SetTrie<TKeyElement, TValue>` type - where TValue is the type of the attached
values. Note that the `SetTrie<TKeyElement>` class demonstrated above is just a
very thin utility wrapper around an instance of this class:

```
using SCSetTrie;

var setTrie = new SetTrie<int, string>();

setTrie.Add(new HashSet<int>([]), "∅");
setTrie.Add(new HashSet<int>([1]), "1");
setTrie.Add(new HashSet<int>([3]), "3");
setTrie.Add(new HashSet<int>([1, 2, 3]), "1-3");

// subsets will yield "∅" and "1":
IEnumerable<string> subsets = setTrie.GetSubsets(new HashSet<int>([1, 2]));

// supersets will yield "3" and "1-3":
IEnumerable<string> supersets = setTrie.GetSupersets(new HashSet<int>([3]));
```

## Using the Asynchronous Implementations

Asynchronous implementations also exist, intended for use with custom node
implementations that utilise secondary storage. Moving from the synchronous 
implementation to asynchronous should be very intuitive.
There are only a few things to note:

* `Add` becomes `AddAsync` and returns a `Task`.
* `GetSubsets` and `GetSupersets` return `IAsyncEnumerable<TValue>`
* The comparer constructor parameter is not optional. This is because of the
  unsuitability of default comparer for anything involving persistence -
  the "arbitrary but consistent decision" it makes when collisions occur will
  not necessarily be the same across runs.
* The default root node is a new implementation of a type that just stores 
  things in memory. Again, this trie implementation is really intended for
  custom node implementations, but there's no actual problem with just keeping
  stuff in memory, so this default remains.

Where the value to return is the stored set itself:

```
using SCSetTrie;

var setTrie = new AsyncSetTrie<int>(Comparer<int>.Default);

await setTrie.AddAsync(new HashSet<int>([]));
await setTrie.AddAsync(new HashSet<int>([1]));
await setTrie.AddAsync(new HashSet<int>([3]));
await setTrie.AddAsync(new HashSet<int>([1, 2, 3]));

// subsets will yield [] and [1]:
IAsyncEnumerable<ISet<int>> subsets = setTrie.GetSubsets(new HashSet<int>([1, 2]));

// supersets will yield [3] and [1, 2, 3]:
IAsyncEnumerable<ISet<int>> supersets = setTrie.GetSupersets(new HashSet<int>([3]));
```

Where the value to return is something other than the stored set:

```
using SCSetTrie;

var setTrie = new AsyncSetTrie<int, string>(Comparer<int>.Default);

await setTrie.AddAsync(new HashSet<int>([]), "∅");
await setTrie.AddAsync(new HashSet<int>([1]), "1");
await setTrie.AddAsync(new HashSet<int>([3]), "3");
await setTrie.AddAsync(new HashSet<int>([1, 2, 3]), "1-3");

// subsets will yield "∅" and "1":
IAsyncEnumerable<string> subsets = setTrie.GetSubsets(new HashSet<int>([1, 2]));

// supersets will yield "3" and "1-3":
IAsyncEnumerable<string> supersets = setTrie.GetSupersets(new HashSet<int>([3]));
```
