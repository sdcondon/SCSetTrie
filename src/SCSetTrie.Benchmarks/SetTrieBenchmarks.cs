using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;

namespace SCSetTrie;

[MemoryDiagnoser]
[InProcess]
public class SetTrieBenchmarks
{
    private static readonly SetTrie<int> tree = new(new HashSet<int>[]
    {
        new(),
        new([1]),
        new([2]),
        new([3]),
        new([1, 2]),
        new([1, 3]),
        new([2, 3]),
        new([1, 2, 3])
    });

    private readonly Consumer consumer = new Consumer();

    [Benchmark]
    public void GetSubsets() => tree.GetSubsets(new HashSet<int>([1, 3])).Consume(consumer);

    [Benchmark]
    public void GetSupersets() => tree.GetSupersets(new HashSet<int>([1, 3])).Consume(consumer);
}
