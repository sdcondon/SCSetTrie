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
                new( // Empty trie
                    Content: [],
                    Query: [1],
                    ExpectedResults: []),

                new( // vanilla happy path test
                    Content: [[], [1], [1, 2]],
                    Query: [1],
                    ExpectedResults: [[], [1]]),

                new( // search for empty set - no match
                    Content: [[1], [1, 2]],
                    Query: [],
                    ExpectedResults: []),

                new( // search for empty set - with match
                    Content: [[], [1], [1, 2]],
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
                new( // Empty trie
                    Content: [],
                    Query: [1],
                    ExpectedResults: []),

                new(
                    Content: [[], [1], [1, 2]],
                    Query: [1],
                    ExpectedResults: [[1], [1, 2]]),

                new( // search for empty set - without exact match
                    Content: [[], [1], [1, 2]],
                    Query: [],
                    ExpectedResults: [[1], [1, 2]]),

                new( // search for empty set - with exact match
                    Content: [[], [1]],
                    Query: [],
                    ExpectedResults: [[], [1]]),
            })
            .When(tc =>
            {
                var sut = new SetTrie<int>(new SetTrieDictionaryNode<int, ISet<int>>(), tc.Content.Select(a => new HashSet<int>(a)));
                return sut.GetSupersets(new HashSet<int>(tc.Query));
            })
            .ThenReturns((tc, rv) => rv.Should().BeEquivalentTo(tc.ExpectedResults));


        // TODO: tests for hash code collisions

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
