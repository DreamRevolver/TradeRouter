namespace Utility.Benchmark.Default
{
    internal delegate void BenchmarkIterationHandler(BenchmarkIteration data);
    internal readonly struct BenchmarkIteration
    {
        public readonly long elapsed;
        public readonly long delay;
        public BenchmarkIteration(long elapsed, long delay)
        {
            this.elapsed = elapsed;
            this.delay = delay;
        }
    }
}
