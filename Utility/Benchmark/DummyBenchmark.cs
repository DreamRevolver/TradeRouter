using Utility.Benchmark.Default;
using Utility.Benchmark.Interfaces;

namespace Utility.Benchmark
{
    public static class DummyBenchmark
    {
        private static readonly BenchmarkDummyScopeFactory _scopeFactory = new();
        public static IBenchmarkScopeFactory<BenchmarkState> ScopeFactory => _scopeFactory;
    }
}
