using System;

using JetBrains.Annotations;

namespace Utility.Benchmark.Interfaces
{
    [PublicAPI]
    public interface IBenchmarkScope
    {
        IDisposable Context { get; }
        IBenchmarkTape Tape { get; }
    }
}
