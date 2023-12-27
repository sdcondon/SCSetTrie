using FluentAssertions;
using FlUnit;
using System.Collections.Generic;

namespace SCSetTrie.Tests
{
    public static class SetTrieTests
    {
        public static Test GetSubsetsBehaviour => TestThat
            .GivenEachOf(() => new LookupTestCase[]
            {
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
            })
            .When(tc =>
            {
                var sut = new SetTrie<int>(tc.Content);
                return sut.GetSupersets(tc.Query);
            })
            .ThenReturns((tc, rv) => rv.Should().BeEquivalentTo(tc.ExpectedResults));

        private class LookupTestCase
        {
            public IEnumerable<HashSet<int>> Content { get; init; }

            public HashSet<int> Query { get; init; }

            public IEnumerable<HashSet<int>> ExpectedResults { get; init; }
        }
    }
}
