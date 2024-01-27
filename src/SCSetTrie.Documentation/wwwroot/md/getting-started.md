# Getting Started

NB: This page doesn't really delve into what a [set trie](https://www.google.com/search?&q=set+trie) is,
it just explains how to use this library to store sets for fast subset and superset lookup. In case it helps -
fundamentally, set tries are just prefix tries - the only significant difference to the more common string 
prefix scenario is that the elements of the "string" (that is, set) are sorted before storage and querying.

## Using the Synchronous Implementations

The simplest type to use is the `SetTrie<TKeyElement>` type - where
TKeyElement is the type of the elements of the stored sets.
When looking at the code below, please note the following important
facts about SetTrie construction:

* SetTries require a comparer (to determine the ordering of elements in the trie)
  and a root node (that implements the backing store for the trie). However, there
  are defaults for both of these, so a parameterless constructor does exist.
* First, the comparer. It is important that set tries can unambiguously order the
  elements of a set. As such, it is important that this comparer does not return 0
  for any pairings other than those of equal elements (and of course sets shouldn't
  contain duplicates).

  The default comparer is `Comparer<TKeyElement>.Default`. This is very unlikely to
  exhibit proper behaviour if `TKeyElement` is neither `IComparable<TKeyElement>` nor
  `IComparable`. In fact, unless the *runtime* types *are* comparable, the default comparer
  just throws an exception. Our allowance of the use of the default comparer for non-comparable
  types is just to allow for cases such as storing `object`s and trusting the consumer
  that the *runtime* types can all be compared unambiguously.
  
  However, do not despair if your element types aren't comparable. To assist with the
  storage of non-comparable types, the library declares an `IComparer<T>` called
  `CollisionResolvingHashCodeComparer<T>`. This comparer uses the hash code of the 
  stored elements for ordering, making an arbitrary but consistent decision when collisions
  occur. This type can be safely used for set tries as long as, (a) your element types
  have appropriate hash code semantics for the queries that will be performed, and (b) 
  you're not doing any kind of persistence (the arbitrary but consistent decision will not
  be the same across runs). If either of those conditions don't hold, this won't be of use
  to you, and you will need to implement your own comparer (or of course make your element
  type comparable). It is of course also worth noting that this comparer has some drawbacks
  with regards to performance, especially if a lot of collisions occur (because it has to
  store all of its collision resolutions - of course, one would hope this won't happen much
  with a good hash function).
* The default root node is a new instance of the `SetTrieDictionaryNode<,>` type,
  which just stores things in memory (using a `Dictionary<,>` for child nodes).

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
values. A couple of notes:
* The `SetTrie<TKeyElement>` class demonstrated above is just a very thin utility
  wrapper around an instance of this class.
* This class also has methods and ctors that accept `IEnumerable<TKeyElement>`,
  for ease of use. These behave identically to their `ISet<TKeyElement>`-accepting
  overloads, except that they also explicitly verify that the trie's element
  comparer can unambiguously order the key - thus catching duplicate elements 
  (or of course badly-behaved comparers).

```
using SCSetTrie;

var setTrie = new SetTrie<int, string>();

setTrie.Add([], "∅");
setTrie.Add([1], "1");
setTrie.Add([3], "3");
setTrie.Add([1, 2, 3], "1-3");

// subsets will yield "∅" and "1":
IEnumerable<string> subsets = setTrie.GetSubsets([1, 2]);

// supersets will yield "3" and "1-3":
IEnumerable<string> supersets = setTrie.GetSupersets([3]);

// subsets will yield just "∅":
setTrie.Remove([1]);
subsets = setTrie.GetSubsets([1, 2]);
```

To back a trie with storage other than the dictionaries used by the library-provided node type, you will need to create an
implementation of the [ISetTrieNode&lt;TKeyElement, TValue&gt;](https://github.com/sdcondon/SCSetTrie/blob/main/src/SCSetTrie/ISetTrieNode%7BTKeyElement%2CTValue%7D.cs)
interface, and pass the instance of your type that represents the root node of the trie to the constructor. Hopefully,
[SetTrieDictionaryNode&lt;TKeyElement, TValue&gt;](https://github.com/sdcondon/SCSetTrie/blob/main/src/SCSetTrie/SetTrieDictionaryNode%7BTKeyElement%2CTValue%7D.cs)
serves as a somewhat useful example. Of course, if your implementation does any kind of IO, you probably want to be using
the async version instead - see below.

## Using the Asynchronous Implementations

Asynchronous implementations also exist, intended for use with custom node
implementations that utilise secondary storage. Moving from the synchronous 
implementation to asynchronous should be very intuitive. There are only a few
things to note (other than everything mentioned above about the comparer - which
of course still applies here):

* `Add` becomes `AddAsync` and returns a `Task`.
* `Remove` becomes `RemoveAsync` and returns a `Task<bool>`.
* `GetSubsets` and `GetSupersets` return `IAsyncEnumerable<TValue>`.
* The default root node is a new instance of a node type that still just stores 
  things in memory. Again, this trie implementation is really intended for
  custom node implementations, and this node type is really just intended as a
  basis to help people create custom implementations - but it works just fine.

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

var setTrie = new AsyncSetTrie<int, string>();

await setTrie.AddAsync([], "∅");
await setTrie.AddAsync([1], "1");
await setTrie.AddAsync([3], "3");
await setTrie.AddAsync([1, 2, 3], "1-3");

// subsets will yield "∅" and "1":
IAsyncEnumerable<string> subsets = setTrie.GetSubsets([1, 2]);

// supersets will yield "3" and "1-3":
IAsyncEnumerable<string> supersets = setTrie.GetSupersets([3]);

// subsets will yield just "∅":
await setTrie.RemoveAsync([1]);
subsets = setTrie.GetSubsets([1, 2]);
```

To back a trie with storage other than the dictionaries used by the library-provided node type, you will need to create an
implementation of the [IAsyncSetTrieNode&lt;TKeyElement, TValue&gt;](https://github.com/sdcondon/SCSetTrie/blob/main/src/SCSetTrie/IAsyncSetTrieNode%7BTKeyElement%2CTValue%7D.cs)
interface, and pass the instance of your type that represents the root node of the trie to the constructor. Hopefully,
[AsyncSetTrieDictionaryNode&lt;TKeyElement, TValue&gt;](https://github.com/sdcondon/SCSetTrie/blob/main/src/SCSetTrie/AsyncSetTrieDictionaryNode%7BTKeyElement%2CTValue%7D.cs)
serves as a somewhat useful example.
