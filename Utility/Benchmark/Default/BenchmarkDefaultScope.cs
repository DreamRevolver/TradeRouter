using System;
using Utility.Benchmark.Interfaces;
namespace Utility.Benchmark.Default
{

	internal sealed class BenchmarkDefaultScope : IBenchmarkScope
	{

		private readonly BenchmarkStateChangeHandler<BenchmarkState> _handler;
		internal BenchmarkDefaultScope(BenchmarkStateChangeHandler<BenchmarkState> handler)
		{
			_handler = handler;
			_state = BenchmarkState.Initialize();
		}

		private BenchmarkState _state;
		private void IterationHandler(BenchmarkIteration data)
		{
			_state.Apply(data);
			_handler(ref _state);
		}

		public IDisposable Context => new BenchmarkDefaultContext(IterationHandler);
		public IBenchmarkTape Tape => new BenchmarkDefaultTape(IterationHandler);


	}

}
