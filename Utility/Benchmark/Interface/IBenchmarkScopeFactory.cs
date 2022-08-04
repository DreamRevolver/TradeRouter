namespace Utility.Benchmark.Interfaces
{

    public delegate void BenchmarkStateChangeHandler<T>(ref T state) where T : struct;
    public interface IBenchmarkScopeFactory<T> where T : struct
    {
        IBenchmarkScope MakeScope(BenchmarkStateChangeHandler<T> handler);
    }

}
