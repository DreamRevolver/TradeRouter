using Utility.Benchmark.Default;
using Utility.Benchmark.Interfaces;

namespace Utility.Benchmark
{
    public static class DefaultBenchmark
    {
        private static readonly BenchmarkDefaultScopeFactory _scopeFactory = new();
        public static IBenchmarkScopeFactory<BenchmarkState> ScopeFactory => _scopeFactory;
    }
}
