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
                var sut = new SetTrie<int>(tc.Content.Select(a => new HashSet<int>(a)));
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
                var sut = new SetTrie<int>(tc.Content.Select(a => new HashSet<int>(a)));
                return sut.GetSupersets(new HashSet<int>(tc.Query));
            })
            .ThenReturns((tc, rv) => rv.Should().BeEquivalentTo(tc.ExpectedResults));

        public static Test RemovalBehaviour => TestThat
            .GivenEachOf(() => new RemovalTestCase[]
            {
                new( // simple non-trivial test
                    InitialContent: [[], [1], [2], [3], [1, 2], [1, 3], [2, 3], [1, 2, 3]],
                    RemovedKey: [3],
                    SubsetQuery: [1, 3],
                    ExpectedResults: [[], [1], [1, 3]]),
            })
            .When(tc =>
            {
                var sut = new SetTrie<int>(tc.InitialContent.Select(a => new HashSet<int>(a)));
                sut.Remove(new HashSet<int>(tc.RemovedKey));
                return sut.GetSubsets(new HashSet<int>(tc.SubsetQuery)).ToList();
            })
            .ThenReturns((tc, rv) => rv.Should().BeEquivalentTo(tc.ExpectedResults));

        public static Test NullValueStorage => TestThat
            .When(() =>
            {
                var sut = new SetTrie<int, string?>();
                var key = new HashSet<int>([1]);
                sut.Add(key, null);
                sut.TryGet(key, out var value);
                return value;
            })
            .ThenReturns(v => v.Should().BeNull());

        private record LookupManyTestCase(
            IEnumerable<IEnumerable<int>> Content,
            IEnumerable<int> Query,
            IEnumerable<IEnumerable<int>> ExpectedResults);

        private record TryLookupSingleTestCase(
            IEnumerable<IEnumerable<int>> Content,
            IEnumerable<int> Query,
            bool ExpectedResult);

        private record RemovalTestCase(
            IEnumerable<IEnumerable<int>> InitialContent,
            IEnumerable<int> RemovedKey,
            IEnumerable<int> SubsetQuery,
            IEnumerable<IEnumerable<int>> ExpectedResults);
    }
}
