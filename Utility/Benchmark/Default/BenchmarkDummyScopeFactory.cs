using System;
using Utility.Benchmark.Interfaces;

namespace Utility.Benchmark.Default
{
    internal class BenchmarkDummyScopeFactory : IBenchmarkScopeFactory<BenchmarkState>
    {
        private sealed class BenchmarkDummyScope : IBenchmarkScope
        {
            private sealed class BenchmarkDummyContext : IDisposable
            {
                public void Dispose() { }
            }
            private sealed class BenchmarkDummyTape : IBenchmarkTape
            {
                public void Start() { }
                public void Stop() { }
            }
            private static readonly IDisposable _context = new BenchmarkDummyContext();
            private static readonly IBenchmarkTape _tape = new BenchmarkDummyTape();
            public IDisposable Context => _context;
            public IBenchmarkTape Tape => _tape;
        }
        private readonly IBenchmarkScope _scope = new BenchmarkDummyScope();
        public IBenchmarkScope MakeScope(BenchmarkStateChangeHandler<BenchmarkState> _) => _scope;
    }
}
