using FluentAssertions;
using FlUnit;
using System.Collections.Generic;
using System.Linq;

namespace SCSetTrie.Tests
{
    public static class SetTrieTests
    {
        public static Test GetSubsetsBehaviour => TestThat
            .GivenEachOf(() => new LookupManyTestCase[]
            {
                new( // simple non-trivial test
                    Content: [[], [1], [2], [3], [1, 2], [1, 3], [2, 3], [1, 2, 3]],
                    Query: [1, 3],
                    ExpectedResults: [[], [1], [3], [1, 3]]),

                new( // search for non-empty set in empty trie
                    Content: [],
                    Query: [1],
                    ExpectedResults: []),

                new( // search for empty set in empty trie
                    Content: [],
                    Query: [],
                    ExpectedResults: []),

                new( // search for empty set - no match
                    Content: [[1], [2], [1, 2]],
                    Query: [],
                    ExpectedResults: []),

                new( // search for empty set - with match
                    Content: [[], [1], [2], [1, 2]],
                    Query: [],
                    ExpectedResults: [[]]),
            })
            .When(tc =>
            {
                var sut = new SetTrie<int>(new SetTrieDictionaryNode<int, ISet<int>>(), tc.Content.Select(a => new HashSet<int>(a)));
                return sut.GetSubsets(new HashSet<int>(tc.Query));
            })
            .ThenReturns((tc, rv) => rv.Should().BeEquivalentTo(tc.ExpectedResults));

        public static Test GetSupersetsBehaviour => TestThat
            .GivenEachOf(() => new LookupManyTestCase[]
            {
                new( // simple non-trivial test
                    Content: [[], [1], [2], [3], [1, 2], [1, 3], [2, 3], [1, 2, 3]],
                    Query: [1, 3],
                    ExpectedResults: [[1, 3], [1, 2, 3]]),

                new( // search for non-empty set in empty trie
                    Content: [],
                    Query: [1],
                    ExpectedResults: []),

                new( // search for empty set in empty trie
                    Content: [],
                    Query: [],
                    ExpectedResults: []),

                new( // search for empty set - without exact match
                    Content: [[1], [2], [1, 2]],
                    Query: [],
                    ExpectedResults: [[1], [2], [1, 2]]),

                new( // search for empty set - with exact match
                    Content: [[], [1], [2], [1, 2]],
                    Query: [],
                    ExpectedResults: [[], [1], [2], [1, 2]]),
            })
            .When(tc =>
            {
                var sut = new SetTrie<int>(new SetTrieDictionaryNode<int, ISet<int>>(), tc.Content.Select(a => new HashSet<int>(a)));
                return sut.GetSupersets(new HashSet<int>(tc.Query));
            })
            .ThenReturns((tc, rv) => rv.Should().BeEquivalentTo(tc.ExpectedResults));


        // TODO: tests for comparison collisions

        private record LookupManyTestCase(
            IEnumerable<IEnumerable<int>> Content,
            IEnumerable<int> Query,
            IEnumerable<IEnumerable<int>> ExpectedResults);

        private record TryLookupSingleTestCase(
            IEnumerable<IEnumerable<int>> Content,
            IEnumerable<int> Query,
            bool ExpectedResult);
    }
}
