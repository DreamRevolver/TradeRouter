using System;
using Utility.Benchmark.Interfaces;

namespace Utility.Benchmark.Default
{

	internal readonly struct BenchmarkDefaultTape : IBenchmarkTape
    {

		private readonly Lazy<BenchmarkDefaultContext> _context;
		internal BenchmarkDefaultTape(BenchmarkIterationHandler iterationHandler)
			=> _context = new(() => new(iterationHandler));

        public void Start()
        {
			if (_context.IsValueCreated || _context.Value.IsDisposed)
			{
				throw new($"attempt to start BenchmarkTape which already {(_context.IsValueCreated ? "started" : "stopped")}");
			}
        }

        public void Stop()
        {
			if (!_context.IsValueCreated)
			{
				throw new("attempt to stop BenchmarkTape which not started");
			}
			if (_context.Value.IsDisposed)
			{
				throw new("attempt to stop BenchmarkTape which already stopped");
			}
			_context.Value.Dispose();
        }

    }

}
