# Getting Started

## Using the Synchronous Implementations

The simplest type to use is the `SetTrie<TKeyElement>` type - where
TKeyElement is the type of the elements of the stored sets.
When looking at the code below, please note the following important
facts about SetTrie construction:

* SetTries require a comparer (to determine the ordering of elements in the trie)
  and a root node (that implements the backing store for the trie). However, there
  are defaults for both of these, so a parameterless constructor does exist.
* First, the comparer. It is important that set tries can unambiguously order the
  elements of a set. As such it is important that this comparer does not return 0
  for any pairings other than those of equal elements (and of course sets shouldn't
  contain duplicates).

  The default comparer is `Comparer<TKeyElement>.Default`. This is very unlikely to
  exhibit proper behaviour if `TKeyElement` is neither `IComparable<TKeyElement>` nor
  `IComporable`. In fact, unless the *runtime* types *are* comparable, the default comparer
  just throws an exception. Our allowance of the use of the default comparer for non-comparable
  types is just to allow for cases such as storing `object`s and trusting the consumer
  that the *runtime* types can all be compared unambiguously.
  
  However, do not despair if your element types aren't comparable. To assist with the
  storage of non-comparable types, the library does declares an `IComparer<T>` called
  `CollisionResolvingHashCodeComparer<T>`. CollisionResolvingHashCodeComparer uses the
  hash code of the stored elements for ordering, making an arbitrary but consistent
  decision when collisions occur. This type can be safely used for set tries as long as,
  (a) your element types have appropriate hash code semantics, and (b) you're not doing 
  any kind of persistence (the arbitrary but consistent will not be the same across runs).
  If either of those conditions don't hold, this won't be of use to you, and you will
  need to implement your own comparer (or of course make your element type comparable).
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

// subsets will yield just []:
setTrie.Remove(new HashSet<int>([1]));
subsets = setTrie.GetSubsets(new HashSet<int>([1, 2]));
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
* `Remove` becomes `AddAsync` and returns a `Task<bool>`.
* `GetSubsets` and `GetSupersets` return `IAsyncEnumerable<TValue>`
* The default root node is a new implementation of a type that still just stores 
  things in memory. Again, this trie implementation is really intended for
  custom node implementations, but there's no actual problem with just keeping
  stuff in memory, so this default remains.

Where the value to return is the stored set itself:

```
using SCSetTrie;

var setTrie = new AsyncSetTrie<int>();

await setTrie.AddAsync(new HashSet<int>([]));
await setTrie.AddAsync(new HashSet<int>([1]));
await setTrie.AddAsync(new HashSet<int>([3]));
await setTrie.AddAsync(new HashSet<int>([1, 2, 3]));

// subsets will yield [] and [1]:
IAsyncEnumerable<ISet<int>> subsets = setTrie.GetSubsets(new HashSet<int>([1, 2]));

// supersets will yield [3] and [1, 2, 3]:
IAsyncEnumerable<ISet<int>> supersets = setTrie.GetSupersets(new HashSet<int>([3]));

// subsets will yield just []:
await setTrie.RemoveAsync(new HashSet<int>([1]));
subsets = setTrie.GetSubsets(new HashSet<int>([1, 2]));
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
