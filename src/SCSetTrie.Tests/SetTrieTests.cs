using FluentAssertions;
using FlUnit;
using System.Collections.Generic;
using System.Linq;

namespace SCSetTrie.Tests
{
    public static class SetTrieTests
    {
        public static Test GetSubsetsBehaviour => TestThat
            .GivenEachOf(() => new LookupTestCase[]
            {
                new( // Empty trie
                    Content: new HashSet<int>[] { },
                    Query: new() { 1 },
                    ExpectedResults: Enumerable.Empty<HashSet<int>>()),

                new( // Smoke
                    Content: new HashSet<int>[]
                    {
                        new() { },
                        new() { 1 },
                        new() { 1, 2 },
                    },
                    Query: new() { 1 },
                    ExpectedResults: new HashSet<int>[]
                    {
                        new() { },
                        new() { 1 },
                    }),

                new( // search for empty set - no match
                    Content: new HashSet<int>[]
                    {
                        new() { 1 },
                        new() { 1, 2 },
                    },
                    Query: new() { },
                    ExpectedResults: new HashSet<int>[]
                    {
                    }),

                new( // search for empty set - with match
                    Content: new HashSet<int>[]
                    {
                        new() { },
                        new() { 1 },
                        new() { 1, 2 },
                    },
                    Query: new() { },
                    ExpectedResults: new HashSet<int>[]
                    {
                        new() { }
                    }),
            })
            .When(tc =>
            {
                var sut = new SetTrie<int>(tc.Content);
                return sut.GetSubsets(tc.Query);
            })
            .ThenReturns((tc, rv) => rv.Should().BeEquivalentTo(tc.ExpectedResults));

        public static Test GetSupersetsBehaviour => TestThat
            .GivenEachOf(() => new LookupTestCase[]
            {
                new( // Empty trie
                    Content: new HashSet<int>[] { },
                    Query: new() { 1 },
                    ExpectedResults: Enumerable.Empty<HashSet<int>>()),

                new( // Smoke
                    Content: new HashSet<int>[]
                    {
                        new() { },
                        new() { 1 },
                        new() { 1, 2 },
                    },
                    Query: new() { 1 },
                    ExpectedResults: new HashSet<int>[]
                    {
                        new() { 1 },
                        new() { 1, 2 },
                    }),

                new( // search for empty set - without exact match
                    Content: new HashSet<int>[]
                    {
                        new() { 1 },
                        new() { 1, 2 },
                    },
                    Query: new() { },
                    ExpectedResults: new HashSet<int>[]
                    {
                        new() { 1 },
                        new() { 1, 2 },
                    }),

                new( // search for empty set - with exact match
                    Content: new HashSet<int>[]
                    {
                        new() { },
                        new() { 1 },
                    },
                    Query: new() { },
                    ExpectedResults: new HashSet<int>[]
                    {
                        new() { },
                        new() { 1 },
                    }),
            })
            .When(tc =>
            {
                var sut = new SetTrie<int>(tc.Content);
                return sut.GetSupersets(tc.Query);
            })
            .ThenReturns((tc, rv) => rv.Should().BeEquivalentTo(tc.ExpectedResults));

        private record LookupTestCase(
            IEnumerable<HashSet<int>> Content,
            HashSet<int> Query,
            IEnumerable<HashSet<int>> ExpectedResults);
    }
}
