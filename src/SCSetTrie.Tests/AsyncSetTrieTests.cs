using FluentAssertions;
using FlUnit;
using System.Collections.Generic;
using System.Linq;

namespace SCSetTrie.Tests
{
    public static class AsyncSetTrieTests
    {
        public static Test ContainsBehaviour => TestThat
            .GivenEachOf(() => new TryLookupSingleTestCase[]
            {
                new( // simple non-trivial test
                    Content: [[], [1], [2], [3], [1, 2], [1, 3], [2, 3], [1, 2, 3]],
                    Query: [1, 3],
                    ExpectedResult: true),

                new( // not found
                    Content: [[], [1], [2], [3], [1, 2], [1, 3], [2, 3], [1, 2, 3]],
                    Query: [1, 4],
                    ExpectedResult: false),

                new( // search for empty set - not found
                    Content: [[1], [2], [1, 2]],
                    Query: [],
                    ExpectedResult: false),

                new( // search for empty set - found
                    Content: [[], [1], [2], [1, 2]],
                    Query: [],
                    ExpectedResult: true),
            })
            .WhenAsync(async tc =>
            {
                var sut = new AsyncSetTrie<int>(tc.Content.Select(a => new HashSet<int>(a)));
                return await sut.ContainsAsync(new HashSet<int>(tc.Query));
            })
            .ThenReturns((tc, rv) => rv.Should().Be(tc.ExpectedResult));

        public static Test GetSubsetsBehaviour => TestThat
            .GivenEachOf(() => new LookupManyTestCase[]
            {
                new( // simple non-trivial test
                    Content: [[], [1], [2], [3], [1, 2], [1, 3], [2, 3], [1, 2, 3]],
                    Query: [1, 3],
                    ExpectedResults: [[], [1], [3], [1, 3]]),

                new( // get all
                    Content: [[], [1], [2], [3], [1, 2], [1, 3], [2, 3], [1, 2, 3]],
                    Query: [1, 2, 3],
                    ExpectedResults: [[], [1], [2], [3], [1, 2], [1, 3], [2, 3], [1, 2, 3]]),

                new( // search empty trie
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
            .WhenAsync(async tc =>
            {
                var sut = new AsyncSetTrie<int>(tc.Content.Select(a => new HashSet<int>(a)));
                return await sut.GetSubsets(new HashSet<int>(tc.Query)).ToListAsync();
            })
            .ThenReturns((tc, rv) => rv.Should().BeEquivalentTo(tc.ExpectedResults));

        public static Test GetSupersetsBehaviour => TestThat
            .GivenEachOf(() => new LookupManyTestCase[]
            {
                new( // simple non-trivial test
                    Content: [[], [1], [2], [3], [1, 2], [1, 3], [2, 3], [1, 2, 3]],
                    Query: [1, 3],
                    ExpectedResults: [[1, 3], [1, 2, 3]]),

                new( // get all
                    Content: [[], [1], [2], [3], [1, 2], [1, 3], [2, 3], [1, 2, 3]],
                    Query: [],
                    ExpectedResults: [[], [1], [2], [3], [1, 2], [1, 3], [2, 3], [1, 2, 3]]),

                new( // get none
                    Content: [[], [1], [2], [3], [1, 2], [1, 3], [2, 3], [1, 2, 3]],
                    Query: [0],
                    ExpectedResults: []),

                new( // search empty trie
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
            .WhenAsync(async tc =>
            {
                var sut = new AsyncSetTrie<int>(tc.Content.Select(a => new HashSet<int>(a)));
                return await sut.GetSupersets(new HashSet<int>(tc.Query)).ToListAsync();
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
            .WhenAsync(async tc =>
            {
                var sut = new AsyncSetTrie<int>(tc.InitialContent.Select(a => new HashSet<int>(a)));
                sut.RemoveAsync(new HashSet<int>(tc.RemovedKey)).GetAwaiter().GetResult();
                return await sut.GetSubsets(new HashSet<int>(tc.SubsetQuery)).ToListAsync();
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

        private record TryLookupSingleTestCase(
            IEnumerable<IEnumerable<int>> Content,
            IEnumerable<int> Query,
            bool ExpectedResult);

        private record LookupManyTestCase(
            IEnumerable<IEnumerable<int>> Content,
            IEnumerable<int> Query,
            IEnumerable<IEnumerable<int>> ExpectedResults);

        private record RemovalTestCase(
            IEnumerable<IEnumerable<int>> InitialContent,
            IEnumerable<int> RemovedKey,
            IEnumerable<int> SubsetQuery,
            IEnumerable<IEnumerable<int>> ExpectedResults);
    }
}
