using System;
using System.Diagnostics;

namespace Utility.Benchmark.Default
{

    internal readonly struct BenchmarkDefaultContext : IDisposable
    {

		private readonly BenchmarkIterationHandler _iterationHandler;
		private readonly Stopwatch _stopwatch;
		private readonly long _timeStart;

		internal BenchmarkDefaultContext(BenchmarkIterationHandler iterationHandler)
		{
			_iterationHandler = iterationHandler;
			_stopwatch = Stopwatch.StartNew();
			_timeStart = DateTimeOffset.Now.ToUnixTimeMilliseconds();
		}

		internal bool IsDisposed => !_stopwatch.IsRunning;

		public void Dispose()
		{
			_stopwatch.Stop();
			_iterationHandler(new(_stopwatch.ElapsedMilliseconds, DateTimeOffset.Now.ToUnixTimeMilliseconds() - _timeStart));
		}

    }

}
