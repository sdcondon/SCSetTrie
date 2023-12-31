# Getting Started

## Using the Synchronous Implementations

The simplest type to use is the `SetTrie<TKeyElement>` type - where
TElement is the type of the elements of the stored sets:

```
using SCSetTrie;

// SetTries require a comparer (to determine the ordering of elements in the trie)
// and a root node - but there are defaults for both of these, so a parameterless 
// constructor does exist.
// - The default comparer is a new instance of a comparer declared in the library
// called CollisionResolvingHashCodeComparer<T>. It is important that the comparer 
// defines an "antisymmetric" less-than-or-equal-to relationship between elements.
// That is, that it doesn't return 0 for any pairings other than those of equal 
// elements (and of course sets shouldn't contain duplicates). 
// CollisionResolvingHashCodeComparer does this by using the hash code of the stored
// elements for ordering, and making an arbitrary but consistent decision when
// collisions occur. Of course, for IComparable types (such as the ints in the 
// example below), its a fairly safe bet that Comparer<T>.Default is antisymmetric,
// so the recommendation is to use that where possible. This is only not a default
// behaviour out of a degree of paranoia - this may change in a future update.
// - The default root node is a new instance of the SetTrieDictionaryNode<,> type,
// which just stores things in memory (using a dictionary for child nodes).
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
values:

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
* The comparer constructor parameter is not optional. This is because the unsuitability
  of default comparer for anything involving persistence.
* The default root node is a new implmentation of a type that just stores 
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

// subsets yield [] and [1]:
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
