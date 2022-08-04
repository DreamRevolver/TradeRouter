using JetBrains.Annotations;

namespace Utility.Benchmark.Interfaces
{
    [PublicAPI]
    public interface IBenchmarkTape
    {
        void Start();
        void Stop();
    }
}
