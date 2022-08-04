using Utility.Benchmark.Interfaces;

namespace Utility.Benchmark.Default
{
    internal class BenchmarkDefaultScopeFactory : IBenchmarkScopeFactory<BenchmarkState>
    {
        public IBenchmarkScope MakeScope(BenchmarkStateChangeHandler<BenchmarkState> handler)
            => new BenchmarkDefaultScope(handler);
    }
}
